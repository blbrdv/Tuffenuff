[<AutoOpen>]
module Tuffenuff.Instructions

open System
open Tuffenuff.Domain


let dockerfile (entities : Entity seq) = entities

let df = dockerfile

let plain = Plain

let br = plain ""

let comment text = Simple { Name = "#"; Value = text } |> Instruction

let ( !/ ) = comment

let syntax value = !/ (sprintf "syntax=%s" value)

let escape value = !/ (sprintf "escape=%c" value)

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

let bindParams target = BindParametersBuilder (target)

let bind target = bindParams target {()}

let cacheParams target = CacheParametersBuilder (target)

let cache target = cacheParams target {()}

let tmpfsParams target = TmpfsParametersBuilder (target)

let tmpfs target = tmpfsParams target {()}

let secret = SecretParametersBuilder ()

let ssh = SshParametersBuilder ()

let defaultNetwork = Def

let noNetwork = Absent

let hostNetwork = Host

let noSecurity = Insecure

let sandboxSecurity = Sandbox

let run = RunInstructionBuilder ()

let ( !> ) (commands : string seq) = run { commands }

let cmd elements = List { Name = "CMD"; Elements = elements } |> Instruction

let labels ps = KeyValueList { Name = "LABEL"; Elements = ps } |> Instruction

let label key value = [ (key, value) ] |> Map.ofSeq |> labels

[<Obsolete("MAINTAINER instruction is deprecated, use LABEL instead.")>]
let maintainer name = Simple { Name = "MAINTAINER"; Value = name } |> Instruction

let exp value = Simple { Name = "EXPOSE"; Value = value } |> Instruction

let expose elements = List { Name = "EXPOSE"; Elements = elements } |> Instruction

let envs ps = KeyValueList { Name = "ENV"; Elements = ps } |> Instruction

let env key value = [ (key, value) ] |> Map.ofSeq |> envs

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
        Elements = args
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
        Elements = args
    }
    |> Instruction

let cp ps src dst = [ src; dst ] |> copy ps

let entrypoint elements = List { Name = "ENTRYPOINT"; Elements = elements } |> Instruction

let entry = entrypoint

let volume value = SimpleQuoted { Name = "VOLUME"; Value = value } |> Instruction

let volumes elements = List { Name = "VOLUME"; Elements = elements } |> Instruction

let vol = volume

let user value = Simple { Name = "USER"; Value = value } |> Instruction

let usr = user

let workdir value = SimpleQuoted { Name = "WORKDIR"; Value = value } |> Instruction

let args ps = KeyValueList { Name = "ARG"; Elements = ps } |> Instruction

let arg key value = [ (key, value) ] |> Map.ofSeq |> args

let usearg key = Simple { Name = "ARG"; Value = key } |> Instruction

let onbuild entity = Onbuild { Instruction = entity } |> Instruction

let ( ~~ ) = onbuild

let stopsignal value = Simple { Name = "STOPSIGNAL"; Value = value } |> Instruction

let ( !! ) = stopsignal

let interval = Interval

let timout = Timeout

let startPeriod = StartPeriod

let start = StartPeriod

let retries = Retries

let healthchech ps commands = 
    let interval = 
        ps
        |> Seq.tryFindBack (function
                        | Interval _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Interval s -> Some s
                        | _ -> None)
        |> Option.flatten
    let timeout = 
        ps
        |> Seq.tryFindBack (function
                        | Timeout _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Timeout s -> Some s
                        | _ -> None)
        |> Option.flatten
    let startPeriod = 
        ps
        |> Seq.tryFindBack (function
                        | StartPeriod _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | StartPeriod s -> Some s
                        | _ -> None)
        |> Option.flatten
    let retries = 
        ps
        |> Seq.tryFindBack (function
                        | Retries _ -> true
                        | _ -> false) 
        |> Option.map (function
                        | Retries s -> Some s
                        | _ -> None)
        |> Option.flatten
    Healthcheck { 
        Interval = interval; 
        Timeout = timeout; 
        StartPeriod = startPeriod; 
        Retries = retries; 
        Instructions = commands 
    }
    |> Instruction

let hc = healthchech

let disableHealthcheck = Simple { Name = "HEALTHCHECK"; Value = "NONE" } |> Instruction

let healthchechNone = disableHealthcheck

let disableHc = disableHealthcheck

let hcNone = disableHealthcheck

let shell elements = List { Name = "SHELL"; Elements = elements } |> Instruction

let part = Subpart

let ( !& ) = part

let variable = sprintf "${%s}"

let ( ~% ) = variable
