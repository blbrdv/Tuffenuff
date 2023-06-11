module Tests.Healthcheck

open Expecto
open Tuffenuff.DSL
open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections

[<Tests>]
let tests =
    testList "HEALTHCHECK instruction tests" [
        testCase "disable healthcheck test" <| fun () ->
            let expected = 
                Simple { Name = "HEALTHCHECK"; Value = "NONE" } 
                |> Instruction

            let actual1 = disableHealthcheck
            let actual2 = healthcheckNone
            let actual3 = disableHc
            let actual4 = hcNone
            
            Expect.equal actual1 expected "Healthcheck must be disabled"
            Expect.equal actual2 expected "Healthcheck must be disabled"
            Expect.equal actual3 expected "Healthcheck must be disabled"
            Expect.equal actual4 expected "Healthcheck must be disabled"

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
