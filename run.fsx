#r "nuget: docopt.net, 0.8.1"
#r "nuget: Fli, 1.111.10"

open System

[<Literal>]
let private dotnetSilent = "-v q"

[<Literal>]
let private dotnetNormal = "-v n"

[<Literal>]
let private dotnetVerbose = "-v d"

[<Literal>]
let private list = "--list"

[<Literal>]
let private version = "--version"

/// Command line argument parsing essentials
module private Arguments =
    open System.Collections
    open System.Collections.Generic
    open DocoptNet

    [<Literal>]
    let private silent = "-s"

    [<Literal>]
    let private normal = "-n"

    [<Literal>]
    let private verbose = "-v"

    [<Literal>]
    let private rebuild = "--rebuild"

    [<Literal>]
    let private env = "-e"

    [<Literal>]
    let private targetVar = "<TARGET>"

    [<Literal>]
    let private envVAR = "<ENV>"

    let doc =
        $"""Automation script.

Usage:
    script {targetVar} [{silent}|{normal}|{verbose}] [{rebuild}] [{env} {envVAR} ...]
    script (-h | --help)
    script {list}
    script {version}

Options:
    -h --help     Print this.
    {list}        Print list of available targets.
    {version}     Print module version.
    {silent}            Silent trace level.
    {normal}            Normal trace level.
    {verbose}            Verbose trace level.
    {rebuild}     Force building CICD project.
    {env}            Sets environment variables."""

    type Arguments(dict : IDictionary<string, ValueObject>) =
        member _.Target = dict[targetVar].ToString ()

        member _.Verbosity =
            if dict[verbose].IsTrue then verbose
            elif dict[normal].IsTrue then normal
            else silent

        member _.Rebuild = dict[rebuild].IsTrue

        member this.DotnetVerbosity =
            if verbose.Equals this.Verbosity then dotnetVerbose
            elif normal.Equals this.Verbosity then dotnetNormal
            else dotnetSilent

        member this.Env =
            let verbosityEnv =
                if verbose.Equals this.Verbosity then
                    [ ("FAKE_FORCE_VERBOSITY", "TRUE") ]
                else
                    List.empty

            let otherEnvs =
                if dict[env].IsTrue then
                    // "Yes please give me an OBJECT of the VALUE of the ELEMENT
                    // from the dictionary,
                    // then downcast it to an ArrayList,
                    // then copy the elements of it to the new Array"
                    // - Statements dreamed up by the utterly Deranged
                    (dict[envVAR].Value :?> ArrayList).ToArray ()
                    |> Array.map (fun o ->
                        let str = o |> string
                        let temp = str.Split "="

                        if temp.Length = 1 then
                            (str, "TRUE")
                        else
                            let value = temp[1..] |> String.concat "="
                            (temp[0], value)
                    )
                    |> List.ofArray
                else
                    List.empty

            verbosityEnv @ otherEnvs

