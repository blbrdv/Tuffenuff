[<AutoOpen>]
module Tuffenuff.DSL.From

open Tuffenuff.Domain.CE


let based image = FromBuilder (image)

let stage image name = based image { as' name }

let from image = based image {()}

let fresh = based "scratch" {()}
