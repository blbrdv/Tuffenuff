#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.DSL

df [
    !/ "Simple Hello World dockerfile"
    from "alpine:3.18"
    br
    cmd [| "echo" ; "'Hello world'" |]
]
|> Dockerfile.render
|> Dockerfile.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.HelloWorld"))
