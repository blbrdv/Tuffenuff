[<AutoOpen>]
module Tuffenuff.DSL.Add

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE

/// <summary>Copies files, directories, or remote file URLs from the source to the
/// destination in the container.</summary>
let addOpts args = AddBuilder args

/// <summary>Copies files, directories, or remote file URLs from the source to the
/// destination in the container.</summary>
let add args =
    args |> AddInstruction.Create |> Add |> Instruction
