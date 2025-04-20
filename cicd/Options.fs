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

/// Default 'dotnet fantomas' command options.
/// Using this instead of 'dotnetOptions' because fantomas silently fails when verbosity
/// is Quiet or Minimal. 🤡🤡🤡
let fantomasOptions =
    (fun (opt : DotNet.Options) ->
        Trace.logObject
            { opt with
                Verbosity = Some DotNet.Verbosity.Normal
            }
    )

/// Default 'dotnet build' command options.
let buildOptions =
    (fun (opt : DotNet.BuildOptions) ->
        Trace.logObject
            { opt with
                Common =
                    { opt.Common with
                        Verbosity =
                            if isVerbose then
                                Some DotNet.Verbosity.Normal
                            else
                                Some DotNet.Verbosity.Quiet
                    }
                MSBuildParams = setLogParams opt.MSBuildParams
                NoLogo = true
            }
    )

/// Default 'dotnet test' command options.
let testOptions =
    (fun (opt : DotNet.TestOptions) ->
        Trace.logObject
            { opt with
                Logger =
                    if isVerbose then
                        Some "console;verbosity=detailed"
                    else
                        None
                MSBuildParams = setLogParams opt.MSBuildParams
                NoLogo = true
                NoRestore = true 
            }
    )

/// Default MSBuild cli options.
let MSBuildOptions =
    (fun (opt : DotNet.MSBuildOptions) ->
        Trace.logObject
            { opt with
                MSBuildParams = setLogParams opt.MSBuildParams
            }
    )
