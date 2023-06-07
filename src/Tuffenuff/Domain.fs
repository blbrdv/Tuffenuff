module Tuffenuff.Domain

open Tuffenuff.Collections


//---------------------------------------------------------------------------------------
// Base types
//---------------------------------------------------------------------------------------


type SimpleInstruction = { Name : string; Value : string }    


type ListInstruction = { Name : string; Elements : Arguments }


type KVInstruction = { Name : string; Key : string; Value : string }


type KVListInstruction = { Name : string; Elements : Parameters }


type FromParameter =
    | As of string
    | Platform of string


type FromInstruction = { Image : string; Name : string option; Platform : string option }


type InsertParameter =
    | Source of string
    | Chown of string
    | Chmod of string
    | Checksum of string
    | KeepGitDir
    | Link


type AddInstruction = { 
    Chown : string option; 
    Chmod : string option; 
    Checksum : string option;
    KeepGitDir : bool;
    Link : bool;
    Elements : Arguments
}


type CopyInstruction = { 
    From : string option
    Chown : string option; 
    Chmod : string option;
    Link : bool;
    Elements : Arguments
}


type HealthcheckParameter =
    | Interval of string 
    | Timeout of string
    | StartPeriod of string
    | Retries of int


type HealthcheckInstruction = { 
    Interval : string option; 
    Timeout : string option;
    StartPeriod : string option;
    Retries : int option;
    Instructions : Arguments
}


//---------------------------------------------------------------------------------------
// RUN types
//---------------------------------------------------------------------------------------


type SharingType =
    | Shared
    | Private
    | Locked


type MountType = 
    | Bind
    | Cache
    | Tmpfs
    | Secret
    | Ssh


type MountParameters = 
    {
        Name : MountType;
        Params : Parameters
    }
    with
        static member Create(name, key, value) =
            { Name = name; Params = Dict [ key, value ] }
        static member Create(name) =
            { Name = name; Params = Dict.empty }


type NetworkType =
    | DefaultNetwork
    | NoneNetwok
    | Host
    

type SecurityType =
    | Insecure
    | Sandbox
    

type RunInstruction =
    {
        Mounts : Collection<MountParameters>
        Network : NetworkType option
        Security : SecurityType option
        Arguments : Arguments
    }
    with
        static member Create() =
            {
                Mounts = Collection.empty;
                Network = None;
                Security = None;
                Arguments = Collection.empty;
            }


//---------------------------------------------------------------------------------------
// Computation Expressions
//---------------------------------------------------------------------------------------


type BindParametersBuilder (target) =
    member __.Zero() = MountParameters.Create(Bind, "target", target)

    member this.Yield (_) = this.Zero()

    [<CustomOperation("source")>]
    member __.Source (state, value) = 
        { state with Params = state.Params.Add("source", value) }

    [<CustomOperation("from")>]
    member __.From (state, value) = 
        { state with Params = state.Params.Add("from", value) }

    [<CustomOperation("rw")>]
    member __.RW (state, value : bool) = 
        { state with Params = state.Params.Add("rw", value.ToString().ToLower()) }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (state) = state


type CacheParametersBuilder (target) =
    member __.Zero() = MountParameters.Create(Cache, "target", target)

    member this.Yield (_) = this.Zero()

    [<CustomOperation("id")>]
    member __.Id (state, value) = 
        { state with Params = state.Params.Add("id", value) }

    [<CustomOperation("ro")>]
    member __.RO (state, value : bool) = 
        { state with Params = state.Params.Add("ro", value.ToString().ToLower()) }

    [<CustomOperation("sharing")>]
    member __.Sharing (state, value : SharingType) = 
        { state with Params = state.Params.Add("sharing", value.ToString().ToLower()) }

    [<CustomOperation("source")>]
    member __.Source (state, value) =
        { state with Params = state.Params.Add("source", value) }

    [<CustomOperation("from")>]
    member __.From (state, value) =
        { state with Params = state.Params.Add("from", value) }

    [<CustomOperation("mode")>]
    member __.Mode (state, value) =
        { state with Params = state.Params.Add("mode", value) }

    [<CustomOperation("UID")>]
    member __.UID (state, value : int) = 
        { state with Params = state.Params.Add("UID", value.ToString()) }

    [<CustomOperation("GID")>]
    member __.GID (state, value : int) = 
        { state with Params = state.Params.Add("GID", value.ToString()) }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (state) = state


