// http://www.fssnip.net/7WR/title/Computation-expression-over-StringBuilder
module Tuffenuff.StringCE

open System.Text


type StringBuffer = StringBuilder -> unit


type StringBufferBuilder() =
    member inline _.Yield (txt : string) =
        fun (b : StringBuilder) -> Printf.bprintf b $"%s{txt}"

    member inline _.Yield (c : char) =
        fun (b : StringBuilder) -> Printf.bprintf b $"%c{c}"

    member inline _.Yield (strings : #seq<string>) =
        fun (b : StringBuilder) ->
            for s in strings do
                Printf.bprintf b $"%s{s}\n"

    member inline _.YieldFrom (f : StringBuffer) = f

    member _.Combine (f, g) =
        fun (b : StringBuilder) ->
            f b
            g b

    member _.Delay f = fun (b : StringBuilder) -> (f ()) b

    member _.Zero () = ignore

    member _.For (xs : 'a seq, f : 'a -> StringBuffer) =
        fun (b : StringBuilder) ->
            use e = xs.GetEnumerator ()

            while e.MoveNext () do
                (f e.Current) b

    member _.While (p : unit -> bool, f : StringBuffer) =
        fun (b : StringBuilder) ->
            while p () do
                f b

    member _.Run (f : StringBuffer) =
        let b = StringBuilder ()
        do f b
        b.ToString ()


let str = StringBufferBuilder ()


type StringBufferBuilder with

    member inline _.Yield (b : byte) =
        fun (sb : StringBuilder) -> Printf.bprintf sb $"%02x{b} "
