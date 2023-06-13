module Tuffenuff.Domain.Types

open System
open System.Collections.Generic
open Tuffenuff.Domain.Collections


//---------------------------------------------------------------------------------------
// Base types
//---------------------------------------------------------------------------------------


type SimpleInstruction = { Name : string; Value : string }    


type ListInstruction = { Name : string; Elements : Arguments }


type KVInstruction = { Name : string; Key : string; Value : string }


type KVListInstruction = { Name : string; Elements : Parameters }


type FromInstruction = 
    { 
        Image : string; 
        Name : string option; 
        Platform : string option 
    }
    with
        static member Create(image) =
            { Image = image; Name = None; Platform = None }


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


type HealthcheckInstruction = 
    { 
        Options : Parameters
        Instructions : Arguments
    }
    with
        static member Create(cmds) =
            let instr =
                match box (cmds) with
                | :? string as s -> Arguments [ s ]
                | :? IEnumerable<string> as ls -> Arguments ls
                | _ -> raise (ArgumentException("Invalid argument type"))
            { 
                Options = Parameters []; 
                Instructions = instr
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





