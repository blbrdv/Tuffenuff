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

    Trace.traceLine ()
    Trace.trace "Passed environment variables:"
    Environment.environVars ()
    |> List.iter (fun (key, value) -> Trace.trace $"  %s{key}=\"%s{value}\"")
    Trace.traceLine ()
    Trace.trace ""

    Target.runOrList ()

    0
