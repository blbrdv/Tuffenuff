module Tuffenuff.Domain

open System.Collections.Generic


//---------------------------------------------------------------------------------------
// Base types
//---------------------------------------------------------------------------------------


type List = IEnumerable<string>


type KVList = Map<string, string>


type SimpleInstruction = { Name : string; Value : string }    


type ListInstruction = { Name : string; Elements : List }


type KVInstruction = { Name : string; Key : string; Value : string }


type KVListInstruction = { Name : string; Elements : KVList }


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
    Elements : List 
}


type CopyInstruction = { 
    From : string option
    Chown : string option; 
    Chmod : string option;
    Link : bool;
    Elements : List 
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
    Instructions : List
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
        Params : Map<string, string>
    }
    with
        static member Create(name, list) =
            { Name = name; Params = Map.ofList(list) }
        static member Create(name) =
            { Name = name; Params = Map.empty }


type NetworkType =
    | Def    // Default
    | Absent // None
    | Host
    

type SecurityType =
    | Insecure
    | Sandbox
    

type RunInstruction =
    {
        Mounts : MountParameters seq
        Network : NetworkType option
        Security : SecurityType option
        Commands : List
    }
    with
        static member Create() =
            {
                Mounts = Seq.empty;
                Network = None;
                Security = None;
                Commands = Seq.empty;
            }


//---------------------------------------------------------------------------------------
// Computation Expressions
//---------------------------------------------------------------------------------------


type BindParametersBuilder (target) =
    let mutable _state = MountParameters.Create(Bind, [ "target", target ])

    member __.Yield (_) = ()

    [<CustomOperation("source")>]
    member __.Source (_, value) = 
        _state <- { _state with Params = _state.Params.Add("source", value) }

    [<CustomOperation("from")>]
    member __.From (_, value) = 
        _state <- { _state with Params = _state.Params.Add("from", value) }

    [<CustomOperation("rw")>]
    member __.RW (_, value : bool) = 
        _state <- { _state with Params = _state.Params.Add("rw", value.ToString().ToLower()) }

    member __.Zero() = _state

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (_) = _state


type CacheParametersBuilder (target) =
    let mutable _state = MountParameters.Create(Cache, [ "target", target ])

    member __.Yield (_) = ()

    [<CustomOperation("id")>]
    member __.Id (_, value) = 
        _state <- { _state with Params = _state.Params.Add("id", value) }

    [<CustomOperation("ro")>]
    member __.RO (_, value : bool) = 
        _state <- { _state with Params = _state.Params.Add("ro", value.ToString().ToLower()) }

    [<CustomOperation("sharing")>]
    member __.Sharing (_, value : SharingType) = 
        _state <- { _state with Params = _state.Params.Add("sharing", (nameof value).ToLower()) }

    [<CustomOperation("source")>]
    member __.Source (_, value) =
        _state <- { _state with Params = _state.Params.Add("source", value) }

    [<CustomOperation("from")>]
    member __.From (_, value) =
        _state <- { _state with Params = _state.Params.Add("from", value) }

    [<CustomOperation("mode")>]
    member __.Mode (_, value) =
        _state <- { _state with Params = _state.Params.Add("mode", value) }

    [<CustomOperation("UID")>]
    member __.UID (_, value : int) = 
        _state <- { _state with Params = _state.Params.Add("UID", value.ToString()) }

    [<CustomOperation("GID")>]
    member __.GID (_, value : int) = 
        _state <- { _state with Params = _state.Params.Add("GID", value.ToString()) }

    member __.Zero() = _state

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (_) = _state


type TmpfsParametersBuilder (target) =
    let mutable _state = MountParameters.Create(Tmpfs, [ "target", target ])

    member __.Yield (_) = ()

    [<CustomOperation("size")>]
    member __.Size (_, value : int) =
        _state <- { _state with Params = _state.Params.Add("size", value.ToString()) }

    member __.Zero() = _state

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (_) = _state


type SecretParametersBuilder () =
    let mutable _state = MountParameters.Create(Secret)

    member __.Yield (_) = ()

    [<CustomOperation("id")>]
    member __.Id (_, value) =
        _state <- { _state with Params = _state.Params.Add("id", value) }

    [<CustomOperation("target")>]
    member __.Target (_, value) =
        _state <- { _state with Params = _state.Params.Add("target", value) }

    [<CustomOperation("required")>]
    member __.Required (_, value : bool) =
        _state <- { _state with Params = _state.Params.Add("required", value.ToString().ToLower()) }

    [<CustomOperation("mode")>]
    member __.Mode (_, value) =
        _state <- { _state with Params = _state.Params.Add("mode", value) }

    [<CustomOperation("UID")>]
    member __.UID (_, value : int) =
        _state <- { _state with Params = _state.Params.Add("UID", value.ToString()) }

    [<CustomOperation("GID")>]
    member __.GID (_, value : int) =
        _state <- { _state with Params = _state.Params.Add("GID", value.ToString()) }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (_) = _state


type SshParametersBuilder () =
    let mutable _state = MountParameters.Create(Ssh)

    member __.Yield (_) = ()

    [<CustomOperation("id")>]
    member __.Id (_, value) =
        _state <- { _state with Params = _state.Params.Add("id", value) }

    [<CustomOperation("target")>]
    member __.Target (_, value) =
        _state <- { _state with Params = _state.Params.Add("target", value) }

    [<CustomOperation("required")>]
    member __.Required (_, value : bool) =
        _state <- { _state with Params = _state.Params.Add("required", value.ToString().ToLower()) }

    [<CustomOperation("mode")>]
    member __.Mode (_, value) =
        _state <- { _state with Params = _state.Params.Add("mode", value) }

    [<CustomOperation("UID")>]
    member __.UID (_, value : int) =
        _state <- { _state with Params = _state.Params.Add("UID", value.ToString()) }

    [<CustomOperation("GID")>]
    member __.GID (_, value : int) =
        _state <- { _state with Params = _state.Params.Add("GID", value.ToString()) }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (_) = _state


//---------------------------------------------------------------------------------------
// Entity types
//---------------------------------------------------------------------------------------


type Instruction =
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


and RunInstructionBuilder () =
    let mutable _state = RunInstruction.Create()

    member __.Yield (cmd : string) =
        _state <- { _state with Commands = _state.Commands |> Seq.append [ cmd ] }

    member __.Yield (cmds : string seq) =
        _state <- { _state with Commands = _state.Commands |> Seq.append cmds }

    member __.Yield (mount : MountParameters) =
        _state <- { _state with Mounts = _state.Mounts |> Seq.append [ mount ] }

    member __.Yield (network : NetworkType) =
        _state <- { _state with Network = Some network }

    member __.Yield (security : SecurityType) =
        _state <- { _state with Security = Some security }

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (_) = _state |> Run |> Instruction


and OnbuildInstruction = { Instruction : Entity }


and Entity = 
    | Plain of string
    | Instruction of Instruction
    | Subpart of Entity seq
