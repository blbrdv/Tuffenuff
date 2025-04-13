module internal Tuffenuff.Domain.Common

open System

let checkIfEmpty (value : string) (name : string) =
    if String.IsNullOrEmpty value then
        raise (ArgumentException $"%s{name} can not be null or empty")
