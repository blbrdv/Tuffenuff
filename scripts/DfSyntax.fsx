(*
    Generates module with dockerfile syntax versions from 
    https://hub.docker.com/r/docker/dockerfile/tags
*)

#r "nuget: FsHttp, 15.0.1"
#r "nuget: FParsec, 1.1.1"

let args = fsi.CommandLineArgs

if args.Length <> 2 then
    eprintf "Output file target must be provided, "
    eprintfn "e.g. 'dotnet fsi <this script file> <path to output file>'"
    failwith "One argument expected"
    
open System
open System.Text
open System.IO

let target =
    [| Environment.CurrentDirectory ; args[1] |]
    |> Path.Combine
    |> Path.GetFullPath

let (<~|) (builder : StringBuilder) (text : string) = builder.Append(text)
let (<<|) (builder : StringBuilder) (text : string) = builder.AppendLine(text)

let (<~|!) (builder : StringBuilder) (text : string) = builder <~| text |> ignore
let (<<|!) (builder : StringBuilder) (text : string) = builder <<| text |> ignore

module Version =
        
    type SemVer =
        struct
            val Major: uint8
            val Minor: uint8 option
            val Patch: uint8 option
            val Prerelease: string option
            val Name: string
            val Raw: string
            
            new (value: uint8 * (uint8 * uint8 option) option * string option) =
                let maj, rest, pre = value
                
                let mutable min: uint8 option = None
                let mutable pat: uint8 option = None
                
                let name = StringBuilder($"v{maj}")
                let raw = StringBuilder(maj.ToString())
                
                match rest with
                | Some (m, p) ->
                    min <- Some m
                    name <~|! $"_{m}"
                    raw <~|! $".{m}"
                    
                    match p with
                    | Some v ->
                        pat <- Some v
                        name <~|! $"_{v}"
                        raw<~|! $".{v}"
                    | None -> ()
                    
                | None -> ()
                
                if pre.IsSome then
                    name <~|! $"_{pre.Value}"
                    raw <~|! $"-{pre.Value}"
                
                {
                    Major = maj;
                    Minor = min;
                    Patch = pat;
                    Prerelease = pre;
                    Name = name.ToString();
                    Raw = raw.ToString()
                }
        end
    
    type Version =
        | Tag of string
        | Value of SemVer
        
        member this.Name with get() =
            match this with
            | Tag t -> t
            | Value v -> v.Name
        
        member this.Raw with get() =
            match this with
            | Tag t -> t
            | Value v -> v.Raw
    
    module private Parsing =
        open FParsec
        
        let zero =
            pchar '0'
            >>= (fun _ -> preturn Byte.MinValue)
            
        let nonZero =
            let parser =
                satisfy (fun  c -> '1' <= c && c <= '9') .>>.
                many digit
            fun stream ->
                let reply = parser stream
                if reply.Status = Ok then
                    let result = reply.Result
                    
                    let c1, crest = result
                    let rest = crest |> Array.ofList |> String
                    
                    try
                        Reply( uint8 $"%c{c1}%s{rest}" )
                    with
                    | :? OverflowException ->
                        let len = 1 + crest.Length
                        stream.Skip(-len)
                        Reply(
                            FatalError,
                            messageError "Value is too small or too large for number"
                        )
                else
                    Reply(reply.Status, reply.Error)
        
        let parseNumber: Parser<uint8, unit> =
            (
                attempt zero <|>
                nonZero
            ) <?> "Number (unsigned)"
                    
        let parseNumberPart =
            pchar '.' >>.
            parseNumber
        
        let parseTag = many1Chars asciiLetter <?> "Tag (ascii word)"
        
        let parseLabel =
            pchar '-' >>.
            parseTag
            
        let parseMajor =
            parseNumber <?> "Major version"
            
        let parseMinor =
            parseNumberPart <?> "Minor version"
            
        let parsePatch =
            parseNumberPart <?> "Patch version"
            
        let parseRest =
            (opt (
                parseMinor .>>.
                opt parsePatch
            ))
            
        let parsePrerelease =
            (opt parseLabel <?> "Label (ascii word)")
            
        let parseSemVer =
            pipe3
                parseMajor
                parseRest
                parsePrerelease
                (fun a b c -> (a, b, c))
        
        let parseVersion: Parser<Version, _> =
            (
                attempt parseTag <?> "Tag (ascii word)"
                >>= (fun x -> x |> Tag |> preturn)
            ) <|>
            (
                parseSemVer <?> "SemVer"
                >>= (fun x -> x |> SemVer |> Value |> preturn)
            )
            .>> eof
            
        exception ParsingException of string
            
        let parse (value : string) =
            match runParserOnString parseVersion () "docker image name" value with
            | ParserResult.Success(result, _, _) -> result
            | ParserResult.Failure(msg, _, _) ->
                eprintfn $"ParsingException: %s{msg}"
                raise (ParsingException(msg))
            
    module private Sorting =        
        let sortMaj (a: uint8, b: uint8) =
            if a > b then
                -1
            elif a < b then
                1
            else
                0
            
        let sortVer (a: uint8 option, b: uint8 option) (prev: int) =
            if prev = 0 then
                match (a, b) with
                | Some av, Some bv -> sortMaj (av, bv)
                | Some _, None -> -1
                | None, Some _ -> 1
                | _ -> 0
            else
                prev
                
        let sortString (a: string, b: string) =
            String.Compare(a,b) * -1
        
        let sortPre (a: string option, b: string option) (prev: int) =
            if prev = 0 then
                match (a, b) with
                | Some av, Some bv -> sortString (av, bv)
                | Some _, None -> -1
                | None, Some _ -> 1
                | _ -> 0
            else
                prev
        
        let sort (a: SemVer, b: SemVer) =
            sortMaj (a.Major, b.Major)
            |> sortVer (a.Minor, b.Minor)
            |> sortVer (a.Patch, b.Patch)
            |> sortPre (a.Prerelease, b.Prerelease)
            
    let parse (str : string) = Parsing.parse str
    
    let sort (a: Version) (b: Version) =
        match (a, b) with
        | Tag at, Tag bt -> Sorting.sortString(at, bt)
        | Value av, Value bv -> Sorting.sort(av, bv)
        | Tag _, _ -> -1
        | _, Tag _ -> 1

