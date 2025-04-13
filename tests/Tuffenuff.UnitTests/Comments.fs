module Tests.Comments

open System
open Expecto
open Tuffenuff.DSL
open Tuffenuff.String
open Tests.Utils

[<Literal>]
let private text = "This is comment text"

[<Literal>]
let private errorMsg = "Sequence of directives can not be empty"

[<Tests>]
let tests =
    testList "comment tests" [
        testCase "comment test"
        <| fun _ ->
            Expect.equal
                (text |> comment |> render)
                "# This is comment text"
                "String must start with '# ' and contains unmodified text"

        testCase "empty comment test"
        <| fun _ ->
            Expect.equal (empty |> comment |> render) String.Empty "String must be empty"

        testCase "short comment test"
        <| fun _ ->
            Expect.equal
                (!/text |> render)
                "# This is comment text"
                "String must start with '# ' and contains unmodified text"

        testCase "custom syntax parser directive test"
        <| fun _ ->
            Expect.equal
                ("foo/bar" |> syntax |> render)
                "# syntax=foo/bar"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1 'syntax' keyword\n\
                    3.2. custom image reference from argument\n\n"

        testCase "'v1' syntax parser directive test"
        <| fun _ ->
            Expect.equal
                (Syntax.v1 |> render)
                "# syntax=docker/dockerfile:1"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1 'syntax' keyword\n\
                    3.2. first 'dockerfile' version image reference\n\n"

        testCase "'master' upstream syntax parser directive test"
        <| fun _ ->
            Expect.equal
                (UpstreamSyntax.master |> render)
                "# syntax=docker/dockerfile-upstream:master"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1 'syntax' keyword\n\
                    3.2. 'master' 'dockerfile-upstream' image reference\n\n"

        testCase "escape parser directive test"
        <| fun _ ->
            Expect.equal
                ('`' |> escape |> render)
                "# escape=`"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1. 'escape' keyword\n\
                    3.2. provided character\n\n"

        testCase "check all parser directive test"
        <| fun _ ->
            Expect.equal
                (check [ "all" ] true |> render)
                "# check=skip=all;error=true"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1. 'check' keyword\n\
                    3.2. key-value with\n\
                      3.2.1. 'skip' keyword\n\
                      3.2.2. values from first argument seperated by comma\n\
                    3.3. semicolon\n\
                    3.4. key-value with\n\
                      3.4.1. 'error' keyword\n\
                      3.4.2. value from second argument\n\n"

        testCase "check some parser directive test"
        <| fun _ ->
            Expect.equal
                (check [ "StageNameCasing" ; "FromAsCasing" ] false |> render)
                "# check=skip=StageNameCasing,FromAsCasing;error=false"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1. 'check' keyword\n\
                    3.2. key-value with\n\
                      3.2.1. 'skip' keyword\n\
                      3.2.2. values in first argument seperated by comma\n\
                    3.3. semicolon\n\
                    3.4. key-value with\n\
                      3.4.1. 'error' keyword\n\
                      3.4.2. value from second argument\n\n"

        testCase "check empty parser directive test"
        <| fun _ ->
            Expect.throwsT<ArgumentOutOfRangeException>
                (fun _ -> check [] false |> ignore)
                errorMsg

        testCase "warnAsError parser directive test"
        <| fun _ ->
            Expect.equal
                (warnAsError |> render)
                "# check=error=true"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1. 'check' keyword\n\
                    3.2. 'error=true' value\n\n"

        testCase "skip some parser directive test"
        <| fun _ ->
            Expect.equal
                ([ "StageNameCasing" ; "FromAsCasing" ] |> skip |> render)
                "# check=skip=StageNameCasing,FromAsCasing"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1. 'check' keyword\n\
                    3.2. key-value with\n\
                      3.2.1. 'skip' keyword\n\
                      3.2.2. values in first argument seperated by comma\n\n"

        testCase "skip empty parser directive test"
        <| fun _ ->
            Expect.throwsT<ArgumentOutOfRangeException>
                (fun _ -> skip [] |> ignore)
                errorMsg

        testCase "skipAll parser directive test"
        <| fun _ ->
            Expect.equal
                (skipAll |> render)
                "# check=skip=all"
                "String must contain\n\
                  1. sharp symbol\n\
                  2. whitespace\n\
                  3. key-value with\n\
                    3.1. 'check' keyword\n\
                    3.2. 'skip=all' value\n\n"
    ]
