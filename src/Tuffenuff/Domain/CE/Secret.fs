namespace Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Common

[<Sealed>]
type SecretBuilder() =
    member _.Zero () : MountParameters = MountParameters.Create Secret

    member this.Yield _ = this.Zero ()

    /// Sets ID of the secret. Defaults to basename of the target path.
    [<CustomOperation("id")>]
    member _.Id (state : MountParameters, value : string) =
        checkIfStringEmpty value "Id"
        { state with
            Params = state.Params.Add ("id", value)
        }

    /// Mount the secret to the specified path. Defaults to /run/secrets/ + id if unset
    /// and if env is also unset.
    [<CustomOperation("target")>]
    member _.Target (state : MountParameters, value : string) =
        checkIfStringEmpty value "Target (Dst, Destination)"
        { state with
            Params = state.Params.Add ("target", value)
        }

    /// Mount the secret to the specified path. Defaults to /run/secrets/ + id if unset
    /// and if env is also unset.
    [<CustomOperation("dst")>]
    member this.Dst (state : MountParameters, value : string) =
        this.Target(state, value)

    /// Mount the secret to the specified path. Defaults to /run/secrets/ + id if unset
    /// and if env is also unset.
    [<CustomOperation("destination")>]
    member this.Destination (state : MountParameters, value : string) =
        this.Target(state, value)

    /// Sets mount the secret to an environment variable instead of a file, or both.
    [<CustomOperation("env")>]
    member _.Env (state : MountParameters, value : string) =
        checkIfStringEmpty value "Env"
        state

    /// Sets instruction to errors out when the secret is unavailable.
    /// Defaults to false.
    [<CustomOperation("required")>]
    member _.Required (state : MountParameters, value : bool) =
        { state with
            Params = state.Params.Add ("required", value.ToString().ToLower ())
        }

    /// Sets file mode for secret file in octal. Default 0400.
    [<CustomOperation("mode")>]
    member _.Mode (state : MountParameters, value : string) =
        checkIfStringEmpty value "Mode"
        { state with
            Params = state.Params.Add ("mode", value)
        }

    /// Sets user ID for secret file. Default 0.
    [<CustomOperation("UID")>]
    member _.UID (state : MountParameters, value : int) =
        checkIfPositive value "UID"
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    /// Sets group ID for secret file. Default 0.
    [<CustomOperation("GID")>]
    member _.GID (state : MountParameters, value : int) =
        checkIfPositive value "GID"
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member _.Combine (_, _) = ()

    member _.Delay (f : unit -> 'a) = f ()

    member _.Run (state : MountParameters) = state
