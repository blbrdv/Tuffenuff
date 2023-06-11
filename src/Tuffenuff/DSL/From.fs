[<AutoOpen>]
module Tuffenuff.DSL.From

open Tuffenuff.Domain.CE


let fromOpts image = FromBuilder (image)

let stage image name = fromOpts image { alias name }

let from image = fromOpts image {()}

let fresh = fromOpts "scratch" {()}
