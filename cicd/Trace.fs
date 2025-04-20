[<RequireQualifiedAccess>]
module CICD.Trace

open System.Collections.Generic
open System.Reflection
open Fake.Core
open Microsoft.FSharp.Reflection

let inline logObject (data : 'a) : 'a =
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
        (values : List<string>)
        : List<string>
        =

        let keyWithIndent = withIndent indentLength key

        match values.Count with
        | 0 -> [ $"%s{keyWithIndent}%s{separator}None" ] |> List
        | 1 -> [ $"%s{keyWithIndent}%s{separator}%s{Seq.head values}" ] |> List
        | n ->
            let result = List<string> (n + 1)
            let nextIndent = indentLength + 1

            result.Add $"%s{keyWithIndent}%s{separator}"

            for v in values do
                withIndent nextIndent v
                |> result.Add
            
            result

    let rec formList (indent : int) (boxed : obj) : List<string> =
        let nextIndent = indent + 1
        let result = List<string> ()

        if boxed = null then
            "None"
            |> withIndent indent
            |> result.Add 
        else
            let origType = boxed.GetType ()

            // TODO: refactor type determination
            if
                origType.FullName.Equals "System.String" ||
                origType.FullName.Equals "Microsoft.FSharp.Core.string"
            then

                boxed
                |> string
                |> sanitize
                |> sprintf "\"%s\""
                |> withIndent indent
                |> result.Add

            elif origType.IsPrimitive then
                
                boxed
                |> string
                |> withIndent indent
                |> result.Add
            
            elif
                FSharpType.IsUnion origType &&
                not (
                    origType.GetInterfaces()
                    |> Array.exists (fun i -> i.Name = "IEnumerable")
                )
            then
                let case, values = FSharpValue.GetUnionFields (boxed, origType)

                match values.Length with
                | 0 -> result.Add case.Name
                | 1 ->
                    // "(indent - 1)" - This is bullshit but it's works
                    let data = values[0] |> formList (indent - 1)
                    
                    (indent, case.Name, data)
                    |||> toString " "
                    |> result.AddRange
                | _ ->
                    let list = formList indent values
                    
                    (indent, case.Name, list)
                    |||> toString " "
                    |> result.AddRange
                
            elif origType.FullName.StartsWith "Microsoft.FSharp.Collections.FSharpMap`2"
            then

                match origType.GetProperty ("Item") with
                | null -> failwith "Not supposed to happened!"
                | _ ->
                    let list = (boxed :?> IEnumerable<obj>) |> List.ofSeq

                    if List.length list = 0 then
                        result.Add "[]"
                    else
                        result.Add "["

                        list
                        |> List.map (fun o ->
                            let key = o.GetType().GetProperty("Key").GetValue o |> box

                            let value =
                                o.GetType().GetProperty("Value").GetValue o |> box

                            (key, value)
                        )
                        |> List.iter (fun kv ->
                            let k, v = kv
                            let key = $"%A{k}"

                            let data = v |> formList indent

                            (nextIndent, key, data)
                            |||> toString " = "
                            |> result.AddRange
                        )

                        result.Add "]"

            elif boxed :? IEnumerable<obj> then
                let list = (boxed :?> IEnumerable<obj>) |> List.ofSeq

                if List.length list = 0 then
                    result.Add "()"
                else
                    result.Add "("

                    list
                    |> List.iter (fun v ->
                        v
                        |> formList nextIndent
                        |> result.AddRange
                    )
                    
                    result.Add ")"

            else

                let props =
                    origType.GetProperties (BindingFlags.Instance ||| BindingFlags.Public)
                    |> Array.where (fun p -> p.MemberType = MemberTypes.Property)

                if FSharpType.IsRecord origType then
                    if props.Length = 0 then
                        result.Add "{}"
                    else
                        result.Add "{"

                        props
                        |> Array.map (fun p -> (p.Name, p.GetValue (boxed)))
                        |> Array.iter (fun (name, v) ->
                            // Redacting the "Environment" property because we are already
                            // tracing environment variables on start
                            if "Environment".Equals (name) then
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

                else
                    $"%A{boxed}"
                    |> withIndent indent
                    |> sanitize
                    |> result.Add

        result

    let dataType = data.GetType ()

    let module' = dataType.Module.Name.Replace (".dll", "")
    let key = $"%s{module'}.%s{dataType.Name}"

    let value = data |> box |> formList 0

    let result = (0, key, value) |||> toString " = " |> String.concat newLine

    Trace.traceLine ()
    Trace.trace result
    Trace.traceLine ()

    data
