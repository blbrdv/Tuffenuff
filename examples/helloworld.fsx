#r @"..\src\Tuffenuff\bin\Release\netstandard2.0\Tuffenuff.dll"

open System.IO
open Tuffenuff

df [
    fresh
    cp [] "hello" "/"
    cmd [ "/hello" ]
]
|> render
|> toFile (Path.Combine(__SOURCE_DIRECTORY__, "Dockerfile.helloworld"))
