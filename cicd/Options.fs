module CICD.Options

open Fake.DotNet

// https://github.com/fsprojects/FAKE/issues/2744#issuecomment-2325173747
/// If trace level is not verbose - disable any output for MSBuild.
let inline private setLogParams (args : MSBuild.CliArguments) =
    if isVerbose then
        { args with
            DisableInternalBinLog = true
        }
    else
        { args with
            NoConsoleLogger = true
            Loggers = None
            BinaryLoggers = None
            DistributedLoggers = None
            FileLoggers = None
            DisableInternalBinLog = true 
        }

/// Default 'dotnet test' command options.
let inline testOptions (filter : string option) =
    (fun (opt : DotNet.TestOptions) ->
        Trace.traceObject
            { opt with
                Logger = Some "console;verbosity=detailed"
                MSBuildParams = setLogParams opt.MSBuildParams
                NoLogo = true
                Filter = filter
            }
    )

/// Default 'dotnet fantomas' command options.
/// Using this instead of 'dotnetOptions' because fantomas silently fails when verbosity
/// is Quiet or Minimal. 🤡🤡🤡
let fantomasOptions =
    (fun (opt : DotNet.Options) ->
        Trace.traceObject
            { opt with
                Verbosity = Some DotNet.Verbosity.Normal
            }
    )

/// Default 'dotnet build' command options.
let buildOptions =
    (fun (opt : DotNet.BuildOptions) ->
        Trace.traceObject
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

/// Default MSBuild cli options.
let MSBuildOptions =
    (fun (opt : DotNet.MSBuildOptions) ->
        Trace.traceObject
            { opt with
                MSBuildParams = setLogParams opt.MSBuildParams
            }
    )
