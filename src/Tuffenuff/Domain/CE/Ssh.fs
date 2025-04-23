namespace Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Common

[<Sealed>]
type SshBuilder() =
    member _.Zero () : MountParameters = MountParameters.Create Ssh

    member this.Yield _ = this.Zero ()

    /// Sets the ID of SSH agent socket or key. Defaults to "default".
    [<CustomOperation("id")>]
    member _.Id (state : MountParameters, value : string) =
        checkIfStringEmpty value "Id"
        { state with
            Params = state.Params.Add ("id", value)
        }

    /// Sets SSH agent socket path. Defaults to /run/buildkit/ssh_agent.${N}.
    [<CustomOperation("target")>]
    member _.Target (state : MountParameters, value : string) =
        checkIfStringEmpty value "Target (Dst, Destination)"
        { state with
            Params = state.Params.Add ("target", value)
        }

    /// Sets SSH agent socket path. Defaults to /run/buildkit/ssh_agent.${N}.
    [<CustomOperation("dst")>]
    member this.Dst (state : MountParameters, value : string) =
        this.Target (state, value)

    /// Sets SSH agent socket path. Defaults to /run/buildkit/ssh_agent.${N}.
    [<CustomOperation("destination")>]
    member this.Destination (state : MountParameters, value : string) =
        this.Target (state, value)

    /// Sets instruction to errors out when the key is unavailable.
    /// Defaults to false.
    [<CustomOperation("required")>]
    member _.Required (state : MountParameters, value : bool) =
        { state with
            Params = state.Params.Add ("required", value.ToString().ToLower ())
        }

    /// Sets file mode for socket in octal. Default 0600.
    [<CustomOperation("mode")>]
    member _.Mode (state : MountParameters, value : string) =
        checkIfStringEmpty value "Mode"
        { state with
            Params = state.Params.Add ("mode", value)
        }

    /// Sets user ID for socket. Default 0.
    [<CustomOperation("UID")>]
    member _.UID (state : MountParameters, value : int) =
        checkIfPositive value "UID"
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    /// Sets group ID for socket. Default 0.
    [<CustomOperation("GID")>]
    member _.GID (state : MountParameters, value : int) =
        checkIfPositive value "GID"
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member _.Combine (_, _) = ()

    member _.Delay (f : unit -> 'a) = f ()

    member _.Run (state : MountParameters) = state
