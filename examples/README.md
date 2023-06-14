# Examples

## How to run

1. Build project with `dotnet fake run build.fsx -t Release`
2. Execute example `dotnet fsi <filename>.fsx`

This will create `Dockerfile.<filename>` file in same directory.

> Take note: files with `part` postfix not intended to be executed.

## Description

### `HelloWorld.fsx`

Simple Hello World dockerfile.

### `CustomSyntax.fsx`

Example using `INCLUDE` custom syntax.

See [bergkvist/includeimage](https://github.com/bergkvist/includeimage) 
for more info.

### `Partial.fsx`

Example that combine parts from other files:
- `Partial.part.fsx`
- `Dockerfile.part`

### `ElixirApp.fsx`

Simple 'Hello World' Elixir app.
