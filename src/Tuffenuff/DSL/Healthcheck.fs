[<AutoOpen>]
module Tuffenuff.DSL.Healthcheck

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE


let healthcheck cmds = HealthcheckBuilder (cmds)

let hc commands =
    HealthcheckInstruction.Create (commands)

let disableHealthcheck =
    Simple
        {
            Name = "HEALTHCHECK"
            Value = "NONE"
        }
    |> Instruction

let healthcheckNone = disableHealthcheck

let disableHc = disableHealthcheck

let hcNone = disableHealthcheck
