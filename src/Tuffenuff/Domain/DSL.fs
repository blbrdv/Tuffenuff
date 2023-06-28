namespace Tuffenuff.Domain

open Tuffenuff.Domain.Entity

module DSL =
    let comment text =
        Simple { Name = "#" ; Value = text } |> Instruction

    let (!/) = comment

    let syntax value = !/(sprintf "syntax=%s" value)

    let escape value = !/(sprintf "escape=%c" value)
