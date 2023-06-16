(*
    Generates module with dockerfile syntax versions from 
    https://hub.docker.com/r/docker/dockerfile/tags
*)

#r "nuget: FsHttp"


module DockerHub =
    open FsHttp
    open FsHttp.CSharp


    type Tag = 
        {
            Name : string
        }


    type Page = 
        {
            Next : string option
            Results : Tag seq
        }


    let get (url) =
        http {
            GET url
        }
        |> Request.send
        |> Response.AssertOk
        |> Response.deserializeJson<Page>


    let getAllTags() =
        let mutable result = Seq.empty
        let page = "https://hub.docker.com/v2/namespaces/docker/repositories/dockerfile/tags?page=1"

        let rec getTags(url) =
            let data = get(url)

            result <- 
                data.Results
                |> Seq.map (fun tag -> tag.Name)
                |> Seq.append result

            if data.Next.IsSome then
                getTags(data.Next.Value)

        getTags(page)

        result


module Codegen =
    open System.Text


    let genModule(values) =
        let str = StringBuilder()
        let (!>) (text : string) = str.Append(text) |> ignore

        !> "(*\n    Generated with 'scripts/DfSyntax.fsx'\n*)\n"
        !> "module Tuffenuff.DSL.Dockerfile\n\n"

        values
        |> Seq.iter (fun (key, value) ->
            !> $"\n///<summary><c>{value}</c> version of docker syntax</summary>"
            !> $"\nlet {key} = syntax \"docker/dockerfile:{value}\"\n"
        )

        str.ToString()


open System

let result = 
    DockerHub.getAllTags()
    |> Seq.where (fun tag -> tag <> "latest")
    |> Seq.map (fun tag -> 
        let name = 
            if (tag |> Seq.exists Char.IsDigit) then
                "v" + tag
                    .Replace(".", "_")
                    .Replace("-", "_")
            else
                tag
        (name, tag)
    )
    |> Seq.sortByDescending (fun (k, _) -> k)
    |> Codegen.genModule

printfn "%s" result
