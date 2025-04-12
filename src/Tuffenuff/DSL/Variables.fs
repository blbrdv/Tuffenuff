[<AutoOpen>]
module Tuffenuff.DSL.Variables

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types

let variable (name : string) : string = $"${{%s{name}}}"

let (!~) (name : string) : string = name |> variable

let (!~-) (name : string) (value : string) : string = $"%s{name}:-%s{value}" |> variable

let (!~+) (name : string) (value : string) : string = $"%s{name}:+%s{value}" |> variable

let (!~?) (name : string) (pattern : string) : string =
    $"%s{name}#%s{pattern}" |> variable

let (!~??) (name : string) (pattern : string) : string =
    $"%s{name}##%s{pattern}" |> variable

let (!~%) (name : string) (pattern : string) : string =
    $"%s{name}%%%s{pattern}" |> variable

let (!~%%) (name : string) (pattern : string) : string =
    $"%s{name}%%%%%s{pattern}" |> variable
    
let (!~/) (name : string) (pattern : string) (replacement : string) : string =
    $"%s{name}/%s{pattern}/%s{replacement}" |> variable
    
let (!~//) (name : string) (pattern : string) (replacement : string) : string =
    $"%s{name}//%s{pattern}/%s{replacement}" |> variable
