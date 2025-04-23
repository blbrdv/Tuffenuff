namespace Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Common

[<Sealed>]
type FromBuilder (reference : string) =
    member _.Zero () : FromInstruction =
        checkIfStringEmpty reference "Image reference"
        FromInstruction.Create reference

    member this.Yield _ = this.Zero ()

    /// <summary>Specify the name of the build stage.</summary>
    [<CustomOperation("as'")>]
    member _.Alias (state : FromInstruction, alias : string) =
        checkIfStringEmpty alias "Alias"
        { state with Name = Some alias }

    /// <summary>Specify the platform of the image.</summary>
    [<CustomOperation("platform")>]
    member _.Platform (state : FromInstruction, platform : string) =
        checkIfStringEmpty platform "Platform"
        { state with Platform = Some platform }

    member _.Combine (_, _) = ()

    member _.Delay (f : unit -> 'a) = f ()

    member _.Run (state : FromInstruction) : Entity = state |> From |> Instruction
