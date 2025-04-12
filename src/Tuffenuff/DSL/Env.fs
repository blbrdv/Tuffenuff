[<AutoOpen>]
module Tuffenuff.DSL.Env

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections

/// <summary>Sets environment variables in the container, which can be used by the
/// application running inside the container.</summary>
let envs ps =
    KeyValueList { Name = "ENV" ; Elements = ps } |> Instruction

/// <summary>Sets environment variables in the container, which can be used by the
/// application running inside the container.</summary>
let env key value =
    [ (key, value) ] |> Map.ofSeq |> Parameters |> envs
