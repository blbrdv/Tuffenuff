[<AutoOpen>]
module Tuffenuff.DSL.Comments

open Tuffenuff.Domain.Types


let comment text =
    Simple { Name = "#" ; Value = text } |> Instruction

let (!/) = comment

let syntax value = !/(sprintf "syntax=%s" value)

let escape value = !/(sprintf "escape=%c" value)
