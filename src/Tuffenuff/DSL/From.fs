[<AutoOpen>]
module Tuffenuff.DSL.From

open Tuffenuff.Domain.CE


/// <summary>Specifies the base image to use for the Dockerfile.</summary>
let based image = FromBuilder image

/// <summary>Specifies the base image to use for the Dockerfile.</summary>
let stage image name = based image { as' name }

/// <summary>Specifies the base image to use for the Dockerfile.</summary>
let from image = based image { () }

/// <summary>Specifies the base image to use for the Dockerfile.</summary>
let fresh = based "scratch" { () }
