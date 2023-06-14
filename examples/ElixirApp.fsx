#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open System.IO
open Tuffenuff
open Tuffenuff.DSL

df [
    !/ "Hello World Elexir dockerfile"
    
    stage "bitnami/git:latest" "repo"
    workdir "/"
    !> "git clone https://github.com/rjNemo/docker_examples.git"

    br
    from "elixir:1.14-alpine"
    workdir "/app"
    copy [ from_ "repo" ] [ 
        "/docker_examples/elixir/hello.exs"
        "." 
    ]
    cmd [| "elixir"; "hello.exs" |]
]
|> Dockerfile.render
|> Dockerfile.toFile (Path.Combine(__SOURCE_DIRECTORY__, "Dockerfile.ElixirApp"))