module DockerHub =
    open FsHttp
    open System.Text.Json.Serialization
    open Version

    type Tag = 
        {
            Name : string
            [<JsonPropertyName "content_type">]
            ContentType : string
        }

    type Page = 
        {
            Next : string option
            Results : Tag seq
        }

    let get url =
        http {
            GET url
        }
        |> Request.send
        |> Response.assert2xx
        |> Response.deserializeJson<Page>

    let getAllTags() =
        let mutable result = Seq.empty
        let page = "https://hub.docker.com/v2/namespaces/docker/repositories/dockerfile/tags?page=1"

        let rec getTags url =
            let data = get url

            result <- 
                data.Results
                |> Seq.filter (fun tag ->
                    (not ("latest".Equals(tag.Name)))
                    && "image".Equals(tag.ContentType)
                )
                |> Seq.append result

            if data.Next.IsSome then
                getTags(data.Next.Value)

        getTags(page)

        result
        |> Seq.map (fun tag -> parse tag.Name)
        |> Seq.sortWith sort
        |> Seq.readonly

module Codegen =
    open Version
    
    let genModule (values : Version seq) =
        StringBuilder()
        <<| "(*"
        <<| "    Generated with 'scripts/DfSyntax.fsx'"
        <<| "*)"
        <<| "module Tuffenuff.DSL.Dockerfile"
        <<| ""
        <~| (
            values
            |> Seq.map (fun v ->
                
                StringBuilder()
                <<| "///<summary>"
                <<| "///Sets the version of docker syntax to"
                <~| "///<c>docker/dockerfile:"
                <~| v.Raw
                <<| "</c>"
                <<| "///</summary>"
                <<| "///<example>"
                <~| "///<c>"
                <~| v.Name
                <~| "</c> -> <c># syntax=docker/dockerfile:"
                <~| v.Raw
                <<| "</c>"
                <<| "///</example>"
                <<| "///<seealso cref=\"Tuffenuff.DSL.Comments.syntax\" />"
                <~| "let "
                <~| v.Name
                <~| " = syntax \"docker/dockerfile:"
                <~| v.Raw
                <<| "\""
                |> string
                
            )
            |> String.concat Environment.NewLine
        )
        |> string

let fileContent =
    DockerHub.getAllTags()
    |> Codegen.genModule

File.WriteAllText(target, fileContent)

printfn ""
printfn "SUCCESS"
printfn $"File '%s{target}' updated"
