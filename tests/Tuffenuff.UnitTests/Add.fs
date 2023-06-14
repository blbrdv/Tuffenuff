module Tests.Add

open Expecto
open Tuffenuff.DSL
open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections

[<Tests>]
let tests =
    testList "ADD instruction tests" [
        let errorMessage = "Records must be equals"


        testCase "short syntax test" <| fun _ ->
            let expected = 
                AddInstruction.Create([ "/a"; "/b" ])
                |> Add
                |> Instruction

            let actual = !@ "/a" "/b"

            Expect.equal actual expected errorMessage


        testCase "multiple sources test" <| fun _ ->
            let expected = 
                AddInstruction.Create([ "/a"; "/b"; "." ])
                |> Add
                |> Instruction

            let actual = 
                add [ "/a"; "/b"; "." ]

            Expect.equal actual expected errorMessage


        testCase "averything at once test" <| fun _ ->
            let expected = 
                {
                    Chown = Some "myuser:mygroup";
                    Chmod = Some "644";
                    Checksum = Some "123abc";
                    KeepGitDir = true;
                    Link = true;
                    Elements = Arguments [ "/a"; "/b" ]
                }
                |> Add
                |> Instruction

            let actual = 
                addOpts [ "/a"; "/b" ] {
                    chown "myuser:mygroup"
                    chmod "644"
                    checksum "123abc"
                    keepGitDir
                    link
                }

            Expect.equal actual expected errorMessage
    ]
