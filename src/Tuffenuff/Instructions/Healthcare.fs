[<AutoOpen>]
module Tuffenuff.Instructions.Healthcare

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types


let interval = Interval

let timout = Timeout

let startPeriod = StartPeriod

let start = StartPeriod

let retries = Retries

let healthchech ps commands = 
    let interval = 
        ps
        |> Seq.tryFindBack (function
                        | Interval _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Interval s -> Some s
                        | _ -> None)
        |> Option.flatten
    let timeout = 
        ps
        |> Seq.tryFindBack (function
                        | Timeout _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Timeout s -> Some s
                        | _ -> None)
        |> Option.flatten
    let startPeriod = 
        ps
        |> Seq.tryFindBack (function
                        | StartPeriod _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | StartPeriod s -> Some s
                        | _ -> None)
        |> Option.flatten
    let retries = 
        ps
        |> Seq.tryFindBack (function
                        | Retries _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Retries s -> Some s
                        | _ -> None)
        |> Option.flatten
    Healthcheck { 
        Interval = interval; 
        Timeout = timeout; 
        StartPeriod = startPeriod; 
        Retries = retries; 
        Instructions = Arguments commands
    }
    |> Instruction

let hc = healthchech

let disableHealthcheck = Simple { Name = "HEALTHCHECK"; Value = "NONE" } |> Instruction

let healthchechNone = disableHealthcheck

let disableHc = disableHealthcheck

let hcNone = disableHealthcheck
