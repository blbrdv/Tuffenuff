module CICD.Options

open Fake.DotNet

/// If trace level is not verbose - disable any output for MSBuild.
let inline private setLogParams (args : MSBuild.CliArguments) =
    if isVerbose then
        args
    else
        { args with
            NoConsoleLogger = true
            Loggers = None
            BinaryLoggers = None
            DistributedLoggers = None
            FileLoggers = None
        }

/// Default dotnet cli options.
let dotnetOptions =
    (fun (opt : DotNet.Options) ->
        //traceOptions "DotNet options"
        { opt with
            Verbosity =
                if isVerbose then
                    Some DotNet.Verbosity.Detailed
                else
                    Some DotNet.Verbosity.Quiet
        }
    )

/// Default 'dotnet fantomas' command options.
/// Using this instead of 'dotnetOptions' because fantomas silently fails when verbosity
/// is Quiet or Minimal. 🤡🤡🤡
let fantomasOptions =
    (fun (opt : DotNet.Options) ->
        let newOpts = dotnetOptions opt
        { newOpts with
            Verbosity =
                if isVerbose then
                    newOpts.Verbosity
                else
                    Some DotNet.Verbosity.Normal
        }
    )

/// Default 'dotnet build' command options.
let buildOptions =
    (fun (opt : DotNet.BuildOptions) ->
        { opt with
            Common = dotnetOptions opt.Common
            MSBuildParams = setLogParams opt.MSBuildParams
        }
    )

/// Default 'dotnet test' command options.
let testOptions =
    (fun (opt : DotNet.TestOptions) ->
        //traceOptions "Test options"
        { opt with
            Logger =
                if isVerbose then
                    Some "console;verbosity=detailed"
                else
                    None
            MSBuildParams = setLogParams opt.MSBuildParams
        }
    )

/// Default MSBuild cli options.
let MSBuildOptions =
    (fun (opt : DotNet.MSBuildOptions) ->
        //traceOptions "Build options"
        { opt with
            MSBuildParams = setLogParams opt.MSBuildParams
        }
    )
