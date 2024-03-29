module Tuffenuff.Domain.CE

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections


//---------------------------------------------------------------------------------------
// FROM
//---------------------------------------------------------------------------------------


type FromBuilder(image) =
    member __.Zero () = FromInstruction.Create (image)

    member this.Yield (_) = this.Zero ()

    [<CustomOperation("as'")>]
    member __.Alias (state : FromInstruction, value : string) =
        { state with Name = Some value }

    [<CustomOperation("platform")>]
    member __.Platform (state : FromInstruction, value : string) =
        { state with Platform = Some value }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state |> From |> Instruction


//---------------------------------------------------------------------------------------
// ADD
//---------------------------------------------------------------------------------------


type AddBuilder(image) =
    member __.Zero () = AddInstruction.Create (image)

    member this.Yield (_) = this.Zero ()

    [<CustomOperation("chown")>]
    member __.Chown (state : AddInstruction, value : string) =
        { state with Chown = Some value }

    [<CustomOperation("chmod")>]
    member __.Chmod (state : AddInstruction, value : string) =
        { state with Chmod = Some value }

    [<CustomOperation("checksum")>]
    member __.Checksum (state : AddInstruction, value : string) =
        { state with Checksum = Some value }

    [<CustomOperation("keepGitDir")>]
    member __.KeepGitDir (state : AddInstruction) = { state with KeepGitDir = true }

    [<CustomOperation("link")>]
    member __.Link (state : AddInstruction) = { state with Link = true }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state |> Add |> Instruction


//---------------------------------------------------------------------------------------
// COPY
//---------------------------------------------------------------------------------------


type CopyBuilder(image) =
    member __.Zero () = CopyInstruction.Create (image)

    member this.Yield (_) = this.Zero ()

    [<CustomOperation("from'")>]
    member __.From (state : CopyInstruction, value : string) =
        { state with From = Some value }

    [<CustomOperation("chown")>]
    member __.Chown (state : CopyInstruction, value : string) =
        { state with Chown = Some value }

    [<CustomOperation("chmod")>]
    member __.Chmod (state : CopyInstruction, value : string) =
        { state with Chmod = Some value }

    [<CustomOperation("link")>]
    member __.Link (state : CopyInstruction) = { state with Link = true }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state |> Copy |> Instruction


//---------------------------------------------------------------------------------------
// HEALTHCHECK
//---------------------------------------------------------------------------------------


type HealthcheckBuilder(cmds) =
    member __.Yield (_) = HealthcheckInstruction.Create (cmds)

    [<CustomOperation("interval")>]
    member __.Interval (state : HealthcheckInstruction, value : string) =
        { state with
            Options = state.Options.Add ("interval", value)
        }

    [<CustomOperation("timeout")>]
    member __.Timeout (state : HealthcheckInstruction, value : string) =
        { state with
            Options = state.Options.Add ("timeout", value)
        }

    [<CustomOperation("period")>]
    member __.StartPeriod (state : HealthcheckInstruction, value : string) =
        { state with
            Options = state.Options.Add ("start-period", value)
        }

    [<CustomOperation("retries")>]
    member __.Retries (state : HealthcheckInstruction, value : int) =
        { state with
            Options = state.Options.Add ("retries", string value)
        }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state |> Healthcheck |> Instruction


//---------------------------------------------------------------------------------------
// RUN
//---------------------------------------------------------------------------------------


type BindParametersBuilder(target) =
    member __.Zero () =
        MountParameters.Create (Bind, "target", target)

    member this.Yield (_) = this.Zero ()

    [<CustomOperation("source")>]
    member __.Source (state, value) =
        { state with
            Params = state.Params.Add ("source", value)
        }

    [<CustomOperation("from")>]
    member __.From (state, value) =
        { state with
            Params = state.Params.Add ("from", value)
        }

    [<CustomOperation("rw")>]
    member __.RW (state, value : bool) =
        { state with
            Params = state.Params.Add ("rw", value.ToString().ToLower ())
        }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state


