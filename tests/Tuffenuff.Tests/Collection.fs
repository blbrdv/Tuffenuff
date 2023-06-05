module Tests.Collection

open Expecto
open Tuffenuff.Collection

[<Tests>]
let tests =
    testList "Collection tests" [
        testCase "empty test" <| fun _ ->
            let a : Collection<string> = Collection.empty
            let b : Collection<string> = Collection.empty
            
            Expect.equal a b "empty collections must be equal"

        testCase "equal test" <| fun _ ->
            let a = Collection(seq { "a"; "b"; "c" })
            let b = Collection(seq { "a"; "b"; "c" })
            
            Expect.equal a b "collections must be equal"

        testCase "non-equal test" <| fun _ ->
            let a = Collection(seq { "a"; "b"; "x" })
            let b = Collection(seq { "a"; "b"; "y" })
            
            Expect.notEqual a b "collections must be not equal"
    ]
