<img align="left" width="80" height="76" src="imgs/logo.png" alt="logo">

# Tuffenuff

[![Software License](https://img.shields.io/github/license/blbrdv/Tuffenuff?style=flat-square)](LICENSE)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/blbrdv/Tuffenuff/release.yaml?style=flat-square)](https://github.com/blbrdv/Tuffenuff/actions?query=branch%3Arelease)
[![Nuget version](https://img.shields.io/nuget/v/Tuffenuff?style=flat-square)](https://www.nuget.org/packages/Tuffenuff/)

Simple F# DSL for generating dockerfiles.

### Why?

1. To work with more flexing scripts and ability to reuse Dockerfile code.
2. Because I can.

## Usage

### Hello, World!

```f#
#r "nuget: Tuffenuff"

open System.IO
open Tuffenuff

df [
    !/ "Simple Hello World dockerfile"
    from "alpine:3.18"
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

### More complex example

#### foo.fsx

```f#
#r "nuget: Tuffenuff"

open Tuffenuff
open Tuffenuff.DSL

let echoMaessage () =
    df [
        !/ "this is from 'foo.fsx'"
        !> """echo 'echo "Shalom!"' > /etc/profile.d/welcome.sh"""
    ]
```

#### Dockerfile.part

```Dockerfile
# this is from 'Dockerfile.part'
ENTRYPOINT [ "/bin/bash", "-l" ]
```

#### bar.fsx

```f#
#r "nuget: Tuffenuff"
#load "foo.fsx"

open System.IO
open Tuffenuff
open Tuffenuff.DSL
open foo

let part1 = echoMaessage ()

let part2 =
    Dockerfile.fromFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.part"))

df [
    !/ "Partial dockerfile"
    from "ubuntu:latest"
    !&part1
    !&part2
]
|> Dockerfile.render
|> Dockerfile.toFile (Path.Combine (__SOURCE_DIRECTORY__, "Dockerfile.Partial"))

```

will create `Dockerfile` with following content:

```Dockerfile
# Partial dockerfile
FROM ubuntu:latest

# this is from 'foo.fsx'
RUN \
    echo 'echo "Shalom!"' > /etc/profile.d/welcome.sh

# this is from 'Dockerfile.part'
ENTRYPOINT [ "/bin/bash", "-l" ]
```

### More examples

See [examples](examples/).

## Local development

### Pre-requirements

1. Install DotNet SDK version [6.0.x](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Run `dotnet tool restore`

### Use automation script

Run `.\run.cmd` (Windows OS) or `./run.sh` (Unix-based OS) script:

```
Automation script.

Usage:
    script <TARGET> [-s|-n|-v] [--rebuild] [-e <ENV> ...]
    script (-h | --help)
    script --list
    script --version

Options:
  -h --help     Print this.
  --list        Print list of available targets.
  --version     Print module version.
  -s            Silent trace level.
  -n            Normal trace level.
  -v            Verbose trace level.
  --rebuild     Force building CICD project.
  -e            Sets environment variables.
```

#### The following targets are available:

 - `AllTests` - Run "UnitTests" then "IntegrationTests"
 - `Build` - Build Tuffenuff
 - `CheckFormat` - Check if code files need formatting
 - `Clean` - Delete "bin" and "obj" directories
 - `Format` - Format code files
 - `GenerateAllSyntaxVersions` - Run "GenerateSyntaxVersions" then "GenerateUpstreamSyntaxVersions"
 - `GenerateSyntaxVersions` - Generates module with syntax versions from "dockerfile" repository
 - `GenerateUpstreamSyntaxVersions` - Generates module with syntax versions from "dockerfile-upstream" repository
 - `IntegrationTests` - Tests scripts in examples directory
 - `Release` - Push library to Nuget
 - `UnitTests` - Tests DSL
