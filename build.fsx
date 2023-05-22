#if FAKE
#r "paket:
nuget FSharp.Core 6.0.0
nuget Microsoft.Build 16.10.0
nuget Microsoft.Build.Framework 16.10.0
nuget Microsoft.Build.Tasks.Core 16.10.0
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#endif
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

Target.create "Clean" (fun _ -> !! "src/**/bin" ++ "src/**/obj" |> Shell.cleanDirs)

Target.create "Build" (fun _ -> !! "src/**/*.*proj" |> Seq.iter (DotNet.build id))

Target.create "RunTests" (fun _ -> 
    !! "tests/DockerfileDSL.FSharp.Tests2/"
    |> Seq.iter (
        DotNet.test (fun opt -> { opt with Configuration = DotNet.BuildConfiguration.Release })
    )
)

"Clean" 
    ==> "Build"
    ==> "RunTests"

Target.runOrDefault "RunTests"
