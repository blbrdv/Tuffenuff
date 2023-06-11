[<AutoOpen>]
module Tuffenuff.DSL.Healthcheck

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE


let healthcheck = HealthcheckBuilder ()

let hc commands = healthcheck { cmds commands }

let disableHealthcheck = Simple { Name = "HEALTHCHECK"; Value = "NONE" } |> Instruction

let healthcheckNone = disableHealthcheck

let disableHc = disableHealthcheck

let hcNone = disableHealthcheck
