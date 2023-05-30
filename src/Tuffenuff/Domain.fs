namespace Tuffenuff

open System
open System.Collections.Generic

module Domain =
    type List = IEnumerable<string>

    type SimpleInstruction = { Name : string; Value : string }

    type ListInstruction = { Name : string; Elements : List }

    type KVInstruction = { Name : string; Key : string; Value : string }

    type KVListInstruction = { Name : string; Elements : Tuple<string, string> seq }

    type FromInstruction = { Image : string; Name : string option; Platform : string option }

    type RunInstruction = { Commands : List }

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
