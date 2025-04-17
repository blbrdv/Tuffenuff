open CICD.Listener
open Fake.Core

[<EntryPoint>]
let main argv =
    argv
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "cicd"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext

    CoreTracing.setTraceListeners [ MinimalListener () ]
    CICD.Targets.init ()

    Trace.log "Passed environment variables:"
    Environment.environVars ()
    |> List.iter (fun (key, value) -> Trace.log $"  %s{key}=\"%s{value}\"")
    Trace.log ""

    Target.runOrList ()

    0
