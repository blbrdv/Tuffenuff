module CICD.Listener

module private MinimalListener =
    open System.Text.RegularExpressions

    [<Literal>]
    let reset = "\u001b[0m"

    [<Literal>]
    let green = "\u001b[32m"

    [<Literal>]
    let yellow = "\u001b[33m"

    [<Literal>]
    let red = "\u001b[31m"

    [<Literal>]
    let darkGray = "\u001b[90m"

    let inline print (text : string) = text |> printf "%s"
    let inline printn (text : string) = text |> printfn "%s"
    let inline printnl () = printn ""

    let inline wrap (color : string) (text : string) = $"%s{color}%s{text}%s{reset}"

    let inline printg (text : string) = text |> wrap green |> print
    let inline printy (text : string) = text |> wrap yellow |> print
    let inline printr (text : string) = text |> wrap red |> print
    let inline printdg (text : string) = text |> wrap darkGray |> print

    let inline printgn (text : string) = text |> wrap green |> printn
    let inline printyn (text : string) = text |> wrap yellow |> printn
    let inline printrn (text : string) = text |> wrap red |> printn
    let inline printdgn (text : string) = text |> wrap darkGray |> printn

    let inline printLine (text : string) (breakLine : bool) =
        if breakLine then printn text else print text

    let optsStr = "TRACEOPTIONS>>"

    let groupStartStr = "The running order is:"
    let groupPattern = Regex (@"Group - (\d+)(?:\n|\r|\r\n)  - (\w[\w\d]+)")

open MinimalListener
open Fake.Core
open CICD.TimeSpanExtensions

[<Sealed>]
type MinimalListener() =
    interface ITraceListener with

        member x.Write (msg : TraceData) =
            match msg with
            | TraceData.ImportData (type', path) ->
                if isVerbose then
                    printn $"Import data %A{type'} to \"%s{path}\"."

            | TraceData.BuildNumber value ->
                if isVerbose then
                    printn $"Build number: %s{value}"

            | TraceData.ImportantMessage text -> printyn text

            | TraceData.ErrorMessage text -> printrn text

            | TraceData.LogMessage (text, lineBreak) ->
                if isVerbose then
                    printLine text lineBreak
                else
                    if text.StartsWith optsStr then
                        printdgn text[optsStr.Length ..]
                    elif text.StartsWith groupStartStr then
                        let matches = groupPattern.Matches text

                        if matches.Count <> 0 then
                            printnl ()
                            printn groupStartStr

                            matches
                            |> Seq.iter (fun m ->
                                printn $"%s{m.Groups[1].Value} - %s{m.Groups[2].Value}"
                            )

                            printnl ()
                    elif text.StartsWith "Shortened DependencyGraph" then
                        ()
                    else
                        printLine text lineBreak

            | TraceData.TraceMessage (text, lineBreak) ->
                if isVerbose then
                    if lineBreak then printgn text else printg text
                elif text.StartsWith "Building project with version: " then
                    printdgn text

            | TraceData.OpenTag (tag, description) ->
                $" [>] Starting %A{tag}" |> print

                match description with
                | Some value ->
                    print " "
                    printdg $"\"%s{value}\""
                | None -> ()

                printnl ()

            | TraceData.TestOutput (name, out, err) ->
                if isVerbose then
                    printn $"Test \"%s{name}\":"
                    printn out
                    printrn err

            | TraceData.TestStatus (name, status) ->
                if isVerbose then
                    match status with
                    | TestStatus.Ignored message ->
                        printdgn $"Test \"%s{name}\" ignored: \"%s{message}\""
                    | TestStatus.Failed (message, details, comparison) ->
                        printrn $"Test \"%s{name}\" failed: \"%s{message}\""

                        match comparison with
                        | Some (expected, actual) ->
                            printrn $"Expected: %s{expected}"
                            printrn $"Actual: %s{actual}"
                        | None -> ()

                        printdgn details

            | TraceData.CloseTag (tag, time, status) ->
                print " "

                match status with
                | TagStatus.Success -> printg "[=]"
                | TagStatus.Warning -> printy "[!]"
                | TagStatus.Failed -> printr "[X]"

                print " "

                $"Finished %A{tag} [%s{time.ToReadableString ()}]" |> print

                print " ("

                match status with
                | TagStatus.Success -> printg "SUCCESS"
                | TagStatus.Warning -> printy "WARNING"
                | TagStatus.Failed -> printr "FAILED"

                print ")"

                printnl ()

            // TODO: find out what is "something"
            | TraceData.BuildState (status, something) ->
                //if isVerbose then
                printn $"Build %A{status} %A{something}"
