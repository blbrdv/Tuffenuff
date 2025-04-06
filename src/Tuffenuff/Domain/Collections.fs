module Tuffenuff.Domain.Collections

open System.Collections.ObjectModel
open System.Collections.Generic
open System.Linq


type Collection<'T>(elements : IEnumerable<'T>) =

    member _.Collection = elements


    interface IEnumerable<'T> with
        member this.GetEnumerator () = this.Collection.GetEnumerator ()


    interface System.Collections.IEnumerable with
        member this.GetEnumerator () = upcast this.Collection.GetEnumerator ()


    override this.Equals other =
        match other with
        | :? Collection<'T> as c -> Enumerable.SequenceEqual (this, c)
        | _ -> false


    override _.GetHashCode () = elements.GetHashCode ()


    member _.Append (coll2 : Collection<'T>) = Seq.append elements coll2 |> Collection

    member _.Add (element : 'T) = elements.Append element |> Collection


[<RequireQualifiedAccess>]
module Collection =

    let empty<'T> = Collection<'T> Seq.empty


type Arguments = Collection<string>


type Dict<'TKey, 'TValue when 'TValue : equality and 'TKey : comparison>
    (elements : Map<'TKey, 'TValue>)
    =

    inherit ReadOnlyDictionary<'TKey, 'TValue>(elements)


    new(elements : seq<'TKey * 'TValue>) = Dict (Map elements)


    new(key : 'TKey, value : 'TValue) = Dict [ key, value ]


    override this.Equals other =
        match other with
        | :? Dict<'TKey, 'TValue> as d -> Enumerable.SequenceEqual (this, d)
        | _ -> false


    override _.GetHashCode () = elements.GetHashCode ()


    member _.Append (map2 : Map<'TKey, 'TValue>) =
        Map.fold (fun acc key value -> Map.add key value acc) elements map2 |> Dict


    member this.Append (dict2 : Dict<'TKey, 'TValue>) =
        let map2 = dict2 |> Seq.map (|KeyValue|) |> Map.ofSeq
        this.Append map2


    member this.Add (key : 'TKey, value : 'TValue) = this.Append (Map [ key, value ])


[<RequireQualifiedAccess>]
module Dict =

    let empty<'TKey, 'TValue when 'TValue : equality and 'TKey : comparison> =
        Dict<'TKey, 'TValue> Map.empty


    let toMap (source : Dict<'TKey, 'TValue>) =
        source |> Seq.map (|KeyValue|) |> Map.ofSeq


    let ofDictionary (source : IDictionary<'TKey, 'TValue>) =
        source |> Seq.map (|KeyValue|) |> Map.ofSeq |> Dict


type Parameters = Dict<string, string>
