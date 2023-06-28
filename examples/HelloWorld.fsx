#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.Domain.ImageCE

image {
    cmt "Simple Hello World dockerfile"
    FROM "alpine:3.18"
    ___
    CMD [| "echo" ; "'Hello world'" |]
}
|> Image.render
|> Image.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.HelloWorld"))
