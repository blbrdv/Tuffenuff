#r "nuget: docopt.net, 0.8.1"
#r "nuget: Fli, 1.111.10"

open System
open System.IO
open System.Collections
open System.Collections.Generic
open DocoptNet
open Fli

// ** Args **

[<Literal>]
let silent = "-s"

[<Literal>]
let normal = "-n"

[<Literal>]
let verbose = "-v"

[<Literal>]
let rebuild = "--rebuild"

[<Literal>]
let env = "-e"

[<Literal>]
let targetVar = "<TARGET>"

[<Literal>]
let envVAR = "<ENV>"

[<Literal>]
let list = "--list"

[<Literal>]
let version = "--version"

[<Literal>]
let dotnetSilent = "-v q"

[<Literal>]
let dotnetNormal = "-v n"

[<Literal>]
let dotnetVerbose = "-v d"

let doc =
    $"""Automation script.

Usage:
    script {targetVar} [{silent}|{normal}|{verbose}] [{rebuild}] [{env} {envVAR} ...]
    script (-h | --help)
    script {list}
    script {version}

Options:
  -h --help     Print this.
  {list}        Print list of defined targets.
  {version}     Print module version.
  {silent}            Silent trace level.
  {normal}            Normal trace level.
  {verbose}            Verbose trace level.
  {rebuild}     Force building CICD project.
  {env}            Sets environment variables.

"""

type Args(dict : IDictionary<string, ValueObject>) =
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

// ** Print **

[<Literal>]
let white = "\u001b[37m"

[<Literal>]
let red = "\u001b[31m"

[<Literal>]
let reset = "\u001b[0m"

let eol = Environment.NewLine

// ** Exec **

/// Execute 'dotnet' cli with 'args' and environment variables from 'envs'.
/// Print Fli context if 'isVerbose'.
/// Print cli output.
/// Exit with non-zero code if cli return error.
let inline dotnet (isVerbose : bool) (envs : (string * string) list) (args : string) =
    let result =
        cli {
            Exec "dotnet"
            Arguments args
            EnvironmentVariables envs
            Output (printf "%s")
        }
        |> (fun context ->
            if isVerbose then
                let envs =
                    context.config.EnvironmentVariables.Value
                    |> List.map (fun (key, value) ->
                        if "NUGET_API_KEY".Equals key then
                            (key, "<REDACTED>")
                        else
                            (key, value)
                    )

                printfn
                    $"%s{white}Fli context:%s{eol}\
                    Env = %A{envs}%s{eol}\
                    Command = \"dotnet %s{context.config.Arguments.Value}\"%s{eol}\
                    %s{reset}"

            context
        )
        |> Command.execute

    if result.ExitCode <> 0 then
        eprintfn $"%s{red}Command exited with code %d{result.ExitCode}.%s{reset}"

        if result.Error.IsSome then
            eprintfn
                $"%s{red}Error message:%s{eol}\
                %s{result.Error.Value}%s{reset}"
        else
            eprintfn ""

        exit result.ExitCode

/// Execute 'dotnet run' command on cicd project with 'args', environment variables
/// from 'envs', 'verbosity' option and '--no-build' flag if cicd project already built.
let inline run
    (verbosity : string)
    (rebuild : bool)
    (envs : (string * string) list)
    (args : string seq)
    =
    let isVerbose = dotnetVerbose.Equals verbosity

    let noBuild =
        if rebuild || not (File.Exists @".\cicd\bin\Debug\net6.0\cicd.exe") then
            String.Empty
        else
            "--no-build"

    let command = $"run %s{verbosity} %s{noBuild} --project ./cicd/cicd.fsproj"

    if (Seq.length args) > 0 then
        seq {
            yield command
            yield "--"
            yield! args
        }
        |> String.concat " "
    else
        command
    |> dotnet isVerbose envs

/// Execute 'dotnet run' command on cicd project with 'flag' silently.
let inline print (flag : string) =
    run dotnetSilent false [] [ flag ]
    exit 0

// ** Start of this script **

let cliArgs = fsi.CommandLineArgs[1..]

if cliArgs.Length = 0 then
    printfn $"%s{doc}"
    exit 0

let scriptArgs =
    cliArgs
    |> Array.findIndex "--".Equals
    |> (+) 1
    |> Array.skip
    <| cliArgs

let argsRaw = Docopt().Apply (doc, scriptArgs, exit = true)

if argsRaw[list].IsTrue then
    print list

if argsRaw[version].IsTrue then
    print version

let args = argsRaw |> Args

seq { $"-t %s{args.Target}" }
|> run args.DotnetVerbosity args.Rebuild args.Env
