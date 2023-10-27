[<AutoOpen>]
module Tuffenuff.DSL.Comments

open Tuffenuff.Domain.Types


let comment text =
    Simple { Name = "#" ; Value = text } |> Instruction

/// <summary>Comment for providing information about the Dockerfile or explaining the
/// purpose of individual instructions.</summary>
let (!/) = comment

/// <summary>Sets the set of instructions and arguments used to create a Docker container
/// image</summary>
let syntax value = !/(sprintf "syntax=%s" value)

/// <summary>Sets the character used to escape characters in a Dockerfile. Default is 
/// <c>\</c></summary>
let escape value = !/(sprintf "escape=%c" value)
