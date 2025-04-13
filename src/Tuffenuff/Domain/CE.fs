module Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections

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


//---------------------------------------------------------------------------------------
// RUN
//---------------------------------------------------------------------------------------


type BindParametersBuilder(target) =
    member _.Zero () =
        MountParameters.Create (Bind, "target", target)

    member this.Yield _ = this.Zero ()

    [<CustomOperation("source")>]
    member _.Source (state, value) =
        { state with
            Params = state.Params.Add ("source", value)
        }

    [<CustomOperation("from")>]
    member _.From (state, value) =
        { state with
            Params = state.Params.Add ("from", value)
        }

    [<CustomOperation("rw")>]
    member _.RW (state, value : bool) =
        { state with
            Params = state.Params.Add ("rw", value.ToString().ToLower ())
        }

    member _.Combine (_, _) = ()

    member _.Delay f = f ()

    member _.Run state = state


type CacheParametersBuilder(target) =
    member _.Zero () =
        MountParameters.Create (Cache, "target", target)

    member this.Yield _ = this.Zero ()

    [<CustomOperation("id")>]
    member _.Id (state, value) =
        { state with
            Params = state.Params.Add ("id", value)
        }

    [<CustomOperation("ro")>]
    member _.RO (state, value : bool) =
        { state with
            Params = state.Params.Add ("ro", value.ToString().ToLower ())
        }

    [<CustomOperation("sharing")>]
    member _.Sharing (state, value : SharingType) =
        { state with
            Params = state.Params.Add ("sharing", value.ToString().ToLower ())
        }

    [<CustomOperation("source")>]
    member _.Source (state, value) =
        { state with
            Params = state.Params.Add ("source", value)
        }

    [<CustomOperation("from")>]
    member _.From (state, value) =
        { state with
            Params = state.Params.Add ("from", value)
        }

    [<CustomOperation("mode")>]
    member _.Mode (state, value) =
        { state with
            Params = state.Params.Add ("mode", value)
        }

    [<CustomOperation("UID")>]
    member _.UID (state, value : int) =
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    [<CustomOperation("GID")>]
    member _.GID (state, value : int) =
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member _.Combine (_, _) = ()

    member _.Delay f = f ()

    member _.Run state = state


type TmpfsParametersBuilder(target) =
    member _.Zero () =
        MountParameters.Create (Tmpfs, "target", target)

    member this.Yield _ = this.Zero ()

    [<CustomOperation("size")>]
    member _.Size (state, value : int) =
        { state with
            Params = state.Params.Add ("size", value.ToString ())
        }

    member _.Combine (_, _) = ()

    member _.Delay f = f ()

    member _.Run state = state


type SecretParametersBuilder() =
    member _.Zero () = MountParameters.Create Secret

    member this.Yield _ = this.Zero ()

    [<CustomOperation("id")>]
    member _.Id (state, value) =
        { state with
            Params = state.Params.Add ("id", value)
        }

    [<CustomOperation("target")>]
    member _.Target (state, value) =
        { state with
            Params = state.Params.Add ("target", value)
        }

    [<CustomOperation("required")>]
    member _.Required (state, value : bool) =
        { state with
            Params = state.Params.Add ("required", value.ToString().ToLower ())
        }

    [<CustomOperation("mode")>]
    member _.Mode (state, value) =
        { state with
            Params = state.Params.Add ("mode", value)
        }

    [<CustomOperation("UID")>]
    member _.UID (state, value : int) =
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    [<CustomOperation("GID")>]
    member _.GID (state, value : int) =
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member _.Combine (_, _) = ()

    member _.Delay f = f ()

    member _.Run state = state


type SshParametersBuilder() =
    member _.Zero () = MountParameters.Create Ssh

    member this.Yield _ = this.Zero ()

    [<CustomOperation("id")>]
    member _.Id (state, value) =
        { state with
            Params = state.Params.Add ("id", value)
        }

    [<CustomOperation("target")>]
    member _.Target (state, value) =
        { state with
            Params = state.Params.Add ("target", value)
        }

    [<CustomOperation("required")>]
    member _.Required (state, value : bool) =
        { state with
            Params = state.Params.Add ("required", value.ToString().ToLower ())
        }

    [<CustomOperation("mode")>]
    member _.Mode (state, value) =
        { state with
            Params = state.Params.Add ("mode", value)
        }

    [<CustomOperation("UID")>]
    member _.UID (state, value : int) =
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    [<CustomOperation("GID")>]
    member _.GID (state, value : int) =
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member _.Combine (_, _) = ()

    member _.Delay f = f ()

    member _.Run state = state


type RunInstructionBuilder() =

    member _.Yield _ = RunInstruction.Create ()

    /// <summary>Sets command to run.</summary>
    [<CustomOperation "cmd">]
    member _.Command (state : RunInstruction, cmd : string) =
        { state with
            Arguments = state.Arguments.Add cmd
        }

    /// <summary>Sets commands to run.</summary>
    [<CustomOperation "cmds">]
    member _.Arguments (state : RunInstruction, cmds : string seq) =
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

    member _.Run state = state |> Run |> Instruction
