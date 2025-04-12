[<AutoOpen>]
module Tuffenuff.DSL.Entrypoint

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types

/// <summary>Specifies the command to be executed when a container is started.</summary>
let entrypoint elements =
    List
        {
            Name = "ENTRYPOINT"
            Elements = Arguments elements
        }
    |> Instruction

/// <summary>Specifies the command to be executed when a container is started.</summary>
let entry = entrypoint
