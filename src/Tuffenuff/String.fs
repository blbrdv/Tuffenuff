module Tuffenuff.String

open System


let internal eol = Environment.NewLine


let internal eol_slash = sprintf " \\%s    " eol


let internal quote value = sprintf "%c%s%c" '"' (string value) '"'


let internal print name value = sprintf "%s %s%s" name value eol


let internal printKV key value = sprintf "%s=%s" key value


let internal printKVQ key value = printKV key (quote value)


let internal printList list =
    if (list :> obj) :? string[] then
        list |> Seq.map quote |> String.concat ", " |> sprintf "[ %s ]"
    else
        list |> String.concat " "


let internal printFlag name value =
    if value then sprintf " --%s" name else ""


let internal printParameter<'T> name (value : 'T option) =
    if value.IsSome then
        sprintf " --%s=%s" name (string value.Value)
    else
        ""


let internal printParameterQ<'T> name (value : 'T option) =
    match value with
    | Some s -> s |> quote |> Some
    | _ -> None
    |> printParameter name


let internal trim (text : string) = text.TrimStart (eol.ToCharArray ())


let internal variable = sprintf "${%s}"


let internal (~%) = variable
