#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.Domain.ImageCE

image {
    cmt "Simple Hello World dockerfile"
    from "alpine:3.18"
    
    cmd [| "echo" ; "'Hello world'" |]
}
|> Image.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.HelloWorld"))
