namespace Tuffenuff

open System
open System.Collections.Generic

module Domain =
    type List = IEnumerable<string>

    type KVList = Tuple<string, string> seq

    type SimpleInstruction = { Name : string; Value : string }

    type ListInstruction = { Name : string; Elements : List }

    type KVInstruction = { Name : string; Key : string; Value : string }

    type KVListInstruction = { Name : string; Elements : KVList }

    type FromParameter =
        | As of string
        | Platform of string

    type FromInstruction = { Image : string; Name : string option; Platform : string option }

    type BindOptions = 
        {
            Target : string;
            Source : string option;
            From : string option;
            RW : bool option;
        }
        static member Create(target) = 
            { Target = target; Source = None; From = None; RW = None }

    type BindOptionsBuilder () =
        member __.Yield (_) = ()

        [<CustomOperation("target")>]
        member __.Target (_, value) = BindOptions.Create(value)

        [<CustomOperation("source")>]
        member __.Source (state, value) =
            { state with Source = Some value }

        [<CustomOperation("from")>]
        member __.From (state, value) =
            { state with From = Some value }

        [<CustomOperation("rw")>]
        member __.RW (state, value) =
            { state with RW = Some value }

    type SharingType =
        | Shared
        | Private
        | Locked

    type CacheOptions = 
        {
            Id : string option;
            Target : string;
            RO : bool option;
            Sharing : SharingType option;
            Source : string option;
            From : string option;
            Mode : string option;
            UID : int option;
            GID : int option;
        }
        static member Create(target) = 
            { 
                Id = None;
                Target = target;
                RO = None;
                Sharing = None;
                Source = None;
                From = None;
                Mode = None;
                UID = None;
                GID = None;
            }

    type CacheOptionsBuilder () =
        member __.Yield (_) = ()

        [<CustomOperation("target")>]
        member __.Target (_, value) = CacheOptions.Create(value)

        [<CustomOperation("id")>]
        member __.Id (state, value) =
            { state with Id = Some value }

        [<CustomOperation("ro")>]
        member __.RO (state, value) =
            { state with RO = Some value }

        [<CustomOperation("sharing")>]
        member __.Sharing (state, value) =
            { state with Sharing = Some value }

        [<CustomOperation("source")>]
        member __.Source (state : CacheOptions, value) =
            { state with Source = Some value }

        [<CustomOperation("from")>]
        member __.From (state : CacheOptions, value) =
            { state with From = Some value }

        [<CustomOperation("mode")>]
        member __.Mode (state, value) =
            { state with Mode = Some value }

        [<CustomOperation("UID")>]
        member __.UID (state, value) =
            { state with UID = Some value }

        [<CustomOperation("GID")>]
        member __.GID (state, value) =
            { state with GID = Some value }

    type TmpfsOptions = 
        {
            Target : string;
            Size : int option;
        }
        static member Create(target) = 
            { 
                Target = target;
                Size = None
            }
    
    type TmpfsOptionsBuilder () =
        member __.Yield (_) = ()

        [<CustomOperation("target")>]
        member __.Target (_, value) = TmpfsOptions.Create(value)

        [<CustomOperation("size")>]
        member __.Size (state, value) =
            { state with Size = Some value }

    type SecretOptions = 
        {
            Id : string option;
            Target : string option;
            Required : bool option;
            Mode : string option;
            UID : int option;
            GID : int option;
        }
        static member Create() = 
            { 
                Id = None;
                Target = None;
                Required = None;
                Mode = None;
                UID = None;
                GID = None;
            }

    type SecretOptionsBuilder () =
        member __.Yield (_) = ()

        member __.CreateOrModify(state, f) =
            let x = state :> obj
            match x with
            | :? SecretOptions as so -> so |> f
            | _ -> SecretOptions.Create() |> f

        [<CustomOperation("id")>]
        member this.Id (state, value) =
            this.CreateOrModify(state, fun x -> { x with Id = Some value })

        [<CustomOperation("target")>]
        member this.Target (state, value) =
            this.CreateOrModify(state, fun x -> { x with Target = Some value })

        [<CustomOperation("required")>]
        member this.Required (state, value) =
            this.CreateOrModify(state, fun x -> { x with Required = Some value })

        [<CustomOperation("mode")>]
        member this.Mode (state, value) =
            this.CreateOrModify(state, fun x -> { x with Mode = Some value })

        [<CustomOperation("UID")>]
        member this.UID (state, value) =
            this.CreateOrModify(state, fun x -> { x with UID = Some value })

        [<CustomOperation("GID")>]
        member this.GID (state, value) =
            this.CreateOrModify(state, fun x -> { x with GID = Some value })
    
    type SshOptions = 
        {
            Id : string option;
            Target : string option;
            Required : bool option;
            Mode : string option;
            UID : int option;
            GID : int option;
        }
        static member Create() = 
            { 
                Id = None;
                Target = None;
                Required = None;
                Mode = None;
                UID = None;
                GID = None;
            }

    type SshOptionsBuilder () =
        member __.Yield (_) = ()

        member __.CreateOrModify(state, f) =
            let x = state :> obj
            match x with
            | :? SshOptions as so -> so |> f
            | _ -> SshOptions.Create() |> f

        [<CustomOperation("id")>]
        member this.Id (state, value) =
            this.CreateOrModify(state, fun x -> { x with Id = Some value })

        [<CustomOperation("target")>]
        member this.Target (state, value) =
            this.CreateOrModify(state, fun x -> { x with Target = Some value })

        [<CustomOperation("required")>]
        member this.Required (state, value) =
            this.CreateOrModify(state, fun x -> { x with Required = Some value })

        [<CustomOperation("mode")>]
        member this.Mode (state, value) =
            this.CreateOrModify(state, fun x -> { x with Mode = Some value })

        [<CustomOperation("UID")>]
        member this.UID (state, value) =
            this.CreateOrModify(state, fun x -> { x with UID = Some value })

        [<CustomOperation("GID")>]
        member this.GID (state, value) =
            this.CreateOrModify(state, fun x -> { x with GID = Some value })

    type MountType =
        | Bind of BindOptions
        | Cache of CacheOptions
        | Tmpfs of TmpfsOptions
        | Secret of SecretOptions
        | Ssh of SshOptions

    // type MountType =
    //     | BindOptions of BindOptions
    //     | CacheOptions
    //     | TmpfsOptions
    //     | SecretOptions
    //     | SshOptions

    type NetworkType =
        | Def    // Default
        | Absent // None
        | Host
        
    type SecurityType =
        | Insecure
        | Sandbox

    type RunFlag =
        | Mount of MountType
        | Network of NetworkType
        | Security of SecurityType

    type RunInstruction = { 
        Flags : RunFlag seq;
        Commands : List; 
    }
    
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

    and OnbuildInstruction = { Instruction : Entity }

    and Entity = 
        | Plain of string
        | Instruction of Instruction
        | Subpart of Entity seq

    let instr = Instruction
