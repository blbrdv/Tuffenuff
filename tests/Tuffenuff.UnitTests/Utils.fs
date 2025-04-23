[<AutoOpen>]
module internal Tests.Utils

open System
open Tuffenuff
open Tuffenuff.Domain.Types

let private eol = Environment.NewLine

let toMultiline (list : string list) : string =
    let text = list |> String.concat eol
    $"%s{text}%s{eol}"

let toErrorMessage (list : string list) : string =
    let message = list |> String.concat eol
    $"%s{message}%s{eol}%s{eol}"

let toArgsLine (list : string list) : string =
    list |> String.concat " "

/// Convert single DSL entity to its string representation.
let render (entity : Entity) : string = [ entity ] |> df |> Dockerfile.render
