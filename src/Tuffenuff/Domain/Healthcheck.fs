module Tuffenuff.Domain.Healthcheck

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections

type OptionsBuilder () =
    member __.Zero() = Parameters []

    member this.Yield (_) = this.Zero()

    [<CustomOperation("interval")>]
    member __.Interval (state : Parameters, value) = 
        state.Add("interval", value)

    [<CustomOperation("timeout")>]
    member __.Timeout (state : Parameters, value) = 
        state.Add("timeout", value)

    [<CustomOperation("period")>]
    member __.StartPeriod (state : Parameters, value) = 
        state.Add("start-period", value)

    [<CustomOperation("retries")>]
    member __.Retries (state : Parameters, value : int) = 
        state.Add("retries", value.ToString()) 

    member __.Combine (_, _) = ()

    member __.Delay (f) = f()
        
    member __.Run (state : Parameters) = state
