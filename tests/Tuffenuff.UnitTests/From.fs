module Tests.From

open System
open Expecto
open Tuffenuff.String
open Tuffenuff.DSL

[<Tests>]
let tests =
    testList "FROM instruction tests" [
        testCase "from 'scratch' test"
        <| fun _ ->
            Expect.equal
                (fresh |> render)
                $"FROM scratch%s{eol}"
                "String must contain\n\
                  1. 'FROM' keyword\n\
                  2. whitespace\n\
                  3. 'scratch' keyword\n\
                  4. EOL\n\n"

        testCase "from 'alpine:latest' test"
        <| fun _ ->
            Expect.equal
                (from "alpine:latest" |> render)
                $"FROM alpine:latest%s{eol}"
                "String must contain\n\
                  1. 'FROM' keyword\n\
                  2. whitespace\n\
                  3. provided image reference\n\
                  4. EOL\n\n"

        testCase "from empty test"
        <| fun _ ->
            Expect.throwsT<ArgumentException>
                (fun _ -> from "" |> ignore)
                "Image reference can not be null or empty"

        testCase "stage 'alpine:latest' 'builder' test"
        <| fun _ ->
            Expect.equal
                (stage "alpine:latest" "builder" |> render)
                $"FROM alpine:latest AS builder%s{eol}"
                "String must contain\n\
                  1. 'FROM' keyword\n\
                  2. whitespace\n\
                  3. image reference from first argument\n\
                  4. whitespace\n\
                  5. 'AS' keyword\n\
                  6. whitespace\n\
                  7. stage name from second argument\n\
                  8. EOL\n\n"

        testCase "stage empty 'builder' test"
        <| fun _ ->
            Expect.throwsT<ArgumentException>
                (fun _ -> stage "" "builder" |> ignore)
                "Image reference can not be null or empty"

        testCase "stage 'alpine:latest' empty test"
        <| fun _ ->
            Expect.throwsT<ArgumentException>
                (fun _ -> stage "alpine:latest" "" |> ignore)
                "Stage name can not be null or empty"

        testCase "based 'python:3.13.3' as 'foo' platform 'linux/amd64' test"
        <| fun _ ->
            Expect.equal
                (based "python:3.13.3" {
                    as' "foo"
                    platform "linux/amd64"
                 }
                 |> render)
                $"FROM --platform=\"linux/amd64\" python:3.13.3 AS foo%s{eol}"
                "String must contain\n\
                  1. 'FROM' keyword\n\
                  2. whitespace\n\
                  3. two minus characters\n\
                  4. key-value with\n\
                    4.1. 'platform' keyword\n\
                    4.2. quoted target platform from argument of 'platform' command\n\
                  5. whitespace\n\
                  6. image reference from first argument\n\
                  7. whitespace\n\
                  8. 'AS' keyword\n\
                  9. whitespace\n\
                  10. stage name from argument of 'as'' command\n\
                  11. EOL\n\n"

        testCase "based 'python:3.13.3' as 'foo' test"
        <| fun _ ->
            Expect.equal
                (based "python:3.13.3" { as' "foo" } |> render)
                $"FROM python:3.13.3 AS foo%s{eol}"
                "String must contain\n\
                  1. 'FROM' keyword\n\
                  2. whitespace\n\
                  3. image reference from first argument\n\
                  4. whitespace\n\
                  5. 'AS' keyword\n\
                  6. whitespace\n\
                  7. stage name from argument of 'as'' command\n\
                  8. EOL\n\n"

        testCase "based 'python:3.13.3' platform 'linux/amd64' test"
        <| fun _ ->
            Expect.equal
                (based "python:3.13.3" { platform "linux/amd64" } |> render)
                $"FROM --platform=\"linux/amd64\" python:3.13.3%s{eol}"
                "String must contain\n\
                  1. 'FROM' keyword\n\
                  2. whitespace\n\
                  3. two minus characters\n\
                  4. key-value with\n\
                    4.1. 'platform' keyword\n\
                    4.2. quoted target platform from argument of 'platform' command\n\
                  5. whitespace\n\
                  6. image reference from first argument\n\
                  7. EOL\n\n"

        testCase "base empty test"
        <| fun _ ->
            Expect.throwsT<ArgumentException>
                (fun _ -> based "" { () } |> ignore)
                "Stage name can not be null or empty"

        testCase "base 'python:3.13.3' as empty test"
        <| fun _ ->
            Expect.throwsT<ArgumentException>
                (fun _ -> based "python:3.13.3" { as' "" } |> ignore)
                "Alias can not be null or empty"

        testCase "base python:3.13.3' as 'foo' platform empty test"
        <| fun _ ->
            Expect.throwsT<ArgumentException>
                (fun _ -> based "python:3.13.3" { platform "" } |> ignore)
                "Platform can not be empty"
    ]
