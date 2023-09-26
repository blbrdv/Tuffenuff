#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.Domain.ImageCE
open Tuffenuff.Domain.Image
open Tuffenuff.Domain.Entity

type ImageBuilder with
    [<CustomOperation("include")>]
    member __.CustomInclude(context : Image, name : string) =
        { Name = "INCLUDE" ; Value = name } 
        |> Simple
        |> Instruction
        |> context.Add


image {
    syntax "bergkvist/includeimage"

    cmt "Custom syntax example"
    from "alpine:3.12"
    
    include "python:3.8.3-alpine3.12"
    
    cmd [ "python" ]
}
|> Image.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.CustomSyntax"))
