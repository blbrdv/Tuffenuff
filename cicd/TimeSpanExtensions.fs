module CICD.TimeSpanExtensions

open System
open System.Runtime.CompilerServices

let private add (value : int) (suffix : string) (prev : string option) =
    match prev with
    | None ->
        if value = 0 then
            None
        else
            value.ToString($"00'%s{suffix}'") |> Some
    | Some v ->
        value.ToString($"00'%s{suffix}'")
        |> sprintf "%s %s" v
        |> Some

[<Extension>]
type TimeSpanExtensions =

    [<Extension>]
    static member ToReadableString (this : TimeSpan) : string =
        let rest =
            if this.Days = 0 then
                None
            else
                this.Days.ToString("##'d'") |> Some
            |> add this.Hours "h"
            |> add this.Minutes "m"
            |> add this.Seconds "s"

        let ms = this.Milliseconds.ToString("000'ms'")

        match rest with
        | None -> ms
        | Some value -> $"%s{value} %s{ms}"
