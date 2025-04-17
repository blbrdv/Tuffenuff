[<RequireQualifiedAccess>]
module CICD.Targets

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open CICD.Options
open CICD.Paths

let init () =

    // ** Targets **

    Target.description "Check if code files need formatting"
    Target.create
        "CheckFormat"
        (fun _ ->
            let result = DotNet.exec dotnetOptions "fantomas" "--recurse --check ."

            if result.ExitCode = 0 then
                Trace.log "No files need formatting"
            elif result.ExitCode = 99 then
                failwith "Some files need formatting, check output for more info"
            else
                Trace.traceError $"Errors while formatting: %A{result.Errors}"
        )

    Target.description "Format code files"
    Target.create "Format" (fun _ -> DotNet.exec dotnetOptions "fantomas" "." |> ignore)

    Target.description "Delete \"bin\" and \"obj\" directories"
    Target.create
        "Clean"
        (fun _ ->
            buildDirs
            |> (fun value ->
                value |> Seq.iter (fun dir -> Trace.log $"Deleting %s{dir}")

                value
            )
            |> Shell.deleteDirs
        )

    Target.description "Build Tuffenuff"
    Target.create
        "Build"
        (fun _ ->
            let fullPath = System.IO.Path.GetFullPath srcProjFile

            Trace.traceImportant fullPath
            DotNet.build id fullPath
        )

    Target.description "Tests scripts in examples directory"
    Target.create
        "IntegrationTests"
        (fun _ -> DotNet.test testOptions integrationTestsProjFile)

    Target.description "Tests DSL"
    Target.create "UnitTests" (fun _ -> DotNet.test testOptions unitTestsProjFile)

    Target.description "Push library to Nuget"
    Target.create
        "Release"
        (fun _ ->
            let key =
                match nugetApiKey with
                | Some value -> value
                | None ->
                    // TODO raise
                    failwith "todo"

            DotNet.nugetPush
                (fun defaults ->
                    //traceOptions "Nuget push options"
                    { defaults with
                        PushParams =
                            { defaults.PushParams with
                                DisableBuffering = true
                                Source = Some "https://api.nuget.org/v3/index.json"
                                ApiKey = Some key
                            }
                    }
                )
                packageFile
        )

    Target.description
        "Generates module with syntax versions from \"dockerfile\" repository"
    Target.create
        "GenerateSyntaxVersions"
        (fun _ ->
            DockerfileSyntaxVersions.generate syntaxVersionFile "dockerfile" "Syntax"
        )

    Target.description
        "Generates module with syntax versions from \"dockerfile-upstream\" repository"
    Target.create
        "GenerateUpstreamSyntaxVersions"
        (fun _ ->
            DockerfileSyntaxVersions.generate
                syntaxUpstreamVersionFile
                "dockerfile-upstream"
                "UpstreamSyntax"
        )

    Target.description "Run \"UnitTests\" then \"IntegrationTests\""
    Target.create "AllTests" ignore

    Target.description
        "Run \"GenerateSyntaxVersions\" then \"GenerateUpstreamSyntaxVersions\""
    Target.create "GenerateAllSyntaxVersions" ignore

    // ** Dependencies **

    // TODO check for build server

    "Clean" ==> "Build" |> ignore

    "Build" ==> "UnitTests" |> ignore

    "Build" ==> "IntegrationTests" |> ignore

    "Build" ==> "Release" |> ignore

    "UnitTests" ==> "IntegrationTests" ==> "AllTests" |> ignore

    "GenerateSyntaxVersions"
    ==> "GenerateUpstreamSyntaxVersions"
    ==> "GenerateAllSyntaxVersions"
    |> ignore
