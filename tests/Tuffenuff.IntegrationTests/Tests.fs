module Tests.Integration

open FSharpPlus
open System.IO
open Expecto
open Fli

[<Tests>]
let tests =
    [| __SOURCE_DIRECTORY__; ".."; ".."; "examples" |]
    |> Path.Combine
    |> Path.GetFullPath
    |> Directory.GetFiles
    |> Array.where (fun file -> file.EndsWith ".fsx" && not (file.Contains "part"))
    |> Array.map (fun file ->
        let filename = 
            file
            |> Path.GetFileName
            |> String.split [ "." ]
            |> Seq.head

        testCase $"'{filename}' test"
        <| fun () ->
            let examplesPath = Path.GetDirectoryName file
            let resultName = $"Dockerfile.{filename}"

            let result =
                cli {
                    Exec "dotnet"
                    Arguments [ "fsi" ; file ]
                }
                |> Command.execute

            Expect.isTrue (result.ExitCode = 0)
            <| $"Example executed with error:\n{Output.toError result}\n"

            let actual = 
                [| examplesPath; resultName |]
                |> Path.Combine
                |> File.ReadAllText
            let expected =
                [| "expected"; resultName |]
                |> Path.Combine
                |> File.ReadAllText

            Expect.equal actual expected
            <| $"{filename} example must generate correct dockerfile"
    )
    |> List.ofArray 
    |> testList "Integration tests"
