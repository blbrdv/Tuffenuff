namespace DockerfileDSL.FSharp

open System
open DockerfileDSL.FSharp.Domain
open DockerfileDSL.FSharp.Utils.String

[<AutoOpen>]
module DSL =
    let private quote value =
        sprintf "%c%s%c" '"' (string value) '"'

    let private print name value = sprintf "%s %s" name value

    let private printKV key value = sprintf "%s=%s" key (quote value)

    let private printList list =
        if (list :> obj) :? string[] then 
            list |> Seq.map quote |> String.concat ", " |> sprintf "[ %s ]"
        else
            list |> String.concat " "

    let private printFlag name value =
        if value then 
            sprintf " --%s" name 
        else 
            ""

    let private printParameter<'T> name (value : 'T option) =
        if value.IsSome then 
            sprintf " --%s=%s" name (string value.Value)
        else
            ""

    let private printParameterQ<'T> name (value : 'T option) =
        match value with
        | Some s -> s |> quote |> Some
        | _ -> None
        |> printParameter name

    let private trim (text : string) = text.TrimStart('\n')

    let render df = 
        let rec renderInstruction instr =
            match instr with
            | Simple s -> print s.Name s.Value

            | SimpleQuoted s -> print s.Name (quote s.Value)

            | List l -> print l.Name (printList l.Elements)

            | KeyValue kv -> print kv.Name (printKV kv.Key kv.Value)

            | KeyValueList l ->
                str {
                    l.Name
                    " "
                    l.Elements 
                    |> Seq.map (fun e -> 
                        let (key, value) = e
                        printKV key value)
                    |> String.concat " \\\n    "
                }

            | From f ->
                str {
                    "\nFROM"
                    printParameterQ "platform" f.Platform
                    sprintf " %s" f.Image
                    if f.Name.IsSome then 
                        sprintf " AS %s" f.Name.Value
                }

            | Run r -> 
                r.Commands
                |> String.concat ", "
                |> sprintf "RUN %s"

            | Add a -> 
                str {
                    "ADD"
                    printFlag "link" a.Link
                    if a.KeepGitDir then
                        sprintf " --keep-git-dir=true"
                    printParameter "chmod" a.Chmod
                    printParameter "chown" a.Chown
                    printParameter "checksum" a.Checksum
                    " "
                    printList a.Elements
                }

            | Copy cp -> 
                str {
                    "COPY"
                    printFlag "link" cp.Link
                    printParameter "from" cp.From
                    printParameter "chmod" cp.Chmod
                    printParameter "chown" cp.Chown
                    " "
                    printList cp.Elements
                }

            | Healthcheck hc -> 
                str {
                    "HEALTCHECK"
                    printParameter "interval" hc.Interval
                    printParameter "timeout" hc.Timeout
                    printParameter "start-period" hc.StartPeriod
                    printParameter "retries" hc.Retries
                    sprintf " \\\n    CMD %s" (printList hc.Instructions)
                }

            | Onbuild onb -> 
                seq { onb.Instruction }
                |> r
                |> sprintf "ONBUILD %s"

        and r sub =
            sub
            |> Seq.map (fun instr ->
                match instr with
                | Plain t -> t
                | Instruction i -> renderInstruction i
                | Subpart s -> r s |> trim
            )
            |> String.concat "\n"
        
        r df
        |> trim
