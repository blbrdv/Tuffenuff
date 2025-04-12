[<AutoOpen>]
module Tuffenuff.DSL.Stopsignal

open Tuffenuff.Domain.Types

/// <summary>Sets the system call signal that will be sent to the container to stop
/// it gracefully.</summary>
let stopsignal value =
    Simple { Name = "STOPSIGNAL" ; Value = value } |> Instruction

/// <summary>Sets the system call signal that will be sent to the container to stop
/// it gracefully.</summary>
let (!!) = stopsignal
