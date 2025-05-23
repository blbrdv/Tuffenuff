#r "nuget: Tuffenuff"

open System.IO
open Tuffenuff
open Tuffenuff.DSL
open Tuffenuff.Domain.Types

let (!+) (name : string) =
    Simple { Name = "INCLUDE" ; Value = name } |> Instruction

df [
    syntax "bergkvist/includeimage"
    !/"Custom syntax example"
    from "alpine:3.12"

    !+"python:3.8.3-alpine3.12"

    cmd [ "python" ]
]
|> Dockerfile.render
|> Dockerfile.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.CustomSyntax"))
