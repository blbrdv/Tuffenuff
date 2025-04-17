[<AutoOpen>]
module CICD.Utils

open System

/// Convert timespan to milliseconds.
let inline toMs (time : TimeSpan) =
    time.TotalMilliseconds |> Math.Truncate |> int
