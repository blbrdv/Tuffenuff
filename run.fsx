#r "nuget: docopt.net, 0.8.1"
#r "nuget: Fli, 1.111.10"

open System

[<Literal>]
let private list = "--list"

[<Literal>]
let private version = "--version"

[<Literal>]
let private leTruth = "TRUE"

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
    let private dotnetSilent = "-v q"

    [<Literal>]
    let private dotnetNormal = "-v n"

    [<Literal>]
    let private dotnetVerbose = "-v d"

    [<Literal>]
    let private rebuild = "--rebuild"

    [<Literal>]
    let private filter = "--filter"

    [<Literal>]
    let private env = "-e"

    [<Literal>]
    let private targetVar = "<TARGET>"

    [<Literal>]
    let private filterVar = "<FILTER>"

    [<Literal>]
    let private envVar = "<ENV>"

    let doc =
        $"""Automation script.

Usage:
    script {targetVar} [{silent}|{normal}|{verbose}] [{rebuild}] [{filter} {filterVar}] [{env} {envVar}...]
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
    {filter}      Sets filter expression for tests run.
    {env}            Sets environment variables."""

    let inline private mapping (o : obj) = 
        let str = o |> string
        let temp = str.Split "="

        if temp.Length = 1 then
            (str, leTruth)
        else
            let value = temp[1..] |> String.concat "="
            (temp[0], value)

    type DotnetVerbosity =
        | Quiet
        | Normal
        | Verbose

        override this.ToString() =
            match this with
            | Quiet -> dotnetSilent
            | Normal -> dotnetNormal
            | Verbose -> dotnetVerbose

        member this.IsMaxLevel with get() = this = Verbose

    type Arguments =
        {
            Target: string
            DotnetVerbosity: DotnetVerbosity
            Rebuild: bool
            Filter: string option
            Env: (string * string) list
        }

        static member Create(dict : IDictionary<string, ValueObject>) =
            {
                Target = dict[targetVar].ToString() |> sprintf "-t %s"

                DotnetVerbosity =
                    if dict[verbose].IsTrue then
                        DotnetVerbosity.Verbose
                    elif dict[normal].IsTrue then
                        DotnetVerbosity.Normal
                    else
                        DotnetVerbosity.Quiet

                Rebuild = dict[rebuild].IsTrue

                Filter =
                    if dict[filter].IsTrue then
                        dict[filterVar].Value.ToString().Replace(" ", "%20")
                        |> sprintf "--filter %s"
                        |> Some
                    else
                        None

                Env =
                    let verbosityEnv =
                        if dict[verbose].IsTrue then
                            [ ("FAKE_FORCE_VERBOSITY", leTruth) ]
                        else
                            List.empty

                    // for some reason if no --filter provided, Docopt put first env into
                    // filter value
                    let docoptBug =
                        if dict[env].IsTrue && dict[filter].IsFalse then
                            [ (mapping dict[filterVar].Value) ]
                        else
                            List.empty

                    let otherEnvs =
                        if dict[env].IsTrue then
                            (dict[envVar].Value :?> ArrayList).ToArray ()
                            |> Array.map mapping
                            |> List.ofArray
                        else
                            List.empty

                    verbosityEnv @ docoptBug @ otherEnvs 
            }

        static member Create(target : string) =
            {
                Target = target
                DotnetVerbosity = Quiet
                Rebuild = false
                Filter = None
                Env = List.Empty
            }

/// Printing to console with colors no matter supported ANSI escape character (\u001b)
/// or not
[<RequireQualifiedAccess>]
module private Console =
    open System.IO
    open System.Text.RegularExpressions

    [<Literal>]
    let githubActions = "GITHUB_ACTIONS"

    [<Literal>]
    let term = "TERM"

    type private Command =
        | Print of string
        | ColorChange of int

    let private regex = Regex @"\u001b\[(\d+)m"

    let private isSupportANSIColors =
        let envs = Environment.GetEnvironmentVariables ()

        (
            envs.Contains githubActions &&
            envs[githubActions].ToString().ToUpper().Equals(leTruth)
        ) || (
            envs.Contains term &&
            not (String.IsNullOrEmpty(envs[term].ToString()))
        ) ||
        envs.Contains "TERMINAL_EMULATOR"

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

    let inline stdout (text : string) = print Console.Out text

    let inline stderr (text : string) = print Console.Error text