/// Printing to console with colors no matter supported ANSII escape character (\u001b)
/// or not
[<RequireQualifiedAccess>]
module private Console =
    open System.IO
    open System.Text.RegularExpressions

    [<Literal>]
    let githubActions = "GITHUB_ACTIONS"

    [<Literal>]
    let term = "TERM"

    [<Literal>]
    let leTruth = "TRUE"

    type private Command =
        | Print of string
        | ColorChange of int

    let private regex = Regex @"\u001b\[(\d+)m"

    let private isSupportANSIColors =
        let envs = Environment.GetEnvironmentVariables ()

        (
            envs.Contains githubActions &&
            envs[githubActions].ToString().ToUpper().Equals("TRUE")
        ) || (
            envs.Contains term &&
            not (String.IsNullOrEmpty(envs[term].ToString()))
        )

    let inline private print (writer : TextWriter) (text : string) =
        if isSupportANSIColors then
            writer.WriteLine text
        else
            text.Split Environment.NewLine
            |> Array.map (fun value ->
                let result = ResizeArray<Command> ()
                let mutable lastIndex = 0

                let matches = regex.Matches value

                if matches.Count = 0 then
                    value
                    |> Print
                    |> result.Add 
                else
                    for match' in matches do     
                        let num = match'.Groups[1].Value

                        if match'.Index = 0 then
                            num
                            |> Int32.Parse
                            |> ColorChange
                            |> result.Add

                            lastIndex <- match'.Length
                        else
                            value.Substring(lastIndex, match'.Index - lastIndex)
                            |> Print
                            |> result.Add

                            num
                            |> Int32.Parse
                            |> ColorChange
                            |> result.Add

                            lastIndex <- match'.Index + match'.Length

                    let substr = value.Substring(lastIndex)

                    if not (String.IsNullOrEmpty substr) then
                        substr
                        |> Print
                        |> result.Add

                result.ToArray()
            )
            |> Array.iter (fun list ->
                list
                |> Array.iter (fun command ->
                    match command with
                    | Print s -> writer.Write s
                    | ColorChange i ->
                        match i with
                        | 0 -> Console.ResetColor()
                        | 30 -> Console.ForegroundColor <- ConsoleColor.Black
                        | 31 -> Console.ForegroundColor <- ConsoleColor.Red
                        | 32 -> Console.ForegroundColor <- ConsoleColor.Green
                        | 33 -> Console.ForegroundColor <- ConsoleColor.Yellow
                        | 34 -> Console.ForegroundColor <- ConsoleColor.Blue
                        | 35 -> Console.ForegroundColor <- ConsoleColor.Magenta
                        | 36 -> Console.ForegroundColor <- ConsoleColor.Cyan
                        | 37 -> Console.ForegroundColor <- ConsoleColor.Gray
                        | 90 -> Console.ForegroundColor <- ConsoleColor.DarkGray
                        | 91 -> Console.ForegroundColor <- ConsoleColor.DarkRed
                        | 92 -> Console.ForegroundColor <- ConsoleColor.DarkGreen
                        | 93 -> Console.ForegroundColor <- ConsoleColor.DarkYellow
                        | 94 -> Console.ForegroundColor <- ConsoleColor.DarkBlue
                        | 95 -> Console.ForegroundColor <- ConsoleColor.DarkMagenta
                        | 96 -> Console.ForegroundColor <- ConsoleColor.DarkCyan
                        | 97 -> Console.ForegroundColor <- ConsoleColor.White
                        | _ -> ()
                )

                writer.Write Environment.NewLine
            )

    let inline printn (text : string) = print Console.Out text

    let inline eprintn (text : string) = print Console.Error text

/// Bunch of cli commands
module private Commands =
    open System.IO
    open Fli

    [<Literal>]
    let private darkGray = "\u001b[90m"

    [<Literal>]
    let private red = "\u001b[31m"

    [<Literal>]
    let private reset = "\u001b[0m"

    [<Literal>]
    let private projName = "cicd"

    /// End of line symbol(s) as string.
    let private eol = Environment.NewLine

    /// Path to directory of CI/CD project.
    let private projDir = Path.Combine (".", projName)

    /// Path to exe file of CI/CD project.
    let private exePath =
        let fileName =
            if OperatingSystem.IsLinux() then
                Path.Combine("linux-x64", projName)
            elif OperatingSystem.IsMacOS() then
                Path.Combine("osx-x64", projName)
            else
                $"%s{projName}.exe"

        Path.Combine (projDir, "bin", "Debug", "net6.0", fileName)

    let inline testBin () =
        let path = Path.Combine (projDir, "bin", "Debug", "net6.0")
        
        if Directory.Exists path then
            Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            |> Array.iter (fun file -> printfn $"%s{file}")

    /// List of env keys, value which must be redacted.
    let private badKeys = seq { "NUGET_API_KEY" } |> Seq.readonly

    /// Print command error to stderr and exit with its exit code.
    let inline private printError (command : string) (result : Output) =
        Console.eprintn
            $"%s{red}Command \"%s{command}\" \
            exited with code %d{result.ExitCode}.%s{reset}"

        if result.Error.IsSome then
            Console.eprintn
                $"%s{red}Error message:%s{eol}\
                %s{result.Error.Value}%s{reset}"
        else
            Console.eprintn ""

        exit result.ExitCode

    /// Execute binary/executable from "executable" with "args" and environment
    /// variables from "envs".
    /// If "isVerbose" is true - print context before executing.
    /// If "return" is true - return executable output from stdout, else print it.
    /// If executable returns non-zero exit code print it and exit with this code.
    let inline private exec
        (return' : bool)
        (isVerbose : bool)
        (executable : string)
        (args : string)
        (envs : (string * string) list)
        =
        let command = $"%s{executable} %s{args}"

        let result =
            cli {
                Exec executable
                Arguments args
                EnvironmentVariables envs
            }
            |> (fun context ->
                if isVerbose then
                    let envs =
                        context.config.EnvironmentVariables.Value
                        |> List.map (fun (key, value) ->
                            if Seq.contains key badKeys then
                                (key, "<REDACTED>")
                            else
                                (key, value)
                        )

                    Console.printn $"%s{darkGray}Exec \"%s{command}\" %A{envs}%s{reset}"

                context
            )
            |> Command.execute

        if return' then
            if result.ExitCode <> 0 then
                printError command result

            match result.Text with
            | Some value -> value
            | None -> String.Empty
        else
            if result.Text.IsSome then
                Console.printn $"%s{result.Text.Value}"

            if result.ExitCode <> 0 then
                if result.Text.IsSome then
                    Console.printn ""

                printError command result

            String.Empty

    /// Check if files in "path" changed since last time.
    let inline private isModified (path : string) (isVerbose : bool) =
        let result = exec true isVerbose "git" $"diff --dirstat=files -- %s{path}" []

        not (String.Empty.Equals result)

    /// Add files from "path" to Git index.
    let inline private add (path : string) (isVerbose : bool) =
        let glob = Path.Combine (path, "**")
        exec false isVerbose "git" $"add %s{glob}" [] |> ignore

    /// Execute "dotnet" cli with "args" and environment variables from "envs".
    let inline private dotnet
        (isVerbose : bool)
        (envs : (string * string) list)
        (args : string)
        =
        exec false isVerbose "dotnet" args envs |> ignore

    /// Execute "dotnet run" command on cicd project with "args", environment variables
    /// from "envs", "verbosity" option and "--no-build" flag if cicd project has not
    /// changed since the last time.
    let inline run
        (verbosity : string)
        (rebuild : bool)
        (envs : (string * string) list)
        (args : string seq)
        =
        let isVerbose = dotnetVerbose.Equals verbosity

        let noBuild =
            if rebuild || not (File.Exists exePath) || isModified projDir isVerbose then
                String.Empty
            else
                "--no-build"

        if String.Empty.Equals noBuild then
            Console.printn "Building CI/CD project..."

        let fsprojPath = Path.Combine (projDir, $"%s{projName}.fsproj")
        let commandArgs = $"run %s{verbosity} %s{noBuild} --project %s{fsprojPath}"

        if (Seq.length args) > 0 then
            seq {
                yield commandArgs
                yield "--"
                yield! args
            }
            |> String.concat " "
        else
            commandArgs
        |> dotnet isVerbose envs

        if String.Empty.Equals noBuild then
            add projDir isVerbose

    /// Execute "dotnet run" command on cicd project with "flag" silently.
    let inline print (flag : string) =
        run dotnetSilent false [] [ flag ]
        exit 0

// ---------------------------------------------------------------------------------------
// ** Start of this script **
// ---------------------------------------------------------------------------------------

open Arguments

let private cliArgs = fsi.CommandLineArgs[1..]

if cliArgs.Length = 0 then
    Console.printn $"%s{doc}"
    exit 0

let private scriptArgs =
    cliArgs
    |> Array.findIndex "--".Equals
    |> (+) 1
    |> Array.skip
    <| cliArgs

open DocoptNet

let private argsRaw = Docopt().Apply (doc, scriptArgs, exit = true)

open Commands

if argsRaw[list].IsTrue then
    print list

if argsRaw[version].IsTrue then
    print version

let private args = argsRaw |> Arguments

open System.Diagnostics

let private sw = Stopwatch ()

try
    Console.printn "Starting..."
    sw.Start ()

    testBin ()

    seq { $"-t %s{args.Target}" }
    |> run args.DotnetVerbosity args.Rebuild args.Env

    sw.Stop ()
finally
    sw.Elapsed.ToString(@"hh\:mm\:ss\.fff")
    |> sprintf "Finished [%s]"
    |> Console.printn
