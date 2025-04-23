[<AutoOpen>]
module Tuffenuff.DSL.Label

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
