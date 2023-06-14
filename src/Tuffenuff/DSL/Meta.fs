[<AutoOpen>]
module Tuffenuff.DSL.Meta

open System
open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types


let labels ps =
    KeyValueList { Name = "LABEL" ; Elements = ps } |> Instruction

let label key value =
    [ (key, value) ] |> Map.ofSeq |> Parameters |> labels

[<Obsolete("MAINTAINER instruction is deprecated, use LABEL instead.")>]
let maintainer name =
    Simple { Name = "MAINTAINER" ; Value = name } |> Instruction
