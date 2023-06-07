#if FAKE
#r "paket:
nuget FSharp.Core 6.0.0
nuget Microsoft.Build 16.10.0
nuget Microsoft.Build.Framework 16.10.0
nuget Microsoft.Build.Tasks.Core 16.10.0
nuget Fake.DotNet.NuGet
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#endif
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.DotNet.NuGet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

/////////////////////////////////////////////////////////////////////////////////////////

let projName = "Tuffenuff"
let testProjName = sprintf "%s.Tests" projName
let projDir = "src" @@ projName
let projFile = projDir @@ (sprintf "%s.fsproj" projName)
let packageFile = projDir @@ "**/*.nupkg"
let projTestFile = "tests" @@ testProjName @@ (sprintf "%s.fsproj" testProjName)
let buildDirs = !! "**/bin" ++ "**/obj"

/////////////////////////////////////////////////////////////////////////////////////////

Target.initEnvironment ()

Target.create "Clean" (fun _ ->  Shell.deleteDirs buildDirs)

Target.create "Build" (fun _ -> DotNet.build id projFile)

Target.create "RunTests" (fun _ -> 
    DotNet.test (fun opt -> { opt with Logger = Some "console;verbosity=detailed" })
    <| projTestFile
)

Target.create "Release" (fun _ ->
    let nugetApiKey = Environment.environVarOrFail "key"
    let setNugetPushParams (defaults : NuGet.NuGetPushParams) =
        { defaults with
            DisableBuffering = true
            Source = Some "https://api.nuget.org/v3/index.json"
            ApiKey = Some nugetApiKey
        }
    let setParams (defaults : DotNet.NuGetPushOptions) =
        { defaults with
            PushParams = setNugetPushParams defaults.PushParams
        }
        
    DotNet.nugetPush setParams
    <| packageFile
)

"Clean" 
    ==> "Build"
    ==> "RunTests"

"Clean"
    ==> "Build"
    ==> "Release"

Target.runOrDefault "RunTests"
