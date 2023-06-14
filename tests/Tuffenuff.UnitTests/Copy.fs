module Tests.Copy

open Expecto
open Tuffenuff.DSL
open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections

[<Tests>]
let tests =
    testList "COPY instruction tests" [
        let errorMessage = "Records must be equals"


        testCase "short syntax test" <| fun _ ->
            let expected = 
                CopyInstruction.Create([ "/a"; "/b" ])
                |> Copy
                |> Instruction

            let actual = cp "/a" "/b"

            Expect.equal actual expected errorMessage


        testCase "multiple sources test" <| fun _ ->
            let expected = 
                CopyInstruction.Create([ "/a"; "/b"; "." ])
                |> Copy
                |> Instruction

            let actual = 
                copy [ "/a"; "/b"; "." ]

            Expect.equal actual expected errorMessage


        testCase "averything at once test" <| fun _ ->
            let expected = 
                {
                    From = Some "builder";
                    Chown = Some "myuser:mygroup";
                    Chmod = Some "644";
                    Link = true;
                    Elements = Arguments [ "/a"; "/b" ]
                }
                |> Copy
                |> Instruction

            let actual = 
                copyOpts [ "/a"; "/b" ] {
                    from' "builder"
                    chown "myuser:mygroup"
                    chmod "644"
                    link
                }

            Expect.equal actual expected errorMessage
    ]
