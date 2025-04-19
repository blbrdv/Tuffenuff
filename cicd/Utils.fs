[<AutoOpen>]
module CICD.Utils

open System
open System.Text

/// Break line symbol(s) as string.
let newLine = Environment.NewLine

/// Convert timespan to milliseconds.
let inline toMs (time : TimeSpan) =
    time.TotalMilliseconds |> Math.Truncate |> int

/// Replace control characters to escape sequences or Unicode literals.
let inline sanitize (value : string) : string =
    let b = StringBuilder(value.Length)
    value
    |> Seq.iter (fun char' ->
        match char' with
        | '\u0000' -> b.Append(@"\0") |> ignore
        | '\u0001' -> b.Append(@"\u0001") |> ignore
        | '\u0002' -> b.Append(@"\u0002") |> ignore
        | '\u0003' -> b.Append(@"\u0003") |> ignore
        | '\u0004' -> b.Append(@"\u0004") |> ignore
        | '\u0005' -> b.Append(@"\u0005") |> ignore
        | '\u0006' -> b.Append(@"\u0006") |> ignore
        | '\u0007' -> b.Append(@"\a") |> ignore
        | '\u0008' -> b.Append(@"\b") |> ignore
        | '\u0009' -> b.Append(@"\t") |> ignore
        | '\u000A' -> b.Append(@"\n") |> ignore
        | '\u000B' -> b.Append(@"\v") |> ignore
        | '\u000C' -> b.Append(@"\f") |> ignore
        | '\u000D' -> b.Append(@"\r") |> ignore
        | '\u000E' -> b.Append(@"\u000E") |> ignore
        | '\u000F' -> b.Append(@"\u000F") |> ignore
        | '\u0010' -> b.Append(@"\u0010") |> ignore
        | '\u0011' -> b.Append(@"\u0011") |> ignore
        | '\u0012' -> b.Append(@"\u0012") |> ignore
        | '\u0013' -> b.Append(@"\u0013") |> ignore
        | '\u0014' -> b.Append(@"\u0014") |> ignore
        | '\u0015' -> b.Append(@"\u0015") |> ignore
        | '\u0016' -> b.Append(@"\u0016") |> ignore
        | '\u0017' -> b.Append(@"\u0017") |> ignore
        | '\u0018' -> b.Append(@"\u0018") |> ignore
        | '\u0019' -> b.Append(@"\u0019") |> ignore
        | '\u001A' -> b.Append(@"\z") |> ignore
        | '\u001B' -> b.Append(@"\e") |> ignore
        | '\u001C' -> b.Append(@"\u001C") |> ignore
        | '\u001D' -> b.Append(@"\u001D") |> ignore
        | '\u001E' -> b.Append(@"\u001E") |> ignore
        | '\u001F' -> b.Append(@"\u001F") |> ignore
        | '\u0022' -> b.Append(@"\""") |> ignore
        | '\u007F' -> b.Append(@"\?") |> ignore
        | _ -> b.Append(char') |> ignore
    )
    b.ToString()
