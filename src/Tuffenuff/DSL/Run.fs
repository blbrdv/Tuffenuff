[<AutoOpen>]
module Tuffenuff.DSL.Run

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE


let bindParams target = BindParametersBuilder (target)

let bind target = MountParameters.Create(Bind, "target", target)

let cacheParams target = CacheParametersBuilder (target)

let cache target = MountParameters.Create(Cache, "target", target)

let tmpfsParams target = TmpfsParametersBuilder (target)

let tmpfs target = MountParameters.Create(Tmpfs, "target", target)

let secret = SecretParametersBuilder ()

let ssh = SshParametersBuilder ()

let run = RunInstructionBuilder ()

let ( !> ) command = run { cmd command }
