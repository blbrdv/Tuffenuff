[<AutoOpen>]
module Tuffenuff.DSL.Copy

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE

/// <summary>Copies files, directories, or remote file URLs from the source to the
/// destination in the container.</summary>
let (!@) src dst =
    [ src ; dst ] |> AddInstruction.Create |> Add |> Instruction

/// <summary>Copies files or directories from the source to the destination in the
/// container.</summary>
let copyOpts args = CopyBuilder args

/// <summary>Copies files or directories from the source to the destination in the
/// container.</summary>
let copy args =
    args |> CopyInstruction.Create |> Copy |> Instruction

/// <summary>Copies files or directories from the source to the destination in the
/// container.</summary>
let cp src dst =
    [ src ; dst ] |> CopyInstruction.Create |> Copy |> Instruction
