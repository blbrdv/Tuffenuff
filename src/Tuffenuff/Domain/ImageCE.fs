namespace Tuffenuff.Domain

open Tuffenuff.Domain.Entity
open Tuffenuff.Domain.Image
open Tuffenuff.Domain.DSL
open System.Collections.Generic

[<AutoOpen>]
module ImageCE =
    open System

    type IImageContext<'self> with
        member this.Yield(_) = this

        member __.Run(context : IImageContext<#IToImage>) =
            context.Self.Transform()


    and IToImage =
        abstract member Transform: unit -> Image

    and IToVariablesContext =
        inherit IToImage
        abstract member Transform: unit -> VariablesContext

    and IToStageHeaderContext =
        inherit IToImage
        abstract member Transform: string * option<string> -> StageHeaderContext

    and IToStageContext =
        inherit IToImage
        abstract member Transform: unit -> StageContext

    and DirectivesContext = 
        {
            Syntax: string option
            Escape : char option
            Entities : Entity seq
        } 
        with
            member private this.toEntities() =
                let entities = new List<Entity>(this.Entities)

                if this.Syntax.IsSome then
                    entities.Add(syntax this.Syntax.Value)

                if this.Escape.IsSome then
                    entities.Add(escape this.Escape.Value)

                entities :> seq<Entity>

            interface IImageContext<DirectivesContext> with
                member this.Self = this
                member this.Add(entity : Entity) =
                    { this with
                        Entities = Seq.append this.Entities [ entity ]
                    }
                member this.Append(entities : Entity seq) =
                    { this with
                        Entities = Seq.append this.Entities entities
                    }

            interface IToImage with
                member this.Transform() = Image(this.toEntities())

            interface IToVariablesContext with
                member this.Transform() =
                    {
                        VariablesContext.GlobalVariables = Seq.empty
                        Entities = this.toEntities()
                    }

            interface IToStageHeaderContext with
                member this.Transform(image, ?alias) =
                    {
                        Image = image
                        Alias = alias
                        Platform = None
                        GlobalVariables = Seq.empty
                        Entities = this.toEntities()
                    }

    and VariablesContext = 
        {
            GlobalVariables : string seq
            Entities : Entity seq
        } 
        with

            interface IImageContext<VariablesContext> with
                member this.Self = this
                member this.Add(entity : Entity) =
                    { this with
                        Entities = Seq.append this.Entities [ entity ]
                    }
                member this.Append(entities : Entity seq) =
                    { this with
                        Entities = Seq.append this.Entities entities
                    }

            interface IToImage with
                member this.Transform() = 
                    Image(this.Entities)

            interface IToStageHeaderContext with
                member this.Transform(image, ?alias) =
                    {
                        Image = image
                        Alias = alias
                        Platform = None
                        GlobalVariables = this.GlobalVariables
                        Entities = this.Entities
                    }

    and StageHeaderContext =
        {
            Image : string
            Alias : string option
            Platform : string option
            GlobalVariables : string seq
            Entities : Entity seq
        }
        with
            member private this.toEntities() =
                let from =
                    {
                        Image = this.Image
                        Name = this.Alias
                        Platform = this.Platform
                    }
                    |> From
                    |> Instruction
                
                Seq.append this.Entities [ from ]

            interface IImageContext<StageHeaderContext> with
                member this.Self = this
                member this.Add(entity : Entity) =
                    { this with
                        Entities = Seq.append this.Entities [ entity ]
                    }
                member this.Append(entities : Entity seq) =
                    { this with
                        Entities = Seq.append this.Entities entities
                    }

            interface IToImage with
                member this.Transform() = Image(this.toEntities())

            interface IToStageContext with
                member this.Transform() = 
                    {
                        StageContext.GlobalVariables = this.GlobalVariables
                        Entities = this.toEntities()
                    }

    and StageContext =
        {
            GlobalVariables : string seq
            Entities : Entity seq
        }
        with
            interface IImageContext<StageContext> with
                member this.Self = this
                member this.Add(entity : Entity) =
                    { this with
                        Entities = Seq.append this.Entities [ entity ]
                    }
                member this.Append(entities : Entity seq) =
                    { this with
                        Entities = Seq.append this.Entities entities
                    }

            interface IToImage with
                member this.Transform() = Image(this.Entities)

            interface IToStageContext with
                member this.Transform() = this

            interface IToStageHeaderContext with
                member this.Transform(image, ?alias) =
                    {
                        Image = image
                        Alias = alias
                        Platform = None
                        GlobalVariables = this.GlobalVariables
                        Entities = this.Entities
                    }

    and HealthcheckContext =
        {
            Interval : string option
            Timeout : string option
            StartPeriod : string option
            Retries : int option
            GlobalVariables : string seq
            Entities : Entity seq
        }
        with
            interface IImageContext<HealthcheckContext> with
                member this.Self = this
                member this.Add(entity : Entity) =
                    { this with
                        Entities = Seq.append this.Entities [ entity ]
                    }
                member this.Append(entities : Entity seq) =
                    { this with
                        Entities = Seq.append this.Entities entities
                    }

            interface IToImage with
                member this.Transform() = Image(this.Entities)

            interface IToStageHeaderContext with
                member this.Transform(image, ?alias) =
                    {
                        Image = image
                        Alias = alias
                        Platform = None
                        GlobalVariables = this.GlobalVariables
                        Entities = this.Entities
                    }

            interface IToStageContext with
                member this.Transform() = 
                    {
                        StageContext.GlobalVariables = this.GlobalVariables
                        Entities = this.Entities
                    }
                    
    type IImageContext<'self> with

        [<CustomOperation("syntax")>]
        member __.SetSyntax(context : IImageContext<DirectivesContext>, value) =
            let dir = context.Self
            if dir.Syntax.IsSome then
                raise (ArgumentException("asda"))
            { dir with
                Syntax = Some value
            }

        [<CustomOperation("escape")>]
        member __.SetEscape(context : IImageContext<DirectivesContext>, value) =
            let dir = context.Self
            if dir.Escape.IsSome then
                raise (ArgumentException("asda"))
            { dir with
                Escape = Some value
            }
                    
    type IImageContext<'self> with

        [<CustomOperation("ARG")>]
        member __.SetArg(context : IImageContext<#IToVariablesContext>, key : string, value : string) =
            let dir = context.Self.Transform()
            let arg = 
                {
                    Name = "ARG"
                    Key = key
                    Value = value
                }
                |> KeyValue
                |> Instruction

            { dir with
                GlobalVariables = Seq.append dir.GlobalVariables [ key ]
                Entities = Seq.append dir.Entities [ arg ]
            }

    type As = AS
                    
    type IImageContext<'self> with

        [<CustomOperation("FROM")>]
        member __.AddFrom (context : IImageContext<#IToStageHeaderContext>, name : string, ?___ : As, ?alias : string) =
            context.Self.Transform(name, alias)

        [<CustomOperation("platform")>]
        member __.SetPLatform (context : IImageContext<StageHeaderContext>, value : string) =
            { context.Self with
                Platform = Some value
            }
                    
    type IImageContext<'self> with

        [<CustomOperation("___")>]
        member __.BreakLine (context : IImageContext<#IToStageContext>) =
            let stage = context.Self.Transform()
            { stage with
                Entities = Seq.append stage.Entities [ (Plain "") ]
            }

        [<CustomOperation("CMD")>]
        member __.AddCmd (context : IImageContext<#IToStageContext>, elements : IEnumerable<string>) =
            let stage = context.Self.Transform()
            let cmd =
                {
                    ListInstruction.Name = "CMD"
                    Elements = elements
                }
                |> InstructionType.List
                |> Instruction
            { stage with
                Entities = Seq.append stage.Entities [ cmd ]
            }

        [<CustomOperation("RUN")>]
        member __.AddRun (context : IImageContext<#IToStageContext>, elements : IEnumerable<string>) =
            let stage = context.Self.Transform()
            let run =
                {
                    ListInstruction.Name = "RUN"
                    Elements = elements
                }
                |> InstructionType.List
                |> Instruction
            { stage with
                Entities = Seq.append stage.Entities [ run ]
            }

        [<CustomOperation("WORKDIR")>]
        member __.AddWorkdir (context : IImageContext<#IToStageContext>, value : string) =
            let stage = context.Self.Transform()
            let workdir =
                {
                    Name = "WORKDIR"
                    Value = value
                }
                |> Simple
                |> Instruction
            { stage with
                Entities = Seq.append stage.Entities [ workdir ]
            }

        [<CustomOperation("COPY")>]
        member __.AddCopy (context : IImageContext<#IToStageContext>, elements : IEnumerable<string>) =
            let stage = context.Self.Transform()
            let cp =
                {
                    ListInstruction.Name = "COPY"
                    Elements = elements
                }
                |> InstructionType.List
                |> Instruction
            { stage with
                Entities = Seq.append stage.Entities [ cp ]
            }

    type IImageContext<'self> with
        [<CustomOperation("cmt")>]
        member __.AddComment (context : IImageContext<'a>, value : string) : IImageContext<'a> =
            let comment =
                {
                    Name = "#"
                    Value = value
                }
                |> Simple
                |> Instruction
            context.Add(comment)

        [<CustomOperation("incl")>]
        member __.Include (context : IImageContext<'a>, image : Image) : IImageContext<'a> =
            context.Append(image.Lines)
