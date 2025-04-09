module Tests.String

open System
open Expecto
open Tuffenuff.String
open Tuffenuff.StringCE

[<Tests>]
let tests =
    let line = "FROM scratch"
    let trimableLine = $"{eol} {eol}{eol} {line}"
    
    testList "string tests" [
        testCase "quote test"
        <| fun _ ->
            Expect.equal (quote "quoted text") "\"quoted text\""
            <| "String should be quoted"

        testCase "print test"
        <| fun _ ->
            Expect.equal
                (print "INSTRUCTION" "argument")
                $"INSTRUCTION argument%s{eol}"
            <| "Instruction and argument should be printed separately"

        testCase "printKV test"
        <| fun _ ->
            Expect.equal (printKV "key.name" "somevalue") "key.name=somevalue"
            <| "Key-Value print should be prettified"

        testCase "printKVQ test"
        <| fun _ ->
            Expect.equal (printKVQ "key.name" "somevalue") "key.name=\"somevalue\""
            <| "Quoted Key-Value print should be prettified"

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

        testCase "printList array test"
        <| fun _ ->
            Expect.equal (printList [| "a" ; "b" ; "c" |]) """[ "a", "b", "c" ]"""
            <| "Array should be printed in exec form"

        testCase "printFlag true test"
        <| fun _ ->
            Expect.equal (printFlag "flag-name" true) " --flag-name"
            <| "Included flag should be prettified"

        testCase "printFlag false test"
        <| fun _ ->
            Expect.equal (printFlag "flag-name" false) String.Empty
            <| "Excluded flag should be equal to empty string"

        testCase "printParameter some test"
        <| fun _ ->
            Expect.equal (printParameter "parameter" (Some 3)) " --parameter=3"
            <| "Parameter with value should be prettified"

        testCase "printParameter none test"
        <| fun _ ->
            Expect.equal (printParameter "parameter" None) String.Empty
            <| "Parameter without value should be equal to empty string"

        testCase "printParameterQ test"
        <| fun _ ->
            Expect.equal (printParameterQ "parameter" (Some 3)) " --parameter=\"3\""
            <| "String parameter should be quoted"

        testCase "trim empty lines test"
        <| fun _ ->
            Expect.equal
                (trim trimableLine)
                line
            <| "Trim should remove leading empty lines and spaces"
            
        testCase "trim non-empty lines test"
        <| fun _ ->
            Expect.equal
                (trim $"somevalue{trimableLine}")
                $"somevalue{trimableLine}"
            <| "Trim should not remove leading non-empty lines"
            
        testCase "variable test"
        <| fun _ ->
            Expect.equal
                (variable "somevalue")
                "${somevalue}"
            <| "Variable should be notated"
            
        ptestCase "variable short syntax test" // TODO fix short syntax
        <| fun _ ->
            Expect.equal
                "" //(~% "somevalue")
                "${somevalue}"
            <| "Variable should be notated"

        testCase "StringBuilder CE test"
        <| fun _ ->
            let a = "A"
            let b = "B"
            let c = "C"

            let expected = $"{a} {b}{eol}{c}"

            let actual =
                str {
                    a
                    ws
                    b
                    eol
                    c
                }

            Expect.equal actual expected "String should be built"
    ]