type TmpfsParametersBuilder (target) =
    member __.Zero() = MountParameters.Create(Tmpfs, "target", target)

    member this.Yield (_) = this.Zero()

    [<CustomOperation("size")>]
    member __.Size (state, value : int) =
        { state with Params = state.Params.Add("size", value.ToString()) }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (state) = state


type SecretParametersBuilder () =
    member __.Zero() = MountParameters.Create(Secret)

    member this.Yield (_) = this.Zero()

    [<CustomOperation("id")>]
    member __.Id (state, value) =
        { state with Params = state.Params.Add("id", value) }

    [<CustomOperation("target")>]
    member __.Target (state, value) =
        { state with Params = state.Params.Add("target", value) }

    [<CustomOperation("required")>]
    member __.Required (state, value : bool) =
        { state with Params = state.Params.Add("required", value.ToString().ToLower()) }

    [<CustomOperation("mode")>]
    member __.Mode (state, value) =
        { state with Params = state.Params.Add("mode", value) }

    [<CustomOperation("UID")>]
    member __.UID (state, value : int) =
        { state with Params = state.Params.Add("UID", value.ToString()) }

    [<CustomOperation("GID")>]
    member __.GID (state, value : int) =
        { state with Params = state.Params.Add("GID", value.ToString()) }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (state) = state


type SshParametersBuilder () =
    member __.Zero() = MountParameters.Create(Ssh)

    member this.Yield (_) = this.Zero()

    [<CustomOperation("id")>]
    member __.Id (state, value) =
        { state with Params = state.Params.Add("id", value) }

    [<CustomOperation("target")>]
    member __.Target (state, value) =
        { state with Params = state.Params.Add("target", value) }

    [<CustomOperation("required")>]
    member __.Required (state, value : bool) =
        { state with Params = state.Params.Add("required", value.ToString().ToLower()) }

    [<CustomOperation("mode")>]
    member __.Mode (state, value) =
        { state with Params = state.Params.Add("mode", value) }

    [<CustomOperation("UID")>]
    member __.UID (state, value : int) =
        { state with Params = state.Params.Add("UID", value.ToString()) }

    [<CustomOperation("GID")>]
    member __.GID (state, value : int) =
        { state with Params = state.Params.Add("GID", value.ToString()) }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (state) = state


//---------------------------------------------------------------------------------------
// Entity types
//---------------------------------------------------------------------------------------


type InstructionType =
    | Simple of SimpleInstruction
    | SimpleQuoted of SimpleInstruction
    | List of ListInstruction
    | KeyValue of KVInstruction
    | KeyValueList of KVListInstruction
    | From of FromInstruction
    | Run of RunInstruction
    | Add of AddInstruction
    | Copy of CopyInstruction
    | Onbuild of OnbuildInstruction
    | Healthcheck of HealthcheckInstruction


and OnbuildInstruction = { Instruction : Entity }


and Entity = 
    | Plain of string
    | Instruction of InstructionType
    | Subpart of Entity seq


type RunInstructionBuilder () =

    member __.Yield _ = RunInstruction.Create()

    [<CustomOperation "cmd">]
    member __.Command (state : RunInstruction, cmd : string) =
        { state with Arguments = state.Arguments.Add(cmd) }

    [<CustomOperation "cmds">]
    member __.Arguments (state : RunInstruction, cmds : string seq) =
        { state with Arguments = state.Arguments.Append(Arguments cmds) }

    [<CustomOperation "mount">]
    member __.Mount (state : RunInstruction, mount : MountParameters) =
        { state with Mounts = state.Mounts.Add(mount) }

    [<CustomOperation "network">]
    member __.Network (state : RunInstruction, network : NetworkType) =
        { state with Network = Some network }

    [<CustomOperation "security">]
    member __.Security (state : RunInstruction, security : SecurityType) =
        { state with Security = Some security }

    member __.Run (state) = state |> Run |> Instruction

let bindParams target = BindParametersBuilder (target)

let bind target = bindParams target {()}

let cacheParams target = CacheParametersBuilder (target)

let cache target = cacheParams target {()}

let tmpfsParams target = TmpfsParametersBuilder (target)

let tmpfs target = tmpfsParams target {()}

let secret = SecretParametersBuilder ()

let ssh = SshParametersBuilder ()

let run = RunInstructionBuilder ()

let ( !> ) command = run { cmd command }
