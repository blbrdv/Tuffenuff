[<AutoOpen>]
module internal Tests.Utils

open Tuffenuff
open Tuffenuff.Domain.Types

let render (entity : Entity) : string =
    [ entity ]
    |> df
    |> Dockerfile.render
