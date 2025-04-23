module internal Tuffenuff.CE.Common

open System

let checkIfStringEmpty (value : string) (name : string) =
    if String.IsNullOrEmpty value then
        raise (ArgumentException $"%s{name} can not be null or empty")

let checkIfSeqEmpty (value : 'a seq) (name : string) =
    if Seq.isEmpty value then
        raise (ArgumentException $"%s{name} can not be empty")

let checkIfPositive (value : int) (name : string) =
    if value < 0 then
        raise (ArgumentException $"%s{name} can not be negative")
