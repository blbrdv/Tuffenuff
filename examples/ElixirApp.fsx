#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.Domain.ImageCE

image {
    cmt "Hello World Elexir dockerfile"
    stage "bitnami/git:latest" "repo"
    workdir "/"
    run ["git clone https://github.com/rjNemo/docker_examples.git"]

    from "elixir:1.14-alpine"
    workdir "/app"
    copy [ "/docker_examples/elixir/hello.exs" ; "." ] 
    
    from "repo"
    cmd [| "elixir" ; "hello.exs" |]
}
|> Image.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.ElixirApp"))
