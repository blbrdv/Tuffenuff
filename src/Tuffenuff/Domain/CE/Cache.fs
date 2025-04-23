namespace Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Common

[<Sealed>]
type CacheBuilder (target : string) =
    member _.Zero () : MountParameters =
        checkIfStringEmpty target "Mouth"
        MountParameters.Create (Cache, "target", target)

    member this.Yield _ = this.Zero ()

    /// Optional ID to identify separate/different caches. Defaults to value of target.
    [<CustomOperation("id")>]
    member _.Id (state : MountParameters, value) =
        checkIfStringEmpty value "Id"
        { state with
            Params = state.Params.Add ("id", value)
        }

    /// Sets read-only mode.
    [<CustomOperation("ro")>]
    member _.RO (state : MountParameters, value : bool) =
        { state with
            Params = state.Params.Add ("ro", value.ToString().ToLower ())
        }

    /// Sets read-only mode.
    [<CustomOperation("readonly")>]
    member this.Readonly (state : MountParameters, value : bool) =
        this.RO (state, value)

    /// One of shared, private, or locked. Defaults to shared. A shared cache mount can
    /// be used concurrently by multiple writers. private creates a new mount if there
    /// are multiple writers. locked pauses the second writer until the first one
    /// releases the mount.
    [<CustomOperation("sharing")>]
    member _.Sharing (state : MountParameters, value : SharingType) =
        { state with
            Params = state.Params.Add ("sharing", value.ToString().ToLower ())
        }

    /// Sets build stage, context, or image name to use as a base of the cache mount.
    /// Defaults to empty directory.
    [<CustomOperation("from")>]
    member _.From (state : MountParameters, value : string) =
        checkIfStringEmpty value "From"
        { state with
            Params = state.Params.Add ("from", value)
        }

    /// Sets sub-path in the from to mount. Defaults to the root of the from.
    [<CustomOperation("source")>]
    member _.Source (state : MountParameters, value : string) =
        checkIfStringEmpty value "Source"
        { state with
            Params = state.Params.Add ("source", value)
        }

    /// Sets file mode for new cache directory in octal. Default 0755.
    [<CustomOperation("mode")>]
    member _.Mode (state : MountParameters, value : string) =
        checkIfStringEmpty value "Mode"
        { state with
            Params = state.Params.Add ("mode", value)
        }

    /// Sets user ID for new cache directory. Default 0.
    [<CustomOperation("UID")>]
    member _.UID (state : MountParameters, value : int) =
        checkIfPositive value "UID"
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    /// Sets group ID for new cache directory. Default 0.
    [<CustomOperation("GID")>]
    member _.GID (state : MountParameters, value : int) =
        checkIfPositive value "GID"
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member _.Combine (_, _) = ()

    member _.Delay (f : unit -> 'a) = f ()

    member _.Run (state : MountParameters) = state
