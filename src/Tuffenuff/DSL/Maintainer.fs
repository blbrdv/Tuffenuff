[<AutoOpen>]
module Tuffenuff.DSL.Maintainer

open System
open Tuffenuff.Domain.Types

/// <summary>Sets the name and email address of the person who maintains the Dockerfile.
/// </summary>
[<Obsolete("MAINTAINER instruction is deprecated, use LABEL instead.")>]
let maintainer name =
    Simple { Name = "MAINTAINER" ; Value = name } |> Instruction
