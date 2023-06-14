# Tuffenuff

Simple F# DSL for generating dockerfiles

## Usage

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

For more see [examples](https://github.com/blbrdv/Tuffenuff/tree/main/examples):

## Documentation

**WIP**

## Release notes

See [release page](https://github.com/blbrdv/Tuffenuff/releases).

## License

MIT License
