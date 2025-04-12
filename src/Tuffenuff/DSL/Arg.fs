[<AutoOpen>]
module Tuffenuff.DSL.Arg

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections

/// <summary>Defines a build-time variable.</summary>
let args ps =
    KeyValueList { Name = "ARG" ; Elements = ps } |> Instruction

/// <summary>Defines a build-time variable.</summary>
let arg key value =
    [ (key, value) ] |> Map.ofSeq |> Parameters |> args

/// <summary>Defines a build-time variable.</summary>
let usearg key =
    Simple { Name = "ARG" ; Value = key } |> Instruction
