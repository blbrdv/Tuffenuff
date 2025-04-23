[<AutoOpen>]
module Tuffenuff.DSL.Shell

open Tuffenuff.Domain.Types

/// <summary>Sets the default shell used for the RUN, CMD, and ENTRYPOINT instructions.
/// </summary>
let shell elements =
    List { Name = "SHELL" ; Elements = elements } |> Instruction
