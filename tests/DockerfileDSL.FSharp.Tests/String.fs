module Tests.String

open Expecto
open DockerfileDSL.FSharp.String

[<Tests>]
let tests =
    testList "string tests" [
        testCase "quote test" <| fun _ ->
            Expect.equal "\"quoted text\"" (quote "quoted text") 
            <| "String should be quoted"
        
        testCase "print test" <| fun _ ->
            Expect.equal "INSTRUCTION argument" (print "INSTRUCTION" "argument") 
            <| "Instruction and argument should be printed separately"
        
        testCase "printKV test" <| fun _ ->
            Expect.equal "key.name=\"somevalue\"" (printKV "key.name" "somevalue") 
            <| "Key-Value print should be prettified"
        
        testCase "printList seq test" <| fun _ ->
            Expect.equal "a b c" (printList (seq { "a"; "b"; "c" })) 
            <| "Seq should be printed in shell form"
        
        testCase "printList list test" <| fun _ ->
            Expect.equal "a b c" (printList [ "a"; "b"; "c" ]) 
            <| "List should be printed in shell form"
        
        testCase "rintList array test" <| fun _ ->
            Expect.equal """[ "a", "b", "c" ]""" (printList [| "a"; "b"; "c" |]) 
            <| "Array should be printed in exec form"
        
        testCase "printFlag true test" <| fun _ ->
            Expect.equal " --flag-name" (printFlag "flag-name" true) 
            <| "Passed flag should be prettified"
        
        testCase "printFlag false test" <| fun _ ->
            Expect.equal "" (printFlag "flag-name" false) 
            <| "Unpassed flag should be equal to empty string"
        
        testCase "printParameter some test" <| fun _ ->
            Expect.equal " --parameter=3" (printParameter "parameter" (Some 3)) 
            <| "Parameter with value shoyld be prettified"
        
        testCase "printParameter none test" <| fun _ ->
            Expect.equal "" (printParameter "parameter" None) 
            <| "Parameter without value should be equal to empty string"
        
        testCase "printParameterQ test" <| fun _ ->
            Expect.equal " --parameter=\"3\"" (printParameterQ "parameter" (Some 3)) 
            <| "String parameter should be quoted"
        
        testCase "trim test" <| fun _ ->
            Expect.equal "FROM scratch" (trim """

FROM scratch""" )
            <| "First FROM instruction should be on first line"

        testCase "StringBuilder CE test" <| fun _ ->
            let a = "A"
            let b = "B"
            let c = "C"

            let expected = """A B
C"""
            let actual = str {
                a
                " "
                b
                eol
                c
            }

            Expect.equal expected actual "String should be builded"
        
    ]
