module Tests.Integration

open System.Diagnostics
open System.Threading.Tasks
open System.IO
open Expecto


type CommandResult = { 
  ExitCode: int; 
  StandardOutput: string;
  StandardError: string 
}

// https://alexn.org/blog/2020/12/06/execute-shell-command-in-fsharp/
let executeCommand executable args =
    async {
        let startInfo = ProcessStartInfo()
        startInfo.FileName <- executable
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        startInfo.CreateNoWindow <- true

        for a in args do
            startInfo.ArgumentList.Add(a)

        use p = new Process()
        p.StartInfo <- startInfo
        p.Start() |> ignore

        let outTask = Task.WhenAll([|
            p.StandardOutput.ReadToEndAsync();
            p.StandardError.ReadToEndAsync()
        |])

        do! p.WaitForExitAsync() |> Async.AwaitTask
        let! out = outTask |> Async.AwaitTask
        return {
            ExitCode = p.ExitCode;
            StandardOutput = out.[0];
            StandardError = out.[1]
        }
    }

let fsi path = 
    executeCommand "dotnet" [ "fsi"; path ]
    |> Async.RunSynchronously

let testFuncs =
    Directory.GetFiles(
        Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, @"..\..\examples"))
    )
    |> Seq.where (fun file -> file.EndsWith(".fsx") && not (file.Contains("part")))
    |> Seq.map (fun file ->
        let filename = Path.GetFileName(file).Split('.')[0]

        testCase $"'{filename}' test" <| fun () ->
            let examplesPath = Path.GetDirectoryName(file)
            let resultName = $"Dockerfile.{filename}"

            let result = fsi file

            Expect.isTrue (result.ExitCode = 0)
            <| $"Example executed with error:\n{result.StandardError}"

            let actual =
                File.ReadAllText(Path.Combine(examplesPath, resultName))
            let expected = 
                File.ReadAllText(Path.Combine("expected", resultName))
            
            Expect.equal actual expected
            <| $"{filename} example must generate correct dockerfile"
    )
    |> List.ofSeq


[<Tests>]
let tests =
    testList "Integration tests" testFuncs
