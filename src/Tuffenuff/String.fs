namespace Tuffenuff

module internal String =
    open System

    [<Literal>]
    let empty = ""

    [<Literal>]
    let ws = " "

    [<Literal>]
    let tab = "    "

    [<Literal>]
    let q = "\""

    let eol = Environment.NewLine

    let eol_slash = $"%s{ws}\\%s{eol}%s{tab}"

    let quote (value : 'a) : string = $"%s{q}%s{string value}%s{q}"

    let print (name : string) (value : string) : string =
        $"%s{name}%s{ws}%s{value}%s{eol}"

    let printKV (key : string) (value : string) : string = $"%s{key}=%s{value}"

    let printKVQ (key : string) (value : 'a) : string = value |> quote |> printKV key

    // ReSharper disable once FSharpRedundantParens
    let printList (values : #(string seq)) : string =
        if (values :> obj) :? string[] then
            values |> Seq.map quote |> String.concat ", " |> sprintf "[ %s ]"
        else
            values |> String.concat ws

    let printFlag (name : string) (value : bool) : string =
        if value then $"%s{ws}--%s{name}" else empty

    let printParameter<'T> (name : string) (value : 'T option) : string =
        if value.IsSome then
            $"%s{ws}--%s{name}=%s{string value.Value}"
        else
            empty

    let printParameterQ<'T> (name : string) (value : 'T option) : string =
        match value with
        | Some s -> s |> quote |> Some
        | _ -> None
        |> printParameter name

    let trim (value : string) : string = value.TrimStart [| ' ' ; '\n' ; '\r' |]

    let variable (name : string) : string = $"${{%s{name}}}"

    let (~%) (name : string) : string = variable name
