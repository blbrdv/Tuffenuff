module Tests.From

open Expecto
open Tuffenuff.DSL
open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections


[<Tests>]
let tests =
    testList "FROM instruction tests" [
        testCase "from scratch test" <| fun _ ->
            let expected = 
                FromInstruction.Create("scratch")
                |> From
                |> Instruction

            let actual = fresh

            Expect.equal actual expected "Image must be 'scratch'"

        testCase "from alpine:latest test" <| fun _ ->
            let expected = 
                FromInstruction.Create("alpine:latest")
                |> From
                |> Instruction

            let actual = from "alpine:latest"

            Expect.equal actual expected "Image must be 'alpine:latest'"

        testCase "stage test" <| fun _ ->
            let expected = 
                {
                    Image = "alpine:latest";
                    Name = Some "builder";
                    Platform = None
                }
                |> From
                |> Instruction

            let actual = stage "alpine:latest" "builder"

            Expect.equal actual expected 
            <| "Image must be 'alpine:latest' and named 'builder'"

        testCase "from test" <| fun _ ->
            let expected = 
                {
                    Image = "python:latest";
                    Name = Some "test";
                    Platform = Some "linux/amd64"
                }
                |> From
                |> Instruction

            let actual = 
                fromOpts "python:latest" {
                    alias "test"
                    platform "linux/amd64"
                }

            Expect.equal actual expected 
            <| "Instructions must be equal"
    ]
