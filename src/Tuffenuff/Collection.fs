module Tuffenuff.Collection

open System.Linq

type Collection<'T>(collection : seq<'T>) =
    member __.Collection with get() = collection

    interface System.Collections.Generic.IEnumerable<'T> with
        member this.GetEnumerator() =
            this.Collection.GetEnumerator()

    interface System.Collections.IEnumerable with
        member this.GetEnumerator() =
            upcast this.Collection.GetEnumerator()

    override this.Equals other =
        match other with
        | :? Collection<'T> as c -> Enumerable.SequenceEqual(this, c) //this.SequenceEqual(kek) 
        | _ -> false
        
    override __.GetHashCode() = collection.GetHashCode()

    interface System.IComparable with
        member this.CompareTo obj =
            match obj with
            | :? Collection<'T> as c -> if Enumerable.SequenceEqual(this, c) then 1 else -1
            | _ -> invalidArg "obj" "cannot compare values of different types"
    
    static member empty = Collection<'T>(Seq.empty)
