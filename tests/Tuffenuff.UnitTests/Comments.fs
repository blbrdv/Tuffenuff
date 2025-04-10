module Tests.Comments

open System
open Expecto
open Tuffenuff.DSL
open Tuffenuff.DSL.Dockerfile
open Tuffenuff.String
open Tests.Utils

[<Literal>]
let private text = "This is comment text"

[<Literal>]
let private docker_v1 = "docker/dockerfile:1"

[<Literal>]
let private esc = '`'

[<Literal>]
let private errorMsg = "Sequence of directives must not be empty"

[<Tests>]
let tests =
    testList "comment tests" [
        testCase "comment test"
        <| fun _ ->
            Expect.equal (text |> comment |> render) $"# %s{text}"
            <| "Comment should should start with '#' and contains it's text"

        testCase "empty comment test"
        <| fun _ ->
            Expect.equal (empty |> comment |> render) empty
            <| "Empty comment should not be rendered"

        testCase "short comment test"
        <| fun _ ->
            Expect.equal (!/text |> render) $"# %s{text}"
            <| "Comment should should start with '#' and contains it's text"

        testCase "syntax parser directive test"
        <| fun _ ->
            Expect.equal (docker_v1 |> syntax |> render) $"# syntax=%s{docker_v1}"
            <| "Syntax parser directive should be comment with 'syntax=<version>' value"

        testCase "v1 syntax parser directive test"
        <| fun _ ->
            Expect.equal (v1 |> render) $"# syntax=%s{docker_v1}"
            <| "Syntax parser directive should be comment with 'syntax=<version>' value"

        testCase "escape parser directive test"
        <| fun _ ->
            Expect.equal (esc |> escape |> render) $"# escape=%c{esc}"
            <| "Escape parser directive should be comment with 'escape=<char>' value"

        testCase "check all parser directive test"
        <| fun _ ->
            Expect.equal (check [ "all" ] true |> render) "# check=skip=all;error=true"
            <| "Check parser directive should be comment with 'check=skip=<checks>;error=<boolean>' value"

        testCase "check some parser directive test"
        <| fun _ ->
            Expect.equal
                (check [ "StageNameCasing" ; "FromAsCasing" ] false |> render)
                "# check=skip=StageNameCasing,FromAsCasing;error=false"
            <| "Check parser directive should be comment with 'check=skip=<checks>;error=<boolean>' value"

        testCase "check empty parser directive test"
        <| fun _ ->
            Expect.throwsT<ArgumentOutOfRangeException>
                (fun _ -> check [] false |> render |> ignore)
                errorMsg

        testCase "warnAsError parser directive test"
        <| fun _ ->
            Expect.equal (warnAsError |> render) "# check=error=true"
            <| "Turn on error parser directive should be comment with 'check=error=true' value"

        testCase "skip some parser directive test"
        <| fun _ ->
            Expect.equal
                ([ "StageNameCasing" ; "FromAsCasing" ] |> skip |> render)
                "# check=skip=StageNameCasing,FromAsCasing"
            <| "Skip parser directive should be comment with 'check=skip=<checks>' value"

        testCase "skip empty parser directive test"
        <| fun _ ->
            Expect.throwsT<ArgumentOutOfRangeException>
                (fun _ -> [] |> skip |> render |> ignore)
                errorMsg

        testCase "skipAll parser directive test"
        <| fun _ ->
            Expect.equal (skipAll |> render) "# check=skip=all"
            <| "Skip all parser directive should be comment with 'check=skip=all' value"
    ]
