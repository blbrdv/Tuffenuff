[<AutoOpen>]
module Tuffenuff.Instructions.Configuration

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types


let cmd elements = List { Name = "CMD"; Elements = Arguments elements } |> Instruction

let exp value = Simple { Name = "EXPOSE"; Value = value } |> Instruction

let expose elements = List { Name = "EXPOSE"; Elements = elements } |> Instruction

let entrypoint elements = List { Name = "ENTRYPOINT"; Elements = Arguments elements } |> Instruction

let entry = entrypoint

let stopsignal value = Simple { Name = "STOPSIGNAL"; Value = value } |> Instruction

let ( !! ) = stopsignal

let shell elements = List { Name = "SHELL"; Elements = elements } |> Instruction

let onbuild entity = Onbuild { Instruction = entity } |> Instruction

let ( ~~ ) = onbuild
