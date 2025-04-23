[<AutoOpen>]
module Tuffenuff.DSL.Workdir

open Tuffenuff.Domain.Types

/// <summary>Sets the working directory for any <c>RUN</c>, <c>CMD</c>,
/// <c>ENTRYPOINT</c>, <c>COPY</c> and <c>ADD</c> instructions that follow it in the
/// Dockerfile.</summary>
let workdir value =
    SimpleQuoted { Name = "WORKDIR" ; Value = value } |> Instruction
