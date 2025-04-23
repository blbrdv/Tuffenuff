module CICD.TargetParameterExtensions

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open Fake.Core

let inline private whereAt (args : string list) (flag : string) : int option =
    try
        args
        |> List.findIndex flag.Equals
        |> Some
    with
        | :? KeyNotFoundException -> None
        | :? ArgumentException -> None

[<Extension>]
type TargetParameterExtensions =

    [<Extension>]
    static member GetParameter (this : TargetParameter, flag : string) : string option =
        let args = this.Context.Arguments

        match whereAt args flag with
        | Some index ->
            List.item (index + 1) args
            |> Some
        | None -> None

    [<Extension>]
    static member HasFlag (this : TargetParameter, flag : string) : bool =
        whereAt this.Context.Arguments flag
        |> Option.isSome
