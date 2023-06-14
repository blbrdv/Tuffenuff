#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.DSL
open Tuffenuff.Domain.Types

let (!+) (name : string) =
    Simple { Name = "INCLUDE" ; Value = name } |> Instruction

dockerfile
    [
        syntax "bergkvist/includeimage"
        !/ "Custom syntax example"
        from "alpine:3.12"
        br
        !+ "python:3.8.3-alpine3.12"
        br
        cmd [ "python" ]
    ]
|> Dockerfile.render
|> Dockerfile.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.CustomSyntax"))
