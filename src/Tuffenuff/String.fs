module Tuffenuff.String

open System


let eol = Environment.NewLine


let eol_slash = $" \\%s{eol}    "


let quote value = $"%c{'"'}%s{string value}%c{'"'}"


let print name value = $"%s{name} %s{value}%s{eol}"


let printKV key value = $"%s{key}=%s{value}"


let printKVQ key value = printKV key (quote value)


let printList list =
    if (list :> obj) :? string[] then
        list |> Seq.map quote |> String.concat ", " |> sprintf "[ %s ]"
    else
        list |> String.concat " "


let printFlag name value =
    if value then $" --%s{name}" else ""


let printParameter<'T> name (value : 'T option) =
    if value.IsSome then
        $" --%s{name}=%s{string value.Value}"
    else
        String.Empty


let printParameterQ<'T> name (value : 'T option) =
    match value with
    | Some s -> s |> quote |> Some
    | _ -> None
    |> printParameter name


let trim (text : string) = text.TrimStart (eol.ToCharArray ())


let variable = sprintf "${%s}"


let (~%) = variable
