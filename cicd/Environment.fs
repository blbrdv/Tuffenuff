[<AutoOpen>]
module CICD.Environment

open Fake.Core

let isVerbose = Environment.environVarAsBoolOrDefault "FAKE_FORCE_VERBOSITY" false

let nugetApiKey =
    let env = Environment.environVarOrNone "NUGET_API_KEY"
    Option.iter (TraceSecrets.register "<REDACTED>") env
    env
