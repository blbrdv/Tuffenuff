[<AutoOpen>]
module Tuffenuff.DSL.Volume

open Tuffenuff.Domain.Types

/// <summary>Creates a mount point with the specified name and marks it as holding
/// externally mounted volumes from native host or other containers.</summary>
let volume value =
    SimpleQuoted { Name = "VOLUME" ; Value = value } |> Instruction

/// <summary>Creates a mount point with the specified name and marks it as holding
/// externally mounted volumes from native host or other containers.</summary>
let volumes elements =
    List
        {
            Name = "VOLUME"
            Elements = elements
        }
    |> Instruction

/// <summary>Creates a mount point with the specified name and marks it as holding
/// externally mounted volumes from native host or other containers.</summary>
let vol = volume
