open CICD
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
    Targets.init ()

    Environment.environVars ()
    |> Trace.traceObjectWithName "Fake.Core.Environment.environVars"
    |> ignore

    Target.runOrDefaultWithArguments "Clean"

    0
