namespace Tuffenuff

[<AutoOpen>]
module DSL =

    open Tuffenuff.Domain.ImageCE
    open Tuffenuff.Domain.Image
    open Tuffenuff.Domain.Entity
    
    let image = 
        {
            Syntax = None
            Escape = None
            Entities = Seq.empty
        }

    let stage =
        {
            GlobalVariables = Seq.empty
            Entities = Seq.empty
        }
