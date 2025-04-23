[<AutoOpen>]
module Tuffenuff.Dockerfile

open System
open System.IO
open Tuffenuff.Domain.Types
open Tuffenuff.String
open Tuffenuff.StringCE

type private Entities = Entity seq

/// <summary>
/// Form sequence of Dockerfile entities.
/// </summary>
/// <param name="value">Sequence of entities.</param>
/// <seealso cref="df"/>
let dockerfile (value : Entities) : Entities = value

/// <summary>
/// Form sequence of Dockerfile entities.
/// </summary>
/// <param name="value">Sequence of entities.</param>
/// <seealso cref="dockerfile"/>
let df (value : Entities) : Entities = dockerfile value

/// <summary>
/// Insert plain text line into Dockerfile.
/// </summary>
/// <param name="value">Text to insert.</param>
let plain (value : string) : Entity = Plain value

/// <summary>
/// Insert empty line into Dockerfile.
/// </summary>
let br = plain empty

/// <summary>
/// Insert sequence of Dockerfile entities as part of Dockerfile.
/// </summary>
/// <param name="value">Sequence of entities.</param>
/// <seealso cref="(!&)"/>
let part (value : Entities) : Entity = Subpart value

/// <summary>
/// Insert sequence of Dockerfile entities as part of Dockerfile.
/// </summary>
/// <param name="value">Sequence of entities.</param>
/// <seealso cref="part"/>
let (!&) (value : Entities) : Entity = part value

/// <summary>
/// Module of functions to work with Dockerfile entities.
/// </summary>
[<RequireQualifiedAccess>]
module Dockerfile =

    /// <summary>
    /// Convert sequence of Dockerfile entities to human-readable Dockerfile string.
    /// </summary>
    /// <param name="value">Sequence of entities to convert.</param>
    /// <returns>Dockerfile string.</returns>
    let render (value : Entities) : string =
        let rec renderInstruction (instr : InstructionType) : string =
            match instr with
            | Simple s when s.Name = "#" && s.Value = empty -> empty

            | Simple s when s.Name = "#" -> $"%s{s.Name}%s{ws}%s{s.Value}"

            | Simple s -> print s.Name s.Value

            | SimpleQuoted s -> s.Value |> quote |> print s.Name

            | List l -> l.Elements.Collection |> printList |> print l.Name

            | KeyValue kv -> printKVQ kv.Key kv.Value |> print kv.Name

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
                            printKV "type" (mount.Name.ToString().ToLower ())
                        ]
                        |> String.concat ","
                        |> sprintf "--mount=%s"

                    if r.Network.IsSome then
                        $"--network=%s{r.Network.Value.ToString().ToLower ()}"

                    if r.Security.IsSome then
                        $"--security=%s{r.Security.Value.ToString().ToLower ()}"

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
                seq { onb.Instruction } |> renderSubpart |> sprintf "ONBUILD %s%s" <| eol

        and renderSubpart (part : Entities) : string =
            part
            |> Seq.map (fun instr ->
                match instr with
                | Plain t -> t
                | Instruction i -> renderInstruction i
                | Subpart sp -> sp |> renderSubpart |> trim
            )
            |> String.concat eol

        value |> renderSubpart |> trim

    /// <summary>
    /// Creates a new file, writes the Dockerfile string to the file, and then closes the
    /// file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">Path to the target file.</param>
    /// <param name="text">Dockerfile string.</param>
    let toFile (path : string) (text : string) = File.WriteAllText (path, text)

    /// <summary>
    /// Reads entities from Dockerfile.
    /// </summary>
    /// <param name="path">Path to the Dockerfile.</param>
    let fromFile (path : string) : Entities =
        if not (File.Exists path) then
            raise (ArgumentException ("File not found", nameof path))

        seq {
            for line in File.ReadLines path do
                plain line
        }
