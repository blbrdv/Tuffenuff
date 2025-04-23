namespace Tuffenuff.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections
open Tuffenuff.CE.Common

[<Sealed>]
type RunBuilder() =
    member _.Yield _ = RunInstruction.Create ()

    /// <summary>Sets command to run.</summary>
    [<CustomOperation "cmd">]
    member _.Command (state : RunInstruction, cmd : string) =
        checkIfStringEmpty cmd "Command"
        { state with
            Arguments = state.Arguments.Add cmd
        }

    /// <summary>Sets commands to run.</summary>
    [<CustomOperation "cmds">]
    member _.Commands (state : RunInstruction, cmds : string seq) =
        checkIfSeqEmpty cmds "Commands"
        { state with
            Arguments = state.Arguments.Append (Arguments cmds)
        }

    /// <summary>Sets mount for the container.</summary>
    [<CustomOperation "mount">]
    member _.Mount (state : RunInstruction, mount : MountParameters) =
        { state with
            Mounts = state.Mounts.Add mount
        }

    /// <summary>Specify the network mode for the container.</summary>
    [<CustomOperation "network">]
    member _.Network (state : RunInstruction, network : NetworkType) =
        { state with Network = Some network }

    /// <summary>Specify the security mode for the container.</summary>
    [<CustomOperation "security">]
    member _.Security (state : RunInstruction, security : SecurityType) =
        { state with Security = Some security }

    member _.Run (state : RunInstruction) : Entity = state |> Run |> Instruction
