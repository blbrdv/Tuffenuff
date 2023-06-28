namespace Tuffenuff

[<RequireQualifiedAccess>]
module Image =
    open System.IO
    open Tuffenuff.Domain.Entity
    open Tuffenuff.String
    open Tuffenuff.StringCE
    open Tuffenuff.Domain.Image


    let render (img : Image) =
        let rec renderInstruction instr =
            match instr with
            | Simple s -> print s.Name s.Value

            | SimpleQuoted s -> print s.Name (quote s.Value)

            | List l -> print l.Name (printList l.Elements)

            | KeyValue kv -> print kv.Name (printKVQ kv.Key kv.Value)

            | KeyValueList l ->
                str {
                    l.Name
                    " "

                    l.Elements
                    |> Seq.map (fun e -> printKVQ e.Key e.Value)
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

            // | Run r ->
            //     seq {
            //         for mount in r.Mounts do
            //             mount.Params
            //             |> Seq.map (fun p -> printKV p.Key p.Value)
            //             |> Seq.append [
            //                 printKV "type" (mount.Name.ToString().ToLower ())
            //             ]
            //             |> String.concat ","
            //             |> sprintf "--mount=%s"

            //         if r.Network.IsSome then
            //             sprintf "--network=%s" ((nameof r.Network.Value).ToLower ())

            //         if r.Security.IsSome then
            //             sprintf "--security=%s" ((nameof r.Network.Value).ToLower ())

            //         for arg in r.Arguments do
            //             arg
            //     }
            //     |> String.concat eol_slash
            //     |> sprintf "RUN%s%s" eol_slash

            // | Add a ->
            //     str {
            //         "ADD"
            //         printFlag "link" a.Link

            //         if a.KeepGitDir then
            //             sprintf " --keep-git-dir=true"

            //         printParameter "chmod" a.Chmod
            //         printParameter "chown" a.Chown
            //         printParameter "checksum" a.Checksum
            //         " "
            //         printList a.Elements.Collection
            //     }

            // | Copy cp ->
            //     str {
            //         "COPY"
            //         printFlag "link" cp.Link
            //         printParameter "from" cp.From
            //         printParameter "chmod" cp.Chmod
            //         printParameter "chown" cp.Chown
            //         " "
            //         printList cp.Elements.Collection
            //     }

            // | Healthcheck hc ->
            //     seq {
            //         "HEALTCHECK"

            //         for opt in hc.Options do
            //             printParameter opt.Key (Some opt.Value)

            //         "CMD"
            //         printList hc.Instructions.Collection
            //     }
            //     |> String.concat " "

            | Onbuild onb -> seq { onb.Instruction } |> r |> sprintf "ONBUILD %s"

        and r sub =
            sub
            |> Seq.map (fun instr ->
                match instr with
                | Plain t -> t
                | Instruction i -> renderInstruction i
                | Subpart s -> r s |> trim
            )
            |> String.concat eol

        r img.Lines |> trim |> sprintf "%s%s" <| eol


    let toFile (path : string) (text : string) = File.WriteAllText (path, text)


    let fromFile (path : string) =
        seq {
            for line in File.ReadLines (path) do
                Plain line
        }
        |> Image
