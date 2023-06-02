namespace Tuffenuff

open Tuffenuff.Domain

[<AutoOpen>]
module Instructions = 
    let dockerfile entities = entities

    let df = dockerfile

    let plain = Plain

    let br = plain ""

    let comment text = Simple { Name = "#"; Value = text } |> instr

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
        From { Image = image; Name = name; Platform = platform } |> instr

    let fresh = from "scratch" []

    let bind = BindOptionsBuilder ()

    let cache = CacheOptionsBuilder ()

    let tmpfs = TmpfsOptionsBuilder ()

    let secret = SecretOptionsBuilder ()

    let ssh = SshOptionsBuilder ()

    let mount value = 
        let x = value :> obj
        match x with
        | :? BindOptions as b -> b |> Bind
        | :? CacheOptions as c -> c |> Cache
        | :? TmpfsOptions as t -> t |> Tmpfs
        | :? SecretOptions as s -> s |> Secret
        | :? SshOptions as s -> s |> Ssh
        | _ -> failwith "Invalid type provided"
        |> Mount

    let def = Def

    let none = Absent

    let host = Host
    
    let insecure = Insecure

    let sandbox = Sandbox

    let network = Network 

    let security = Security

    let run flags cmds = Run { Flags = flags; Commands = cmds } |> instr

    let ( !> ) command = run [] [ command ]

    let cmd elements = List { Name = "CMD"; Elements = elements } |> instr

    let labels ps = KeyValueList { Name = "LABEL"; Elements = ps } |> instr

    let label key value = labels [ (key, value) ]

    let exp value = Simple { Name = "EXPOSE"; Value = value } |> instr

    let expose elements = List { Name = "EXPOSE"; Elements = elements } |> instr

    let envs ps = KeyValueList { Name = "ENV"; Elements = ps } |> instr

    let env key value = envs [ (key, value) ]

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
        |> instr

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
        |> instr

    let cp ps src dst = [ src; dst ] |> copy ps

    let entrypoint elements = List { Name = "ENTRYPOINT"; Elements = elements } |> instr

    let entry = entrypoint

    let volume value = SimpleQuoted { Name = "VOLUME"; Value = value } |> instr

    let volumes elements = List { Name = "VOLUME"; Elements = elements } |> instr

    let vol = volume

    let user value = Simple { Name = "USER"; Value = value } |> instr

    let usr = user

    let workdir value = SimpleQuoted { Name = "WORKDIR"; Value = value } |> instr

    let args ps = KeyValueList { Name = "ARG"; Elements = ps } |> instr

    let arg key value = args [ (key, value) ]

    let usearg key = Simple { Name = "ARG"; Value = key } |> instr

    let onbuild entity = Onbuild { Instruction = entity } |> instr

    let ( ~~ ) = onbuild

    let stopsignal value = Simple { Name = "STOPSIGNAL"; Value = value } |> instr

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
        |> instr

    let hc = healthchech

    let disableHealthcheck = Simple { Name = "HEALTHCHECK"; Value = "NONE" } |> instr

    let healthchechNone = disableHealthcheck

    let disableHc = disableHealthcheck

    let hcNone = disableHealthcheck

    let shell elements = List { Name = "SHELL"; Elements = elements } |> instr

    let part = Subpart

    let ( !& ) = part
    
    let variable = sprintf "${%s}"

    let ( ~% ) = variable
