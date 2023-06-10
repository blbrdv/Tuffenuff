[<AutoOpen>]
module Tuffenuff.DSL.FileSystem

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types


let user value = Simple { Name = "USER"; Value = value } |> Instruction

let usr = user

let workdir value = SimpleQuoted { Name = "WORKDIR"; Value = value } |> Instruction

let from_ = Source

let chown = Chown

let chmod = Chmod

let link = Link

let checksum = Checksum

let sha = Checksum

let keepGitDir = KeepGitDir

let keep = KeepGitDir

let add ps args = 
    let chown = 
        ps
        |> Seq.tryFindBack (function
                        | Chown _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Chown s -> Some s
                        | _ -> None)
        |> Option.flatten
    let chmod = 
        ps
        |> Seq.tryFindBack (function
                        | Chmod _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Chmod s -> Some s
                        | _ -> None)
        |> Option.flatten
    let checksum = 
        ps
        |> Seq.tryFindBack (function
                        | Checksum _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Checksum s -> Some s
                        | _ -> None)
        |> Option.flatten
    let keepGitDir = Seq.contains KeepGitDir ps
    let link = Seq.contains Link ps
    Add {
        Chown = chown;
        Chmod = chmod;
        Checksum = checksum;
        KeepGitDir = keepGitDir;
        Link = link;
        Elements = Arguments args
    }
    |> Instruction

let ( !@ ) ps src dst = [ src; dst ] |> add ps

let copy ps args = 
    let from = 
        ps
        |> Seq.tryFindBack (function
                        | Source _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Source s -> Some s
                        | _ -> None)
        |> Option.flatten
    let chown = 
        ps
        |> Seq.tryFindBack (function
                        | Chown _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Chown s -> Some s
                        | _ -> None)
        |> Option.flatten
    let chmod = 
        ps
        |> Seq.tryFindBack (function
                        | Chmod _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Chmod s -> Some s
                        | _ -> None)
        |> Option.flatten
    let link = Seq.contains Link ps
    Copy {
        From = from
        Chown = chown;
        Chmod = chmod;
        Link = link;
        Elements = Arguments args
    }
    |> Instruction

let cp ps src dst = [ src; dst ] |> copy ps

let volume value = SimpleQuoted { Name = "VOLUME"; Value = value } |> Instruction

let volumes elements = List { Name = "VOLUME"; Elements = elements } |> Instruction

let vol = volume
