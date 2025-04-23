namespace Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Common

[<Sealed>]
type BindBuilder (target : string) =
    member _.Zero () : MountParameters =
        checkIfStringEmpty target "Target"
        MountParameters.Create (Bind, "target", target)

    member this.Yield _ = this.Zero ()

    /// Source path in the from. Defaults to the root of the from.
    [<CustomOperation("source")>]
    member _.Source (state : MountParameters, value : string) =
        checkIfStringEmpty target "Source"
        { state with
            Params = state.Params.Add ("source", value)
        }

    /// Build stage, context, or image name for the root of the source. Defaults to the
    /// build context.
    [<CustomOperation("from")>]
    member _.From (state : MountParameters, value : string) =
        checkIfStringEmpty target "From"
        { state with
            Params = state.Params.Add ("from", value)
        }

    /// Allow writes on the mount. Written data will be discarded.
    [<CustomOperation("rw")>]
    member _.RW (state : MountParameters, value : bool) =
        { state with
            Params = state.Params.Add ("rw", value.ToString().ToLower ())
        }

    /// Allow writes on the mount. Written data will be discarded.
    [<CustomOperation("readwrite")>]
    member this.Readwrite (state : MountParameters, value : bool) =
        this.RW (state, value)

    member _.Combine (_, _) = ()

    member _.Delay (f : unit -> 'a) = f ()

    member _.Run (state : MountParameters) = state
