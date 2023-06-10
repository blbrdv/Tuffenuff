[<AutoOpen>]
module Tuffenuff.DSL.From

open Tuffenuff.Domain.Types


let alias = As

let platform = Platform

let from image ps = 
    let name = 
        ps
        |> Seq.tryFindBack (function
                        | As _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | As s -> Some s
                        | _ -> None)
        |> Option.flatten
    let platform = 
        ps
        |> Seq.tryFindBack (function
                        | Platform _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Platform s -> Some s
                        | _ -> None)
        |> Option.flatten
    From { Image = image; Name = name; Platform = platform } |> Instruction

let fresh = from "scratch" []
