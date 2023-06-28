#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.Domain.ImageCE
open Tuffenuff.Domain.Image
open Tuffenuff.Domain.Entity

type IImageContext<'self> with
    [<CustomOperation("INCLUDE")>]
    member __.CustomInclude(context : IImageContext<'a>, name : string) =
        { Name = "INCLUDE" ; Value = name } 
        |> Simple
        |> Instruction
        |> context.Add
    

image {
    syntax "bergkvist/includeimage"
    cmt "Custom syntax example"
    FROM "alpine:3.12"
    ___
    INCLUDE "python:3.8.3-alpine3.12"
    ___
    CMD [ "python" ]
}
|> Image.render
|> Image.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.CustomSyntax"))
