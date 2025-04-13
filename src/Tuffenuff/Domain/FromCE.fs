module Tuffenuff.Domain.FromCE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Common

[<Sealed>]
type FromBuilder(reference : string) =
    member _.Zero () : FromInstruction =
        checkIfEmpty reference "Image reference"
        FromInstruction.Create reference

    member this.Yield _ = this.Zero ()

    [<CustomOperation("as'")>]
    member _.Alias (state : FromInstruction, alias : string) =
        checkIfEmpty alias "Alias"
        { state with Name = Some alias }

    [<CustomOperation("platform")>]
    member _.Platform (state : FromInstruction, platform : string) =
        checkIfEmpty platform "Platform"
        { state with Platform = Some platform }

    member _.Combine (_, _) = ()

    member _.Delay (f : unit -> 'a) = f ()

    member _.Run (state : FromInstruction) : Entity = state |> From |> Instruction
