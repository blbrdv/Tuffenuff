[<AutoOpen>]
module Tuffenuff.DSL.Healthcheck

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE


/// <summary>Define a command that Docker will run to check the health of a containe.
/// </summary>
let healthcheck cmds = HealthcheckBuilder (cmds)

/// <summary>Define a command that Docker will run to check the health of a containe.
/// </summary>
let hc commands =
    HealthcheckInstruction.Create (commands)

/// <summary>Disable any health check inherited from the base image.</summary>
let disableHealthcheck =
    Simple
        {
            Name = "HEALTHCHECK"
            Value = "NONE"
        }
    |> Instruction

/// <summary>Disable any health check inherited from the base image.</summary>
let healthcheckNone = disableHealthcheck

/// <summary>Disable any health check inherited from the base image.</summary>
let disableHc = disableHealthcheck

/// <summary>Disable any health check inherited from the base image.</summary>
let hcNone = disableHealthcheck
