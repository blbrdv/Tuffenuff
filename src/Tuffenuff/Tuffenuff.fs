[<AutoOpen>]
module Tuffenuff.Dockerfile

open System
open System.IO
open Tuffenuff.Domain.Types
open Tuffenuff.String
open Tuffenuff.StringCE

type private Entities = Entity seq

let dockerfile (value: Entities) : Entities = value

let df (value: Entities) : Entities = dockerfile value

let plain (value: string) : Entity = Plain value

let br = plain empty

let part (value: Entities) : Entity = Subpart value

let (!&) (value: Entities) : Entity = part value

[<RequireQualifiedAccess>]
module Dockerfile =

    let render (value: Entities) : string =
        let rec renderInstruction (instr: InstructionType) : string =
            match instr with
            | Simple s ->
                if s.Name = "#" then
                    $"%s{s.Name} %s{s.Value}"
                else
                    print s.Name s.Value

            | SimpleQuoted s -> print s.Name (quote s.Value)

            | List l -> print l.Name (printList l.Elements.Collection)

            | KeyValue kv -> print kv.Name (printKVQ kv.Key kv.Value)

            | KeyValueList l ->
                str {
                    l.Name
                    ws

                    l.Elements
                    |> Seq.map (fun e -> printKVQ e.Key e.Value)
                    |> String.concat eol_slash

                    eol
                }

            | From f ->
                str {
                    "FROM"
                    printParameterQ "platform" f.Platform
                    ws
                    f.Image

                    if f.Name.IsSome then
                        $" AS %s{f.Name.Value}"

                    eol
                }

            | Run r ->
                seq {
                    for mount in r.Mounts do
                        mount.Params
                        |> Seq.map (fun p -> printKV p.Key p.Value)
                        |> Seq.append [
                            printKV "type" (mount.Name.ToString().ToLower())
                        ]
                        |> String.concat ","
                        |> sprintf "--mount=%s"

                    if r.Network.IsSome then
                        $"--network=%s{(nameof r.Network.Value).ToLower()}"

                    if r.Security.IsSome then
                        $"--security=%s{(nameof r.Network.Value).ToLower()}"

                    for arg in r.Arguments do
                        arg
                }
                |> String.concat eol_slash
                |> sprintf "RUN%s%s%s" eol_slash
                <| eol

            | Add a ->
                str {
                    "ADD"
                    printFlag "link" a.Link

                    if a.KeepGitDir then
                        " --keep-git-dir=true"

                    printParameter "chmod" a.Chmod
                    printParameter "chown" a.Chown
                    printParameter "checksum" a.Checksum
                    ws
                    printList a.Elements.Collection

                    eol
                }

            | Copy cp ->
                str {
                    "COPY"
                    printFlag "link" cp.Link
                    printParameter "from" cp.From
                    printParameter "chmod" cp.Chmod
                    printParameter "chown" cp.Chown
                    ws
                    printList cp.Elements.Collection

                    eol
                }

            | Healthcheck hc ->            
                seq {
                    "HEALTCHECK"

                    for opt in hc.Options do
                        printParameter opt.Key (Some opt.Value)

                    "CMD"
                    printList hc.Instructions.Collection
                }
                |> String.concat ws
                |> sprintf "%s%s"
                <| eol

            | Onbuild onb ->
                seq { onb.Instruction }
                |> renderSubpart
                |> sprintf "ONBUILD %s%s"
                <| eol

        and renderSubpart (part: Entities) : string =
            part
            |> Seq.map (fun instr ->
                match instr with
                | Plain t -> t
                | Instruction i -> renderInstruction i
                | Subpart sp -> sp |> renderSubpart |> trim
            )
            |> String.concat eol

        value |> renderSubpart |> trim

    let toFile (path: string) (text: string) = File.WriteAllText (path, text)

    let fromFile (path: string) : Entities =
        if not (File.Exists path) then
            raise (ArgumentException("File not found", nameof path))
        
        seq {
            for line in File.ReadLines path do
                plain line
        }
