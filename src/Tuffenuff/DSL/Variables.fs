[<AutoOpen>]
module Tuffenuff.DSL.Variables

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types


let envs ps = KeyValueList { Name = "ENV"; Elements = ps } |> Instruction

let env key value = [ (key, value) ] |> Map.ofSeq |> Parameters |> envs

let args ps = KeyValueList { Name = "ARG"; Elements = ps } |> Instruction

let arg key value = [ (key, value) ] |> Map.ofSeq |> Parameters |> args

let usearg key = Simple { Name = "ARG"; Value = key } |> Instruction