type CacheParametersBuilder(target) =
    member __.Zero () =
        MountParameters.Create (Cache, "target", target)

    member this.Yield (_) = this.Zero ()

    [<CustomOperation("id")>]
    member __.Id (state, value) =
        { state with
            Params = state.Params.Add ("id", value)
        }

    [<CustomOperation("ro")>]
    member __.RO (state, value : bool) =
        { state with
            Params = state.Params.Add ("ro", value.ToString().ToLower ())
        }

    [<CustomOperation("sharing")>]
    member __.Sharing (state, value : SharingType) =
        { state with
            Params = state.Params.Add ("sharing", value.ToString().ToLower ())
        }

    [<CustomOperation("source")>]
    member __.Source (state, value) =
        { state with
            Params = state.Params.Add ("source", value)
        }

    [<CustomOperation("from")>]
    member __.From (state, value) =
        { state with
            Params = state.Params.Add ("from", value)
        }

    [<CustomOperation("mode")>]
    member __.Mode (state, value) =
        { state with
            Params = state.Params.Add ("mode", value)
        }

    [<CustomOperation("UID")>]
    member __.UID (state, value : int) =
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    [<CustomOperation("GID")>]
    member __.GID (state, value : int) =
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state


type TmpfsParametersBuilder(target) =
    member __.Zero () =
        MountParameters.Create (Tmpfs, "target", target)

    member this.Yield (_) = this.Zero ()

    [<CustomOperation("size")>]
    member __.Size (state, value : int) =
        { state with
            Params = state.Params.Add ("size", value.ToString ())
        }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state


type SecretParametersBuilder() =
    member __.Zero () = MountParameters.Create (Secret)

    member this.Yield (_) = this.Zero ()

    [<CustomOperation("id")>]
    member __.Id (state, value) =
        { state with
            Params = state.Params.Add ("id", value)
        }

    [<CustomOperation("target")>]
    member __.Target (state, value) =
        { state with
            Params = state.Params.Add ("target", value)
        }

    [<CustomOperation("required")>]
    member __.Required (state, value : bool) =
        { state with
            Params = state.Params.Add ("required", value.ToString().ToLower ())
        }

    [<CustomOperation("mode")>]
    member __.Mode (state, value) =
        { state with
            Params = state.Params.Add ("mode", value)
        }

    [<CustomOperation("UID")>]
    member __.UID (state, value : int) =
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    [<CustomOperation("GID")>]
    member __.GID (state, value : int) =
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state


type SshParametersBuilder() =
    member __.Zero () = MountParameters.Create (Ssh)

    member this.Yield (_) = this.Zero ()

    [<CustomOperation("id")>]
    member __.Id (state, value) =
        { state with
            Params = state.Params.Add ("id", value)
        }

    [<CustomOperation("target")>]
    member __.Target (state, value) =
        { state with
            Params = state.Params.Add ("target", value)
        }

    [<CustomOperation("required")>]
    member __.Required (state, value : bool) =
        { state with
            Params = state.Params.Add ("required", value.ToString().ToLower ())
        }

    [<CustomOperation("mode")>]
    member __.Mode (state, value) =
        { state with
            Params = state.Params.Add ("mode", value)
        }

    [<CustomOperation("UID")>]
    member __.UID (state, value : int) =
        { state with
            Params = state.Params.Add ("UID", value.ToString ())
        }

    [<CustomOperation("GID")>]
    member __.GID (state, value : int) =
        { state with
            Params = state.Params.Add ("GID", value.ToString ())
        }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f ()

    member __.Run (state) = state


type RunInstructionBuilder() =

    member __.Yield _ = RunInstruction.Create ()

    /// <summary>Sets command to run.</summary>
    [<CustomOperation "cmd">]
    member __.Command (state : RunInstruction, cmd : string) =
        { state with
            Arguments = state.Arguments.Add (cmd)
        }

    /// <summary>Sets commands to run.</summary>
    [<CustomOperation "cmds">]
    member __.Arguments (state : RunInstruction, cmds : string seq) =
        { state with
            Arguments = state.Arguments.Append (Arguments cmds)
        }

    /// <summary>Sets mount for the container.</summary>
    [<CustomOperation "mount">]
    member __.Mount (state : RunInstruction, mount : MountParameters) =
        { state with
            Mounts = state.Mounts.Add (mount)
        }

    /// <summary>Specify the network mode for the container.</summary>
    [<CustomOperation "network">]
    member __.Network (state : RunInstruction, network : NetworkType) =
        { state with Network = Some network }

    /// <summary>Specify the security mode for the container.</summary>
    [<CustomOperation "security">]
    member __.Security (state : RunInstruction, security : SecurityType) =
        { state with Security = Some security }

    member __.Run (state) = state |> Run |> Instruction
