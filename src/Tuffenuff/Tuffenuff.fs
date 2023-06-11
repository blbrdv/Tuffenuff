[<AutoOpen>]
module Tuffenuff.Dockerfile

open System.IO
open Tuffenuff.Domain.Types
open Tuffenuff.String
open Tuffenuff.StringCE


let dockerfile (entities : Entity seq) = entities


let df = dockerfile


let plain = Plain


let br = plain ""


let part = Subpart


let ( !& ) = part


[<RequireQualifiedAccess>]
module Dockerfile =

    let render df = 
        let rec renderInstruction instr =
            match instr with
            | Simple s -> print s.Name s.Value

            | SimpleQuoted s -> print s.Name (quote s.Value)

            | List l -> print l.Name (printList l.Elements.Collection)

            | KeyValue kv -> print kv.Name (printKVQ kv.Key kv.Value)

            | KeyValueList l ->
                str {
                    l.Name
                    " "
                    l.Elements 
                    |> Seq.map (fun e -> 
                        printKVQ e.Key e.Value)
                    |> String.concat eol_slash
                }

            | From f ->
                str {
                    "FROM"
                    printParameterQ "platform" f.Platform
                    sprintf " %s" f.Image
                    if f.Name.IsSome then 
                        sprintf " AS %s" f.Name.Value
                }

            | Run r ->
                let mutable result = 
                    r.Mounts
                    |> Seq.map (
                        fun m ->
                            m.Params
                            |> Seq.map (fun p -> printKV p.Key p.Value)
                            |> Seq.append [ 
                                printKV "type" (m.Name.ToString().ToLower()) 
                            ]
                            |> String.concat ","
                            |> sprintf "--mount=%s"
                    )
                
                if r.Network.IsSome then do 
                    let value =
                        (nameof r.Network.Value).ToLower()
                        |> sprintf "--network=%s"
                    
                    result <-
                        result
                        |> Seq.append [ value ]
                
                if r.Security.IsSome then do 
                    let value =
                        (nameof r.Network.Value).ToLower()
                        |> sprintf "--security=%s"
                    
                    result <-
                        result
                        |> Seq.append [ value ]

                result <- 
                    result
                    |> Seq.append r.Arguments

                result
                |> String.concat eol_slash
                |> sprintf "RUN%s%s" eol_slash

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
                    printList a.Elements.Collection
                }

            | Copy cp -> 
                str {
                    "COPY"
                    printFlag "link" cp.Link
                    printParameter "from" cp.From
                    printParameter "chmod" cp.Chmod
                    printParameter "chown" cp.Chown
                    " "
                    printList cp.Elements.Collection
                }

            | Healthcheck hc -> 
                str {
                    "HEALTCHECK"
                    printParameter "interval" hc.Interval
                    printParameter "timeout" hc.Timeout
                    printParameter "start-period" hc.StartPeriod
                    printParameter "retries" hc.Retries
                    sprintf "%sCMD %s" eol_slash (printList hc.Instructions.Collection)
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
            |> String.concat eol
        
        r df
        |> trim
        |> sprintf "%s%s"
        <| eol


    let toFile (path : string) (text : string) =
        File.WriteAllText(path, text)


    let fromFile (path : string) =
        seq {
            for line in File.ReadLines(path) do
                plain line
        }