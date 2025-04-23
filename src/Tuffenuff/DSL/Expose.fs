[<AutoOpen>]
module Tuffenuff.DSL.Expose

open Tuffenuff.Domain.Types

/// <summary>Informs Docker that the container will listen on the specified network ports
/// at runtime.</summary>
let exp value =
    Simple { Name = "EXPOSE" ; Value = value } |> Instruction

/// <summary>Informs Docker that the container will listen on the specified network ports
/// at runtime.</summary>
let expose elements =
    List
        {
            Name = "EXPOSE"
            Elements = elements
        }
    |> Instruction
