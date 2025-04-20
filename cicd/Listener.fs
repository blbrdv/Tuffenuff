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
    let inline printLine (text : string) = text |> printfn "%s"
    let inline printNewLine () = printLine ""

    let inline wrap (color : string) (text : string) = $"%s{color}%s{text}%s{reset}"

    let inline printGreen (text : string) = text |> wrap green |> print
    let inline printYellow (text : string) = text |> wrap yellow |> print
    let inline printRed (text : string) = text |> wrap red |> print
    let inline printDarkGray (text : string) = text |> wrap darkGray |> print

    let inline printLineGreen (text : string) = text |> wrap green |> printLine
    let inline printLineYellow (text : string) = text |> wrap yellow |> printLine
    let inline printLineRed (text : string) = text |> wrap red |> printLine
    let inline printLineDarkGray (text : string) = text |> wrap darkGray |> printLine

    let inline printWithBreakLine (text : string) (breakLine : bool) =
        if breakLine then printLine text else print text

    let groupStartStr = "The running order is:"
    let groupPattern = Regex @"Group - (\d+)(?:\n|\r|\r\n)  - (\w[\w\d]+)"

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
                    printLine $"Import data %A{type'} to \"%s{path}\"."

            | TraceData.BuildNumber value ->
                if isVerbose then
                    printLine $"Build number: %s{value}"

            | TraceData.ImportantMessage text -> printLineYellow text

            | TraceData.ErrorMessage text -> printLineRed text

            | TraceData.LogMessage (text, lineBreak) ->
                if isVerbose then
                    printWithBreakLine text lineBreak
                else
                    if text.StartsWith groupStartStr then
                        let matches = groupPattern.Matches text

                        if matches.Count <> 0 then
                            printNewLine ()
                            printLine groupStartStr

                            matches
                            |> Seq.iter (fun m ->
                                printLine $"%s{m.Groups[1].Value} - %s{m.Groups[2].Value}"
                            )

                            printNewLine ()
                    elif text.StartsWith "Shortened DependencyGraph" then
                        ()
                    else
                        printWithBreakLine text lineBreak

            | TraceData.TraceMessage (text, lineBreak) ->
                if isVerbose then
                    if lineBreak then printLineGreen text else printGreen text
                elif text.StartsWith "Building project with version: " then
                    printLineDarkGray text

            | TraceData.OpenTag (tag, description) ->
                $" [>] Starting %A{tag}" |> print

                match description with
                | Some value ->
                    print " "
                    printDarkGray $"\"%s{value}\""
                | None -> ()

                printNewLine ()

            | TraceData.TestOutput (name, out, err) ->
                if isVerbose then
                    printLine $"Test \"%s{name}\":"
                    printLine out
                    printLineRed err

            | TraceData.TestStatus (name, status) ->
                if isVerbose then
                    match status with
                    | TestStatus.Ignored message ->
                        printLineDarkGray $"Test \"%s{name}\" ignored: \"%s{message}\""
                    | TestStatus.Failed (message, details, comparison) ->
                        printLineRed $"Test \"%s{name}\" failed: \"%s{message}\""

                        match comparison with
                        | Some (expected, actual) ->
                            printLineRed $"Expected: %s{expected}"
                            printLineRed $"Actual: %s{actual}"
                        | None -> ()

                        printLineDarkGray details

            | TraceData.CloseTag (tag, time, status) ->
                print " "

                match status with
                | TagStatus.Success -> printGreen "[=]"
                | TagStatus.Warning -> printYellow "[!]"
                | TagStatus.Failed -> printRed "[X]"

                print " "

                $"Finished %A{tag} [%s{time.ToReadableString ()}]" |> print

                print " ("

                match status with
                | TagStatus.Success -> printGreen "SUCCESS"
                | TagStatus.Warning -> printYellow "WARNING"
                | TagStatus.Failed -> printRed "FAILED"

                print ")"

                printNewLine ()

            | TraceData.BuildState (status, _) ->
                if isVerbose then
                    printLine $"Build state: %A{status}"
