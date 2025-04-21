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
            let result = DotNet.exec fantomasOptions "fantomas" "--recurse --check ."

            if result.ExitCode = 0 then
                Trace.log "No files need formatting"
            elif result.ExitCode = 99 then
                failwith "Some files need formatting, check output for more info"
            else
                failwith $"Errors while formatting: %A{result.Errors}"
        )

    Target.description "Format code files"
    Target.create "Format" (fun _ -> DotNet.exec fantomasOptions "fantomas" "." |> ignore)

    Target.description "Delete \"bin\" and \"obj\" directories"
    Target.create
        "Clean"
        (fun _ ->
            let targets = buildDirs |> List.ofSeq

            if targets.Length = 0 then
                Trace.trace "Nothing to delete"
            else
                targets
                |> (fun value ->
                    value |> Seq.iter (fun dir -> Trace.tracefn $"Deleting %s{dir}")

                    value
                )
                |> Shell.deleteDirs
        )

    // TODO: find easier way to add one flag to build command
    Target.description "Build Tuffenuff"
    Target.create "Build" (fun _ ->
        let buildOpts = DotNet.BuildOptions.Create() |> buildOptions
        let commonOpts = buildOpts.Common

        let args =
            [
                $"\"%s{srcProjFile}\""
                $"--configuration %A{buildOpts.Configuration}"
                "--no-dependencies"
            ]
            |> String.concat " "

        let result =
            DotNet.exec
                (fun _ -> commonOpts)
                "build"
                args

        if result.ExitCode <> 0 then
            let errorMessage =
                $"'build %s{srcProjFile}' failed with exitcode %d{result.ExitCode}."
            raise (MSBuildException(errorMessage, []))
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
                    failwith "Nuget API key must be provided via environment variables"

            DotNet.nugetPush
                (fun defaults ->
                    Trace.traceObject
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

    if BuildServer.isLocalBuild then
        Target.description "Run \"UnitTests\" then \"IntegrationTests\""
        Target.create "AllTests" ignore

        Target.description
            "Run \"GenerateSyntaxVersions\" then \"GenerateUpstreamSyntaxVersions\""
        Target.create "GenerateAllSyntaxVersions" ignore

    // ** Dependencies **

    if BuildServer.isLocalBuild then
        "Clean"
            ==> "Build"
            |> ignore

        "Build"
            ==> "UnitTests"
            |> ignore

        "Build"
            ==> "IntegrationTests"
            |> ignore

        "Build"
            ==> "Release"
            |> ignore

        "UnitTests"
            ==> "IntegrationTests"
            ==> "AllTests"
            |> ignore

        "GenerateSyntaxVersions"
            ==> "GenerateUpstreamSyntaxVersions"
            ==> "GenerateAllSyntaxVersions"
            |> ignore
