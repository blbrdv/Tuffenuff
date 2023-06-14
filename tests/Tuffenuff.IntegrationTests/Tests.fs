module Tests.Integration

open System.IO
open Expecto
open Fli


let testFuncs =
    Directory.GetFiles (
        Path.GetFullPath (Path.Combine (__SOURCE_DIRECTORY__, "..", "..", "examples"))
    )
    |> Seq.where (fun file -> file.EndsWith (".fsx") && not (file.Contains ("part")))
    |> Seq.map (fun file ->
        let filename = Path.GetFileName(file).Split ('.')[0]

        testCase $"'{filename}' test"
        <| fun () ->
            let examplesPath = Path.GetDirectoryName (file)
            let resultName = $"Dockerfile.{filename}"

            let result =
                cli {
                    Exec "dotnet"
                    Arguments [ "fsi" ; file ]
                }
                |> Command.execute

            Expect.isTrue (result.ExitCode = 0)
            <| $"Example executed with error:\n{Output.toError result}\n"

            let actual = File.ReadAllText (Path.Combine (examplesPath, resultName))
            let expected = File.ReadAllText (Path.Combine ("expected", resultName))

            Expect.equal actual expected
            <| $"{filename} example must generate correct dockerfile"
    )
    |> List.ofSeq


[<Tests>]
let tests = testList "Integration tests" testFuncs
