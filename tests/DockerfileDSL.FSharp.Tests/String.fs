// TODO: parametrize tests where possible
module DockerfileDSL.FSharp.Tests.String

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open DockerfileDSL.FSharp.String

[<Fact>]
let ``quote test`` () =
    "\"quoted text\""
    |> should equal
    <| quote "quoted text"

[<Fact>]
let ``print test`` () =
    "INSTRUCTION argument"
    |> should equal
    <| print "INSTRUCTION" "argument"

[<Fact>]
let ``print key=value test`` () =
    "key.name=\"somevalue\""
    |> should equal
    <| printKV "key.name" "somevalue"

[<Fact>]
let ``printList seq test`` () =
    "a b c"
    |> should equal
    <| printList (seq { "a"; "b"; "c" })

[<Fact>]
let ``printList list test`` () =
    "a b c"
    |> should equal
    <| printList [ "a"; "b"; "c" ]

[<Fact>]
let ``printList array test`` () =
    """[ "a", "b", "c" ]"""
    |> should equal
    <| printList [| "a"; "b"; "c" |]

[<Fact>]
let ``printFlag true test`` () =
    " --flag-name"
    |> should equal
    <| printFlag "flag-name" true

[<Fact>]
let ``printFlag false test`` () =
    ""
    |> should equal
    <| printFlag "flag-name" false

[<Fact>]
let ``printParameter some test`` () =
    " --parameter=3"
    |> should equal
    <| printParameter "parameter" (Some 3)

[<Fact>]
let ``printParameter none test`` () =
    ""
    |> should equal
    <| printParameter "parameter" None

[<Fact>]
let ``printParameterQ test`` () =
    " --parameter=\"3\""
    |> should equal
    <| printParameterQ "parameter"  (Some 3)

[<Fact>]
let ``trim test`` () =
    "FROM scratch"
    |> should equal
    <| trim """
FROM scratch"""

[<Fact>]
let ``StringBuilder CE test`` () =
    let a = "A"
    let b = "B"
    let c = "C"

    """A B
C"""
    |> should equal
    <| str {
        a
        " "
        b
        "\n"
        c
    }