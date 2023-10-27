[<AutoOpen>]
module Tuffenuff.DSL.Meta

open System
open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types


/// <summary>Adds metadata to an image, such as version, description, and maintainer.
/// </summary>
let labels ps =
    KeyValueList { Name = "LABEL" ; Elements = ps } |> Instruction

/// <summary>Adds metadata to an image, such as version, description, and maintainer.
/// </summary>
let label key value =
    [ (key, value) ] |> Map.ofSeq |> Parameters |> labels

/// <summary>Sets the name and email address of the person who maintains the Dockerfile.
/// </summary>
[<Obsolete("MAINTAINER instruction is deprecated, use LABEL instead.")>]
let maintainer name =
    Simple { Name = "MAINTAINER" ; Value = name } |> Instruction
