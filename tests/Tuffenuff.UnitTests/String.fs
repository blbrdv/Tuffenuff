module Tests.String

open System
open Expecto
open Tuffenuff.String

[<Tests>]
let tests =
    testList "string tests" [
        testCase "quote test"
        <| fun _ ->
            Expect.equal
                (quote "quoted text")
                "\"quoted text\""
                "String must contain\n\
                  1. quote symbol\n\
                  2. unformatted text\n\
                  3. quote symbol\n\n"

        testCase "print test"
        <| fun _ ->
            Expect.equal
                (print "INSTRUCTION" "argument")
                $"INSTRUCTION argument%s{eol}"
                "String must contain\n\
                  1. name from first argument\n\
                  2. whitespace\n\
                  3. value from second argument\n\
                  4. EOL\n\n"

        testCase "printKV test"
        <| fun _ ->
            Expect.equal
                (printKV "key.name" "foo")
                "key.name=foo"
                "String must contain\n\
                  1. key from first argument\n\
                  2. equals symbol\n\
                  3. value from second argument\n\n"

        testCase "printKVQ test"
        <| fun _ ->
            Expect.equal
                (printKVQ "key.name" "foo")
                "key.name=\"foo\""
                "String must contain\n\
                  1. key from first argument\n\
                  2. equals symbol\n\
                  3. quote symbol\n\
                  4. value from second argument\n\
                  5. quote symbol\n\n"

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
                "String must contain all strings from argument seperated with whitespace"

        testCase "printList list test"
        <| fun _ ->
            Expect.equal
                (printList [ "a" ; "b" ; "c" ])
                "a b c"
                "String must contain all strings from argument seperated with whitespace"

        testCase "printList array test"
        <| fun _ ->
            Expect.equal
                (printList [| "a" ; "b" ; "c" |])
                """[ "a", "b", "c" ]"""
                "String must contain\n\
                  1. left bracket\n\
                  2. whitespace\n\
                  3. all strings from argument, quoted and seperated by comma and \
                  whitespace\n\
                  4. whitespace\n\
                  5. right bracket\n\n"

        testCase "printFlag true test"
        <| fun _ ->
            Expect.equal
                (printFlag "flag-name" true)
                " --flag-name"
                "String must contain\n\
                  1. whitespace\n\
                  2. two minus symbols\n\
                  3. flag name unmodified\n\n"

        testCase "printFlag false test"
        <| fun _ ->
            Expect.equal (printFlag "flag-name" false) String.Empty "String must be empty"

        testCase "printParameter some test"
        <| fun _ ->
            Expect.equal
                (printParameter "parameter" (Some 3))
                " --parameter=3"
                "String must contain\n\
                  1. whitespace\n\
                  2. two minus symbols\n\
                  3. parameter name from first argument\n\
                  4. equals symbol\n\
                  5. unfolded value from second argument\n\n"

        testCase "printParameter none test"
        <| fun _ ->
            Expect.equal
                (printParameter "parameter" None)
                String.Empty
                "String must be empty"

        testCase "printParameterQ some test"
        <| fun _ ->
            Expect.equal
                (printParameterQ "parameter" (Some 3))
                " --parameter=\"3\""
                "String must contain\n\
                  1. whitespace\n\
                  2. two minus symbols\n\
                  3. parameter name from first argument\n\
                  4. equals symbol\n\
                  5. quote symbol\n\
                  6. unfolded value from second argument\n\
                  7. quote symbol\n\n"

        testCase "printParameterQ none test"
        <| fun _ ->
            Expect.equal
                (printParameterQ "parameter" None)
                String.Empty
                "String must be empty"

        testCase "trim empty lines test"
        <| fun _ ->
            Expect.equal
                (trim $"{eol} {eol}{eol} FROM scratch")
                "FROM scratch"
                "String must contain text starting from first occurence of \
                symbols except whitespaces and EOL"

        testCase "trim non-empty lines test"
        <| fun _ ->
            Expect.equal
                (trim $"foo{eol} {eol}{eol} FROM scratch")
                $"foo{eol} {eol}{eol} FROM scratch"
                "String must be equal to provided in argument"
    ]
