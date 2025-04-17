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
    Target.runOrList ()

    0
