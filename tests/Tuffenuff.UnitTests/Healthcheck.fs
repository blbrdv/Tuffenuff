module Tests.Healthcheck

open Expecto
open Tuffenuff.DSL
open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections


let disabledHC = 
    Simple { Name = "HEALTHCHECK"; Value = "NONE" } 
    |> Instruction

let testFuncs =
    [ disableHealthcheck; healthcheckNone; disableHc; hcNone ]
    |> Seq.mapi (fun index instr ->
        let func = fun value () ->
            Expect.equal instr value "Healthcheck must be disabled"
        ($"{index} disable healthcheck test", func)
    )


[<Tests>]
let tests =
    testParam disabledHC testFuncs
    |> List.ofSeq
    |> List.append [
        testCase "healthcheck w/o options test"  <| fun () ->
            let expected = 
                Healthcheck { 
                    Options = Dict.empty; 
                    Instructions = Arguments [ "curl -f http://localhost/ || exit 1" ]
                }
                |> Instruction

            let actual = hc [ "curl -f http://localhost/ || exit 1" ]
            
            Expect.equal actual expected "Healthcheck must be w/o options"

        testCase "healthcheck with options test"  <| fun () ->
            let expected = 
                Healthcheck { 
                    Options = 
                        Parameters [
                            "interval", "1s"
                            "timeout", "2s"
                            "start-period", "3s"
                            "retries", "5"
                        ] 
                    Instructions = Arguments [ "curl -f http://localhost/ || exit 1" ]
                }
                |> Instruction

            let actual = 
                healthcheck 
                    (options {
                        interval "1s"
                        timeout "2s"
                        period "3s"
                        retries 5
                    }) 
                    [ "curl -f http://localhost/ || exit 1" ]
            
            Expect.equal actual expected "Healthcheck must be with options"
    ]
    |> testList "HEALTHCHECK instruction tests"
