# Tuffenuff

Simple F# DSL for generating dockerfiles

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

For more see [examples](https://github.com/blbrdv/Tuffenuff/tree/main/examples):

## Documentation

**WIP**

## Release notes

See [release page](https://github.com/blbrdv/Tuffenuff/releases).

## License

MIT License
