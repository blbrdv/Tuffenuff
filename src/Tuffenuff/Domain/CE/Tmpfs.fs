namespace Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Common

[<Sealed>]
type TmpfsBuilder(target) =
    member _.Zero () : MountParameters =
        MountParameters.Create (Tmpfs, "target", target)

    member this.Yield _ = this.Zero ()

    /// Sets mount path.
    [<CustomOperation("target")>]
    member _.Target (state : MountParameters, value : string) =
        checkIfStringEmpty value "Target (Dst, Destination)"
        { state with
            Params = state.Params.Add ("target", value)
        }

    /// Sets mount path.
    [<CustomOperation("dst")>]
    member this.Dst (state : MountParameters, value : string) =
        this.Target (state, value)

    /// Sets mount path.
    [<CustomOperation("destination")>]
    member this.Destination (state : MountParameters, value : string) =
        this.Target (state, value)

    /// Specify an upper limit on the size of the filesystem.
    [<CustomOperation("size")>]
    member _.Size (state : MountParameters, value : int) =
        checkIfPositive value "Size"
        { state with
            Params = state.Params.Add ("size", value.ToString ())
        }

    member _.Combine (_, _) = ()

    member _.Delay (f : unit -> 'a) = f ()

    member _.Run (state : MountParameters) = state
