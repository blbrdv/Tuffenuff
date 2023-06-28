namespace Tuffenuff.Domain.Image

open System.Collections.Generic
open System.Text
open Tuffenuff.Domain.Entity


type IImageContext<'self> =
    abstract member Self: 'self
    abstract member Add: Entity -> 'self
    abstract member Append: Entity seq -> 'self

type Image (entities : Entity seq) =
    let values = new List<Entity>(entities)

    member __.Lines = values :> seq<Entity>

    new () =
        Image([])

    member __.Add (value : Entity) =
        values.Add(value)

    member __.Append (entities : Entity seq) =
        values.AddRange(entities)

    override __.ToString() =
        let sb = StringBuilder()
        for value in values do
            sb.AppendLine(value.ToString())
            |> ignore
        sb.ToString()
