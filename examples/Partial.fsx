#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"
#load "Partial.part.fsx"

open System.IO
open Tuffenuff
open Tuffenuff.DSL
open Partial.part

let part1 = echoMaessage ()

let part2 =
    Dockerfile.fromFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.part"))

df [
    !/ "Partial dockerfile"
    from "ubuntu:latest"
    !&part1
    !&part2
]
|> Dockerfile.render
|> Dockerfile.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.Partial"))
