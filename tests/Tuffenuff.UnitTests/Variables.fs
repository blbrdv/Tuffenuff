module Tests.Variables

open Expecto
open Tuffenuff.DSL.Variables

[<Literal>]
let name = "foo"

[<Literal>]
let value = "bar"

[<Literal>]
let pattern = "b*r"

[<Literal>]
let replacement = "baz"

[<Tests>]
let tests =
    testList "variable tests" [
        testCase "variable test"
        <| fun _ -> Expect.equal (variable name) "${foo}" "String must be notated"

        testCase "variable short syntax test"
        <| fun _ -> Expect.equal (!~name) "${foo}" "String must be notated"

        testCase "variable bash minus modifier syntax test"
        <| fun _ ->
            Expect.equal
                (!~- name value)
                "${foo:-bar}"
                "String must be notated and contains\n\
                  1. variable name from first argument\n\
                  2. bash minus modifier (:-)\n\
                  3. value from second argument\n\n"

        testCase "variable bash plus modifier syntax test"
        <| fun _ ->
            Expect.equal
                (!~+ name value)
                "${foo:+bar}"
                "String must be notated and contains\n\
                  1. variable name from first argument\n\
                  2. bash plus modifier (:+)\n\
                  3. value from second argument\n\n"

        testCase "variable pattern removal (from start, shortest match) syntax test"
        <| fun _ ->
            Expect.equal
                (!~? name pattern)
                "${foo#b*r}"
                "String must be notated and contains\n\
                  1. variable name from first argument\n\
                  2. hash symbol\n\
                  3. pattern from second argument\n\n"

        testCase "variable pattern removal (from start, longest match) syntax test"
        <| fun _ ->
            Expect.equal
                (!~?? name pattern)
                "${foo##b*r}"
                "String must be notated and contains\n\
                  1. variable name from first argument\n\
                  2. two hash symbols\n\
                  3. pattern from second argument\n\n"

        testCase "variable pattern removal (backwards from the end, shortest) syntax test"
        <| fun _ ->
            Expect.equal
                (!~% name pattern)
                "${foo%b*r}"
                "String must be notated and contains\n\
                  1. variable name from first argument\n\
                  2. modulus symbol\n\
                  3. pattern from second argument\n\n"

        "variable pattern removal (backwards from the end, longest match) syntax test"
        |> testCase
        <| fun _ ->
            Expect.equal
                (!~%% name pattern)
                "${foo%%b*r}"
                "String must be notated and contains\n\
                  1. variable name from first argument\n\
                  2. two modulus symbols\n\
                  3. pattern from second argument\n\n"

        testCase "variable pattern replacement (first occurrence) syntax test"
        <| fun _ ->
            Expect.equal
                (!~/ name pattern replacement)
                "${foo/b*r/baz}"
                "String must be notated and contains\n\
                  1. variable name from first argument\n\
                  2. slash symbol\n\
                  3. pattern from second argument\n\
                  4. slash symbol\n\
                  5. replacement from third argument\n\n"

        testCase "variable pattern replacement (all occurrences) syntax test"
        <| fun _ ->
            Expect.equal
                (!~// name pattern replacement)
                "${foo//b*r/baz}"
                "String must be notated and contains\n\
                  1. variable name from first argument\n\
                  2. two slash symbols\n\
                  3. pattern from second argument\n\
                  4. slash symbol\n\
                  5. replacement from third argument\n\n"
    ]
