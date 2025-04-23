namespace Tuffenuff

// http://www.fssnip.net/7WR
module internal StringCE =
    open System.Text
    open Printf
    open Tuffenuff.String

    type private StringBuffer = StringBuilder -> unit
    type private StringBuffer<'T> = StringBuilder -> 'T

    [<Sealed>]
    type StringBufferBuilder() =
        member _.Yield (value : string) : StringBuffer =
            fun (builder : StringBuilder) -> bprintf builder $"%s{value}"

        member _.Yield (value : char) : StringBuffer =
            fun (builder : StringBuilder) -> bprintf builder $"%c{value}"

        member _.Yield (value : byte) : StringBuffer =
            fun (builder : StringBuilder) -> bprintf builder $"%02x{value}"

        member _.Yield (values : #seq<string>) : StringBuffer =
            fun (builder : StringBuilder) ->
                for value in values do
                    bprintf builder $"%s{value}%s{eol}"

        member _.YieldFrom (buffer : StringBuffer) : StringBuffer = buffer

        member _.Combine
            (
                bufferLeft : StringBuffer,
                bufferRight : StringBuffer<'a>
            )
            : StringBuffer<'a>
            =
            fun (builder : StringBuilder) ->
                bufferLeft builder
                bufferRight builder

        member _.Delay (wrapper : unit -> StringBuffer<'a>) : StringBuffer<'a> =
            fun (builder : StringBuilder) -> wrapper () builder

        member _.Zero () : 'a -> unit = ignore

        member _.For (values : 'a seq, yield' : 'a -> StringBuffer) : StringBuffer =
            fun (builder : StringBuilder) ->
                use e = values.GetEnumerator ()

                while e.MoveNext () do
                    yield' e.Current builder

        member _.While (test : unit -> bool, buffer : StringBuffer) : StringBuffer =
            fun builder ->
                while test () do
                    buffer builder

        member _.Run (buffer : StringBuffer) : string =
            let builder = StringBuilder ()
            do buffer builder
            builder.ToString ()

    let str = StringBufferBuilder ()
