[<AutoOpen>]
module Tuffenuff.DSL.Variables

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types


/// <summary>Sets environment variables in the container, which can be used by the 
/// application running inside the container.</summary>
let envs ps =
    KeyValueList { Name = "ENV" ; Elements = ps } |> Instruction

/// <summary>Sets environment variables in the container, which can be used by the 
/// application running inside the container.</summary>
let env key value =
    [ (key, value) ] |> Map.ofSeq |> Parameters |> envs

/// <summary>Defines a build-time variable.</summary>
let args ps =
    KeyValueList { Name = "ARG" ; Elements = ps } |> Instruction

/// <summary>Defines a build-time variable.</summary>
let arg key value =
    [ (key, value) ] |> Map.ofSeq |> Parameters |> args

/// <summary>Defines a build-time variable.</summary>
let usearg key =
    Simple { Name = "ARG" ; Value = key } |> Instruction
