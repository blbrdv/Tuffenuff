<img align="left" width="80" height="76" src="imgs/logo.png" alt="logo">

# Tuffenuff

[![Software License](https://img.shields.io/github/license/blbrdv/Tuffenuff?style=flat-square)](LICENSE)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/blbrdv/Tuffenuff/release.yaml?style=flat-square)](https://github.com/blbrdv/Tuffenuff/actions?query=branch%3Arelease)
[![Nuget version](https://img.shields.io/nuget/v/Tuffenuff?style=flat-square)](https://www.nuget.org/packages/Tuffenuff/)

> The goal of this project is to make it simple to generate dockerfiles using a F# DSL by offering complete syntax support and the ability to create dockerfile from multiple parts.

## Usage

⚠️ **WARNING**: project in the early stages of development, the API can change a lot!

Simple "Hello, World!":

```f#
#r "nuget: Tuffenuff"

open System.IO
open Tuffenuff

df [
    !/ "Simple Hello World dockerfile"
    from "alpine:3.18"
    br
    cmd [| "echo" ; "'Hello world'" |]
]
|> render
|> toFile (Path.Combine(__SOURCE_DIRECTORY__, "Dockerfile"))
```

will create `Dockerfile` with following content:

```Dockerfile
# Simple Hello World dockerfile
FROM alpine:3.18

CMD [ "echo", "'Hello world'" ]
```

For more see [examples](examples/).

## Local development

1. Install DotNet SDK version [6.0.x](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Run `dotnet tool restore`
3. Run project tasks `dotnet fake run build.fsx -t <Target>`, where `<Target>` is:
    - `CodestyleCheck`
    - `CodestyleFormat`
    - `Clean`
    - `Build`
    - `RunTests`

See `build.fsx` source code for more info.

### Scripts

`Scripts` directory contains useful fsx files for code generation.
