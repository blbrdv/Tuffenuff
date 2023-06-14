[<AutoOpen>]
module Tuffenuff.DSL.FileSystem

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE


let user value =
    Simple { Name = "USER" ; Value = value } |> Instruction

let usr = user

let workdir value =
    SimpleQuoted { Name = "WORKDIR" ; Value = value } |> Instruction

let addOpts args = AddBuilder (args)

let add args =
    args |> AddInstruction.Create |> Add |> Instruction

let (!@) src dst =
    [ src ; dst ] |> AddInstruction.Create |> Add |> Instruction

let copyOpts args = CopyBuilder (args)

let copy args =
    args |> CopyInstruction.Create |> Copy |> Instruction

let cp src dst =
    [ src ; dst ] |> CopyInstruction.Create |> Copy |> Instruction

let volume value =
    SimpleQuoted { Name = "VOLUME" ; Value = value } |> Instruction

let volumes elements =
    List
        {
            Name = "VOLUME"
            Elements = elements
        }
    |> Instruction

let vol = volume
