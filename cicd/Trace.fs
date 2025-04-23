[<RequireQualifiedAccess>]
module CICD.Trace

open System
open System.Collections
open System.Reflection
open Fake.Core
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Reflection

let inline traceObjectWithName (name : string) (data : 'a) : 'a =
    let inline withIndent (length : int) (value : string) : string =
        let indent =
            if length > 0 then
                String.replicate length "  "
            else
                ""

        (indent, value)
        ||> sprintf "%s%s"

    let inline toString
        (separator : string)
        (indentLength : int)
        (key : string)
        (values : string list)
        : string list
        =

        let keyWithIndent = withIndent indentLength key

        match List.length values with
        | 0 -> [ $"%s{keyWithIndent}%s{separator}None" ]
        | 1 -> [ $"%s{keyWithIndent}%s{separator}%s{Seq.head values}" ]
        | n ->
            let result = ResizeArray<string> (n + 1)
            let nextIndent = indentLength + 1

            result.Add $"%s{keyWithIndent}%s{separator}"

            values
            |> List.iter (fun value ->
                value
                |> withIndent nextIndent
                |> result.Add
            )
            
            result.ToArray() |> List.ofArray

    let inline isUnion (type': Type) : bool =
        FSharpType.IsUnion type' &&
        not (
            type'.GetInterfaces()
            |> Array.exists (fun i -> i.Name = "IEnumerable")
        )

    let rec formList (indent : int) (boxed : obj) : string list =
        let prevIndent = indent - 1
        let nextIndent = indent + 1
        let result = ResizeArray<string> ()
        let origType = lazy boxed.GetType ()

        match (boxed, origType) with
        | null, _ ->
            "None"
            |> withIndent indent
            |> result.Add 

        | :? string as s, _ ->
            s
            |> sanitize
            |> sprintf "\"%s\""
            |> withIndent indent
            |> result.Add

        | p, t when t.Value.IsPrimitive ->
            p.ToString()
            |> withIndent indent
            |> result.Add

        | tp, t when FSharpType.IsTuple t.Value ->
            FSharpValue.GetTupleFields tp
            |> Array.map (fun o ->
                o
                |> formList prevIndent
                |> List.head
            )
            |> String.concat ", "
            |> sprintf "(%s)"
            |> withIndent indent
            |> result.Add

        | u, t when isUnion t.Value ->
            let case, values = FSharpValue.GetUnionFields (u, t.Value)

            match values.Length with
            | 0 -> result.Add case.Name

            | 1 ->
                let data = values[0] |> formList prevIndent

                (indent, case.Name, data)
                |||> toString " "
                |> result.AddRange

            | _ ->
                let tupleType =
                    values
                    |> Array.map (fun o -> o.GetType())
                    |> FSharpType.MakeTupleType

                let tuple =
                    FSharpValue.MakeTuple(values, tupleType)
                    |> formList indent

                (indent, case.Name, tuple)
                |||> toString " "
                |> result.AddRange

        | :? IEnumerable as enumerable, t when
            t.Value.IsGenericType &&
            t.Value.GetGenericTypeDefinition() = typedefof<Map<_,_>>
            ->
                let enumerator = enumerable.GetEnumerator()
                let list = ResizeArray<string * obj>()

                while enumerator.MoveNext() do
                    let en = enumerator.Current
                    let key =
                        en.GetType().GetProperty("Key").GetValue en
                        |> sprintf "%A"                    
                    let value =
                        en.GetType().GetProperty("Value").GetValue en
                        |> box

                    (key, value)
                    |> list.Add

                if list.Count = 0 then
                    result.Add "[]"
                else
                    result.Add "["

                    for key, value in list do
                        let data = value |> formList indent
                
                        (nextIndent, key, data)
                        |||> toString " = "
                        |> result.AddRange

                    result.Add "]"

        | :? IEnumerable as enumerable, _ ->
            let list = ResizeArray<obj>()
            let en = enumerable.GetEnumerator()

            while en.MoveNext() do
                en.Current
                |> box
                |> list.Add

            if list.Count = 0 then
                result.Add "()"
            else
                result.Add "("

                for value in list do
                    value
                    |> formList nextIndent
                    |> result.AddRange

                result.Add ")"

        | r, t when FSharpType.IsRecord t.Value ->
            let props =
                t.Value.GetProperties (BindingFlags.Instance ||| BindingFlags.Public)
                |> Array.where (fun p -> p.MemberType = MemberTypes.Property)

            if props.Length = 0 then
                result.Add "{}"
            else
                result.Add "{"

                props
                |> Array.map (fun p -> (p.Name, p.GetValue r))
                |> Array.iter (fun (name, v) ->
                    // Redacting the "Environment" property because we are already
                    // tracing environment variables on start
                    if "Environment".Equals name then
                        withIndent nextIndent name
                        |> sprintf "%s = [...]"
                        |> result.Add
                    else
                        let data = v |> formList indent

                        (nextIndent, name, data)
                        |||> toString " = "
                        |> result.AddRange
                )

                result.Add "}"

        | o, _ ->
            o
            |> sprintf "%A"
            |> withIndent indent
            |> sanitize
            |> result.Add

        result.ToArray() |> List.ofArray

    let value = data |> box |> formList 0

    let result = (0, name, value) |||> toString " = " |> String.concat newLine

    Trace.traceLine ()
    Trace.trace result
    Trace.traceLine ()

    data

let inline traceObject (data : 'a) : 'a =
    let dataType = data.GetType ()

    let module' = dataType.Module.Name.Replace (".dll", "")
    let name = $"%s{module'}.%s{dataType.Name}"

    traceObjectWithName name data
