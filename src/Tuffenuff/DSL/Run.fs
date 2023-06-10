[<AutoOpen>]
module Tuffenuff.DSL.Run

open Tuffenuff.Domain.Run


let bindParams target = BindParametersBuilder (target)

let bind target = bindParams target {()}

let cacheParams target = CacheParametersBuilder (target)

let cache target = cacheParams target {()}

let tmpfsParams target = TmpfsParametersBuilder (target)

let tmpfs target = tmpfsParams target {()}

let secret = SecretParametersBuilder ()

let ssh = SshParametersBuilder ()

let run = RunInstructionBuilder ()

let ( !> ) command = run { cmd command }
