namespace Tuffenuff.Domain.Entity


type SimpleInstruction = { Name : string ; Value : string }


type ListInstruction =
    { Name : string ; Elements : string seq }


type KVInstruction =
    {
        Name : string
        Key : string
        Value : string
    }


type KVListInstruction =
    {
        Name : string
        Elements : Map<string, string>
    }


type FromInstruction =
    {
        Image : string
        Name : string option
        Platform : string option
    }

    static member Create (image, ?name : string, ?platform : string) =
        {
            Image = image
            Name = name
            Platform = platform
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
    // | Run of RunInstruction
    // | Add of AddInstruction
    // | Copy of CopyInstruction
    | Onbuild of OnbuildInstruction
    // | Healthcheck of HealthcheckInstruction


and OnbuildInstruction = { Instruction : Entity }


and Entity =
    | Plain of string
    | Instruction of InstructionType
    | Subpart of Entity seq
