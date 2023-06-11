[<AutoOpen>]
module Tuffenuff.DSL.Healthcheck

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Healthcheck


let options = OptionsBuilder ()

let healthcheck options commands = 
    Healthcheck { 
        Options = options; 
        Instructions = Arguments commands
    }
    |> Instruction

let hc commands = healthcheck (options {()}) commands

let disableHealthcheck = Simple { Name = "HEALTHCHECK"; Value = "NONE" } |> Instruction

let healthcheckNone = disableHealthcheck

let disableHc = disableHealthcheck

let hcNone = disableHealthcheck
