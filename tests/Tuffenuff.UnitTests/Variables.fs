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
        <| fun _ ->
            Expect.equal (variable name) "${foo}"
            <| "Variable should be notated"
        
        testCase "variable short syntax test"
        <| fun _ ->
            Expect.equal (!~ name) "${foo}"
            <| "Variable should be notated"
        
        testCase "variable bash minus modifier syntax test"
        <| fun _ ->
            Expect.equal (!~- name value) "${foo:-bar}"
            <| "Variable's name and value should be notated with bash minus modifier"
        
        testCase "variable bash plus modifier syntax test"
        <| fun _ ->
            Expect.equal (!~+ name value) "${foo:+bar}"
            <| "Variable's name and value should be notated with bash plus modifier"
        
        testCase "variable pattern removal (seeking from the start of the string, shortest match) syntax test"
        <| fun _ ->
            Expect.equal (!~? name pattern) "${foo#b*r}"
            <| "Variable's name and pattern should be notated with '#' modifier"
        
        testCase "variable pattern removal (seeking from the start of the string, longest match) syntax test"
        <| fun _ ->
            Expect.equal (!~?? name pattern) "${foo##b*r}"
            <| "Variable's name and pattern should be notated with '##' modifier"
        
        testCase "variable pattern removal (seeking backwards from the end of the string, shortest match) syntax test"
        <| fun _ ->
            Expect.equal (!~% name pattern) "${foo%b*r}"
            <| "Variable's name and pattern should be notated with '%' modifier"
        
        testCase "variable pattern removal (seeking backwards from the end of the string, longest match) syntax test"
        <| fun _ ->
            Expect.equal (!~%% name pattern) "${foo%%b*r}"
            <| "Variable's name and pattern should be notated with '%%' modifier"
        
        testCase "variable pattern replacement (first occurrence) syntax test"
        <| fun _ ->
            Expect.equal (!~/ name pattern replacement) "${foo/b*r/baz}"
            <| "Variable's name, pattern and replacement should be notated with '/' modifier"
        
        testCase "variable pattern replacement (all occurrences) syntax test"
        <| fun _ ->
            Expect.equal (!~// name pattern replacement) "${foo//b*r/baz}"
            <| "Variable's name, pattern and replacement should be notated with '//' modifier"
    ]
