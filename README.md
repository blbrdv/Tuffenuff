<img align="left" width="80" height="80" src="imgs/logo.png" alt="logo">

# Tuffenuff

[![Software License](https://img.shields.io/github/license/blbrdv/Tuffenuff?style=flat-square)](LICENSE)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/blbrdv/Tuffenuff/release.yaml?style=flat-square)](https://github.com/blbrdv/Tuffenuff/actions?query=branch%3Arelease)
[![Nuget version](https://img.shields.io/nuget/v/Tuffenuff?style=flat-square)](https://www.nuget.org/packages/Tuffenuff/)

> The goal of this project is to make it simple to generate dockerfiles using a F# DSL by offering complete syntax support and the ability to create dockerfile from multiple parts.

## Usage

Simple "Hello, World!":

```f#
#r "nuget: Tuffenuff"

open System.IO
open Tuffenuff

df [
    fresh
    cp [] "hello" "/"
    cmd [ "/hello" ]
]
|> render
|> toFile (Path.Combine(__SOURCE_DIRECTORY__, "Dockerfile"))
```

will create `Dockerfile` with following content:

```Dockerfile
FROM scratch
COPY hello /
CMD /hello
```

For more examples see:
- [Examples](examples/)
- [Tests](tests/Tuffenuff.Tests/)
- Docs (not yet)
