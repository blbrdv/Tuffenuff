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
    from "ubuntu:latest"
    
    incl part1
    
    incl part2
}
|> Image.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.Partial"))
