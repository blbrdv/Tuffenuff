#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"
#load "Partial.part.fsx"

open System.IO
open Tuffenuff
open Tuffenuff.Domain.ImageCE
open Partial.part

let part1 = echoMaessage ()

let part2 =
    Image.fromFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.part"))

image {
    cmt "Partial dockerfile"
    FROM "ubuntu:latest"
    ___
    incl part1
    ___
    incl part2
}
|> Image.render
|> Image.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.Partial"))
