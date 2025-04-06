[<AutoOpen>]
module Tuffenuff.DSL.Comments

open Tuffenuff.Domain.Types


/// <summary>Comment for providing information about the Dockerfile or explaining the
/// purpose of individual instructions.</summary>
let comment text =
    Simple { Name = "#" ; Value = text } |> Instruction

/// <summary>Comment for providing information about the Dockerfile or explaining the
/// purpose of individual instructions.</summary>
let (!/) = comment

/// <summary>Sets the set of instructions and arguments used to create a Docker container
/// image</summary>
let syntax value = !/ $"syntax=%s{value}"

/// <summary>Sets the character used to escape characters in a Dockerfile. Default is
/// <c>\</c></summary>
let escape value = !/ $"escape=%c{value}"
