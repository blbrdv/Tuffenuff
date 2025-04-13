(*
    Generate file with F# module with Dockerfile syntax versions from 
    https://hub.docker.com/u/docker
*)

#r "nuget: FsHttp, 15.0.1"

let private args = fsi.CommandLineArgs

if args.Length <> 4 then
    eprintf "Output file path, repository name and namespace must be provided"
    failwith "3 arguments expected"

let private targetFilePath = args[1]
let private repositoryName = args[2]
let private namespace' = args[3]
let private tempFilePath = $"%s{targetFilePath}.tmp"
let private thisFileName = __SOURCE_FILE__

module private Version =
    open System
        
    type Version =
        struct
            val Name: string
            val Raw: string
            val UpdatedAt: DateTime
            
            new (raw: string, updatedAt: DateTime) =
                let escaped = raw.Replace('.', '_').Replace('-', '_')
                let name =
                    if not (Char.IsLetter escaped[0]) then
                        $"v%s{escaped}"
                    else
                        escaped
                
                {
                    Name = name;
                    Raw = raw;
                    UpdatedAt = updatedAt 
                }
        end
            
    let parse (str : string, updatedAt : DateTime) =
        Version(str, updatedAt)
        
    let sort (left : Version) (right : Version) =
        let firstResult = DateTime.Compare(left.UpdatedAt, right.UpdatedAt) * -1
        
        if firstResult = 0 then
            String.Compare(left.Raw, right.Raw) * -1
        else
            firstResult

module private DockerHub =
    open System
    open System.Text.Json
    open FsHttp
    open Version

    let private get url =
        http {
            GET url
        }
        |> Request.send
        |> Response.assert2xx
        |> Response.toJson
        |> (fun json ->
            json?next.GetString(),
            seq {
                let mutable e = json?results.EnumerateArray().GetEnumerator()
                
                while e.MoveNext() do
                    yield e.Current
            }
        )
        |> (fun (page, images) ->
            page,
            seq {
                for image in images do                    
                    let name = image?name.GetString()
                    let updatedAt = image?last_updated.GetDateTime()
                    
                    let mutable type' = JsonElement()
                    
                    if (
                       image.TryGetProperty("content_type", &type') &&
                       "image".Equals(type'.GetString()) &&
                       not ("latest".Equals(name))
                    ) then
                        yield (name, updatedAt)
            }
        )

    let getAllTags =
        let mutable result = Seq.empty
        let page = $"https://hub.docker.com/v2/namespaces/docker/repositories/%s{repositoryName}/tags?page=1&page_size=100"

        let rec getTags (url : string) =
            let nextPage, versions = get url

            result <- Seq.append result versions

            if not (String.IsNullOrEmpty nextPage) then
                getTags nextPage

        getTags page

        result
        |> Seq.map parse
        |> Seq.sortWith sort
        |> Seq.readonly

module private Generate =
    open System
    open System.Text
    open System.IO
    open Version
    
    [<Literal>]
    let private eof = "\r\n"
    
    let inline private (<~|) (stream : FileStream) (text : string) =
        text
        |> Encoding.UTF8.GetBytes
        |> stream.Write
        
        stream
        
    let inline private (<<|) (stream : FileStream) (text : string) =
        let line =
            [ text ; eof ]
            |> String.Concat
        
        stream <~| line
        
    let toFile (values : Version seq) =        
        try
            let lastIndex = (values |> Seq.length) - 1
            
            use stream = File.Create(tempFilePath, 0, FileOptions.WriteThrough)
            
            stream
            <<| "(*"
            <~| "    Generated with 'scripts/"
            <~| thisFileName
            <<| "'"
            <<| "*)"
            <~| eof
            
            <<| "/// <summary>"
            <~| "/// List of Dockerfile syntax versions from '"
            <~| repositoryName
            <<| "' repository."
            <<| "/// </summary>"
            
            <<| "[<RequireQualifiedAccess>]"
            <~| "module Tuffenuff.DSL."
            <<| namespace'
            <~| eof
            
            |> ignore
            
            values
            |> Seq.iteri (fun i v ->
                stream
                <<| "/// <summary>"
                <<| "/// Sets the version of docker syntax to"
                <~| "/// <c>docker/"
                <~| repositoryName
                <~| ":"
                <~| v.Raw
                <<| "</c>"
                <<| "/// </summary>"
                
                <<| "/// <example>"
                <~| "/// <c>"
                <~| v.Name
                <<| "</c> ->"
                <~| "/// <c># syntax=docker/"
                <~| repositoryName
                <~| ":"
                <~| v.Raw
                <<| "</c>"
                <<| "/// </example>"
                
                <<| "/// <remarks>"
                <~| "/// Last updated at "
                <<| v.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                <<| "/// </remarks>"
                
                <<| "/// <seealso cref=\"Tuffenuff.DSL.Comments.syntax\" />"
                
                <~| "let "
                <~| v.Name
                <~| " = syntax \"docker/"
                <~| repositoryName
                <~| ":"
                <~| v.Raw
                <<| "\""
                
                |> ignore
                
                if i <> lastIndex then
                    stream <~| eof |> ignore
            )
            
        finally            
            File.Move(tempFilePath, targetFilePath, true)

DockerHub.getAllTags
|> Generate.toFile 
