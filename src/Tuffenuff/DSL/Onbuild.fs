[<AutoOpen>]
module Tuffenuff.DSL.Onbuild

open Tuffenuff.Domain.Types

/// <summary>Adds a trigger instruction to the image that will be executed when the image
/// is used as a base for another image.</summary>
let onbuild entity =
    Onbuild { Instruction = entity } |> Instruction

/// <summary>Adds a trigger instruction to the image that will be executed when the image
/// is used as a base for another image.</summary>
let (~~) = onbuild
