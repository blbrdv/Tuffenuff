module Tests.String

open Expecto
open Tuffenuff.String
open Tuffenuff.StringCE

[<Tests>]
let tests =
    testList "string tests" [
        testCase "quote test"
        <| fun _ ->
            Expect.equal (quote "quoted text") "\"quoted text\""
            <| "String should be quoted"


        testCase "print test"
        <| fun _ ->
            Expect.equal (print "INSTRUCTION" "argument") (sprintf "INSTRUCTION argument%s" eol)
            <| "Instruction and argument should be printed separately"


        testCase "printKV test"
        <| fun _ ->
            Expect.equal (printKV "key.name" "somevalue") "key.name=somevalue"
            <| "Key-Value print should be prettified"


        testCase "printKVQ test"
        <| fun _ ->
            Expect.equal (printKVQ "key.name" "somevalue") "key.name=\"somevalue\""
            <| "Key-Value print should be prettified"


        testCase "printList seq test"
        <| fun _ ->
            Expect.equal
                (printList (
                    seq {
                        "a"
                        "b"
                        "c"
                    }
                ))
                "a b c"
            <| "Seq should be printed in shell form"


        testCase "printList list test"
        <| fun _ ->
            Expect.equal (printList [ "a" ; "b" ; "c" ]) "a b c"
            <| "List should be printed in shell form"

        testCase "rintList array test"
        <| fun _ ->
            Expect.equal (printList [| "a" ; "b" ; "c" |]) """[ "a", "b", "c" ]"""
            <| "Array should be printed in exec form"


        testCase "printFlag true test"
        <| fun _ ->
            Expect.equal (printFlag "flag-name" true) " --flag-name"
            <| "Passed flag should be prettified"


        testCase "printFlag false test"
        <| fun _ ->
            Expect.equal (printFlag "flag-name" false) ""
            <| "Unpassed flag should be equal to empty string"

        testCase "printParameter some test"
        <| fun _ ->
            Expect.equal (printParameter "parameter" (Some 3)) " --parameter=3"
            <| "Parameter with value shoyld be prettified"


        testCase "printParameter none test"
        <| fun _ ->
            Expect.equal (printParameter "parameter" None) ""
            <| "Parameter without value should be equal to empty string"


        testCase "printParameterQ test"
        <| fun _ ->
            Expect.equal (printParameterQ "parameter" (Some 3)) " --parameter=\"3\""
            <| "String parameter should be quoted"


        testCase "trim test"
        <| fun _ ->
            Expect.equal
                (trim
                    """

FROM scratch""")
                "FROM scratch"
            <| "First FROM instruction should be on first line"


        testCase "StringBuilder CE test"
        <| fun _ ->
            let a = "A"
            let b = "B"
            let c = "C"

            let expected =
                """A B
C"""

            let actual =
                str {
                    a
                    " "
                    b
                    eol
                    c
                }

            Expect.equal actual expected "String should be builded"
    ]
