module Tuffenuff.String

open System


let eol = Environment.NewLine


let eol_slash = sprintf " \\%s    " eol


let quote value =
    sprintf "%c%s%c" '"' (string value) '"'


let print name value = sprintf "%s %s" name value


let printKV key value = sprintf "%s=%s" key value


let printKVQ key value = printKV key (quote value)


let printList list =
    if (list :> obj) :? string[] then 
        list |> Seq.map quote |> String.concat ", " |> sprintf "[ %s ]"
    else
        list |> String.concat " "


let printFlag name value =
    if value then 
        sprintf " --%s" name 
    else 
        ""


let printParameter<'T> name (value : 'T option) =
    if value.IsSome then 
        sprintf " --%s=%s" name (string value.Value)
    else
        ""


let printParameterQ<'T> name (value : 'T option) =
    match value with
    | Some s -> s |> quote |> Some
    | _ -> None
    |> printParameter name


let trim (text : string) = text.TrimStart(eol.ToCharArray())


let variable = sprintf "${%s}"


let ( ~% ) = variable
