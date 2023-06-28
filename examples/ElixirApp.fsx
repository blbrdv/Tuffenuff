#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.Domain.ImageCE

image {
    cmt "Hello World Elexir dockerfile"

    FROM "bitnami/git:latest" AS "repo"
    WORKDIR "/"
    RUN ["git clone https://github.com/rjNemo/docker_examples.git"]

    ___
    FROM "elixir:1.14-alpine"
    WORKDIR "/app"
    COPY [ "/docker_examples/elixir/hello.exs" ; "." ] 
    FROM "repo"
    CMD [| "elixir" ; "hello.exs" |]
}
|> Image.render
|> Image.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.ElixirApp"))
