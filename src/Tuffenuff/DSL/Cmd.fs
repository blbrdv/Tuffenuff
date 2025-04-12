[<AutoOpen>]
module Tuffenuff.DSL.Cmd

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types

/// <summary>Specifies the default command to be executed when a container is launched.
/// </summary>
let cmd elements =
    List
        {
            Name = "CMD"
            Elements = Arguments elements
        }
    |> Instruction
    
