module CICD.TraceEx

// let inline traceOptions (header : string) (options : 'a) : 'a =
//     let prettify (value : 'b) : string =
//         let result = StringBuilder()
//         
//         let write (indent : int) (text : string) =
//             if indent > 0 then
//                 for _ = 1 to indent do
//                     result.Append("  ") |> ignore
//             
//             result.Append(text) |> ignore
//             
//         let sanitize (input : obj) : string =
//             $"%A{input}".Replace("\u001b", "\\u001b")
//             
//         let writeLine (indent : int) (text : string) =
//             write indent $"%s{text}%s{nl}"
//
//         let rec print (some : obj) (indent : int) =            
//             if some = null then
//                 writeLine indent "None"
//             else
//                 let type' = some.GetType()
//                 
//                 if type'.FullName.Equals("System.String") ||
//                    type'.FullName.Equals("Microsoft.FSharp.Core.string")
//                 then
//                     some
//                     |> string
//                     |> sprintf "\"%s\""
//                     |> writeLine indent
//                 elif type'.IsPrimitive then
//                     some
//                     |> string
//                     |> writeLine indent
//                 elif type'.FullName.StartsWith "Microsoft.FSharp.Collections.FSharpMap`2" then
//                     writeLine indent "["
//                     
//                     match type'.GetProperty("Item") with
//                     | null -> failwith "huh?"
//                     | _ ->
//                         let enumerator = (some :?> System.Collections.IEnumerable).GetEnumerator()
//                         
//                         seq {
//                             while enumerator.MoveNext() do
//                                 let entry = enumerator.Current
//                                 let key =
//                                     entry.GetType().GetProperty("Key").GetValue(entry)
//                                     |> box
//                                     |> sanitize
//                                 let value =
//                                     entry.GetType().GetProperty("Value").GetValue(entry)
//                                     |> box
//                                     |> sanitize
//                                 yield (key, value)
//                         }
//                         |> Seq.iter (fun kv ->
//                             let k, v = kv
//                             writeLine (indent + 1) $"(%s{k}, %s{v})"
//                         )
//                         
//                     writeLine indent "]"
//                 elif some :? IEnumerable<obj> then
//                     writeLine indent "["
//                     
//                     some :?> IEnumerable<obj>
//                     |> seq
//                     |> Seq.iter (fun v ->
//                         print v (indent + 1)
//                     )
//                     
//                     writeLine indent "]"
//                 else         
//                     let props =
//                         type'.GetProperties(BindingFlags.Instance ||| BindingFlags.Public)
//                         |> Array.where (fun p ->
//                             p.MemberType = MemberTypes.Property
//                         )
//                     
//                     if FSharpType.IsRecord type' && props.Length > 0 then
//                         writeLine indent "{"
//                         
//                         let nextIndent = indent + 1
//                         
//                         props
//                         |> Array.map (fun p ->
//                             (
//                                 p.Name,
//                                 p.GetValue(some)
//                             )
//                         )
//                         |> Array.iter (fun (name, v) ->
//                             write nextIndent $"%s{name} = "
//                             print v 0
//                         )
//                         
//                         writeLine indent "}"
//                     else
//                         writeLine indent $"%A{some}"
//             
//         print (box value) 1
//         
//         result.ToString()
//     
//     let cock = DotNet.Options.Create()
//     //cock.Value
//     let t1 = cock.Environment |> prettify
//     Trace.logfn $"%s{optsStr}Env\n%s{t1}"
//     
//     //Trace.logfn $"%s{optsStr}%s{header}:%s{Environment.NewLine}%A{options}"
//     options
