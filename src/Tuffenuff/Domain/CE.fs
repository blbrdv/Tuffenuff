module Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types

//---------------------------------------------------------------------------------------
// ADD
//---------------------------------------------------------------------------------------


type AddBuilder(image) =
    member _.Zero () = AddInstruction.Create image

    member this.Yield _ = this.Zero ()

    [<CustomOperation("chown")>]
    member _.Chown (state : AddInstruction, value : string) =
        { state with Chown = Some value }

    [<CustomOperation("chmod")>]
    member _.Chmod (state : AddInstruction, value : string) =
        { state with Chmod = Some value }

    [<CustomOperation("checksum")>]
    member _.Checksum (state : AddInstruction, value : string) =
        { state with Checksum = Some value }

    [<CustomOperation("keepGitDir")>]
    member _.KeepGitDir (state : AddInstruction) = { state with KeepGitDir = true }

    [<CustomOperation("link")>]
    member _.Link (state : AddInstruction) = { state with Link = true }

    member _.Combine (_, _) = ()

    member _.Delay f = f ()

    member _.Run state = state |> Add |> Instruction


//---------------------------------------------------------------------------------------
// COPY
//---------------------------------------------------------------------------------------


type CopyBuilder(image) =
    member _.Zero () = CopyInstruction.Create image

    member this.Yield _ = this.Zero ()

    [<CustomOperation("from'")>]
    member _.From (state : CopyInstruction, value : string) =
        { state with From = Some value }

    [<CustomOperation("chown")>]
    member _.Chown (state : CopyInstruction, value : string) =
        { state with Chown = Some value }

    [<CustomOperation("chmod")>]
    member _.Chmod (state : CopyInstruction, value : string) =
        { state with Chmod = Some value }

    [<CustomOperation("link")>]
    member _.Link (state : CopyInstruction) = { state with Link = true }

    member _.Combine (_, _) = ()

    member _.Delay f = f ()

    member _.Run state = state |> Copy |> Instruction


//---------------------------------------------------------------------------------------
// HEALTHCHECK
//---------------------------------------------------------------------------------------


type HealthcheckBuilder(cmds) =
    member _.Yield _ = HealthcheckInstruction.Create cmds

    [<CustomOperation("interval")>]
    member _.Interval (state : HealthcheckInstruction, value : string) =
        { state with
            Options = state.Options.Add ("interval", value)
        }

    [<CustomOperation("timeout")>]
    member _.Timeout (state : HealthcheckInstruction, value : string) =
        { state with
            Options = state.Options.Add ("timeout", value)
        }

    [<CustomOperation("period")>]
    member _.StartPeriod (state : HealthcheckInstruction, value : string) =
        { state with
            Options = state.Options.Add ("start-period", value)
        }

    [<CustomOperation("retries")>]
    member _.Retries (state : HealthcheckInstruction, value : int) =
        { state with
            Options = state.Options.Add ("retries", string value)
        }

    member _.Combine (_, _) = ()

    member _.Delay f = f ()

    member _.Run state = state |> Healthcheck |> Instruction