/// Bunch of cli commands
module private Commands =
    open System.IO
    open Fli
    open Arguments

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

    /// Path to .fsproj file of CI/CD project.
    let private fsprojPath = Path.Combine (projDir, $"%s{projName}.fsproj")

    /// Current .NET runtime version.
    let private runtime = Environment.Version

    /// Current target framework.
    let private target = $"net%d{runtime.Major}.%d{runtime.Minor}"

    /// Path to exe file of CI/CD project.
    let private exePath =
        let fileName =
            if OperatingSystem.IsLinux() ||
               OperatingSystem.IsMacOS()
            then
                projName
            else
                $"%s{projName}.exe"

        Path.Combine (projDir, "bin", "Debug", target, fileName)

    /// List of env keys, value which must be redacted.
    let private badKeys = seq { "NUGET_API_KEY" } |> Seq.readonly

    /// Print command error to stderr and exit with its exit code.
    let inline private printError (command : string) (result : Output) =
        Console.stderr
            $"%s{red}Command \"%s{command}\" \
            exited with code %d{result.ExitCode}.%s{reset}"

        if result.Error.IsSome then
            Console.stderr
                $"%s{red}Error message:%s{eol}\
                %s{result.Error.Value}%s{reset}"
        else
            Console.stderr ""

        exit result.ExitCode

    /// Execute binary/executable from "executable" with "verbosity", "args" and
    /// environment variables from "envs".
    /// If "return" is true - return executable output from stdout, else print it.
    /// If executable returns non-zero exit code print it and exit with this code.
    let inline private exec
        (return' : bool)
        (verbosity : DotnetVerbosity)
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
                if verbosity.IsMaxLevel then
                    let envs =
                        context.config.EnvironmentVariables.Value
                        |> List.map (fun (key, value) ->
                            if Seq.contains key badKeys then
                                (key, "<REDACTED>")
                            else
                                (key, value)
                        )

                    Console.stdout $"%s{darkGray}Exec \"%s{command}\" %A{envs}%s{reset}"

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
                Console.stdout $"%s{result.Text.Value}"

            if result.ExitCode <> 0 then
                if result.Text.IsSome then
                    Console.stdout ""

                printError command result

            String.Empty

    /// Check if files in "path" changed since last time.
    let inline private isModified (path : string) (verbosity : DotnetVerbosity) =
        let result = exec true verbosity "git" $"diff --dirstat=files -- %s{path}" []

        not (String.Empty.Equals result)

    /// Add files from "path" to Git index.
    let inline private add (path : string) (verbosity : DotnetVerbosity) =
        let glob = Path.Combine (path, "**")
        exec false verbosity "git" $"add %s{glob}" [] |> ignore

    /// Execute "dotnet" cli with "args" and environment variables from "envs".
    let inline private dotnet
        (verbosity : DotnetVerbosity)
        (envs : (string * string) list)
        (args : string)
        =
        exec false verbosity "dotnet" args envs |> ignore

    /// Execute "dotnet run" command with "args" and "--no-build" flag if cicd project
    /// has not changed since the last time.
    let inline run (args : Arguments) =
        let noBuild =
            if args.Rebuild ||
               not (File.Exists exePath) ||
               isModified projDir args.DotnetVerbosity
            then
                Console.stdout "Building CI/CD project..."
                String.Empty
            else
                "--no-build"

        let moreArgs =
            match args.Filter with
            | Some flag -> $"-- %s{flag}"
            | None -> String.Empty

        $"run %s{args.DotnetVerbosity.ToString()} %s{noBuild} \
        --project %s{fsprojPath} \
        -- \
        %s{args.Target} \
        %s{moreArgs}"
        |> dotnet args.DotnetVerbosity args.Env

        if noBuild.Length = 0 then
            add projDir args.DotnetVerbosity

    /// Execute "dotnet run" command on cicd project with "flag" silently.
    let inline print (flag : string) =
        Arguments.Create(flag) |> run
        exit 0

/// Duration prettifying
module Duration =
    let private add (value : int) (suffix : string) (prev : string option) =
        match prev with
        | None ->
            if value = 0 then
                None
            else
                value.ToString($"00'%s{suffix}'") |> Some
        | Some v ->
            value.ToString($"00'%s{suffix}'")
            |> sprintf "%s %s" v
            |> Some

    /// Prettify TimeSpan.
    let toString (duration : TimeSpan) : string =
        let rest =
            if duration.Days = 0 then
                None
            else
                duration.Days.ToString("##'d'") |> Some
            |> add duration.Hours "h"
            |> add duration.Minutes "m"
            |> add duration.Seconds "s"

        let ms = duration.Milliseconds.ToString("000'ms'")

        match rest with
        | None -> ms
        | Some value -> $"%s{value} %s{ms}"

// ---------------------------------------------------------------------------------------
// ** Start of this script **
// ---------------------------------------------------------------------------------------

open System.Diagnostics
open DocoptNet
open Arguments
open Commands
open Duration

let private cliArgs = fsi.CommandLineArgs[1..]

if cliArgs.Length = 0 then
    Console.stdout $"%s{doc}"
    exit 0

let private argsRaw = Docopt().Apply (doc, cliArgs, exit = true)

if argsRaw[list].IsTrue then
    print list

if argsRaw[version].IsTrue then
    print version

let private sw = Stopwatch ()

try
    Console.stdout "Starting..."
    sw.Start ()

    argsRaw |> Arguments.Create |> run 

    sw.Stop ()
finally
    sw.Elapsed
    |> toString
    |> sprintf "Finished [%s]"
    |> Console.stdout
