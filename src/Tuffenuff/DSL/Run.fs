[<AutoOpen>]
module Tuffenuff.DSL.Run

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE


/// <summary>Mount a file or directory from the host machine into the container.
/// </summary>
let bindParams target = BindParametersBuilder (target)

/// <summary>Mount a file or directory from the host machine into the container.
/// </summary>
let bind target =
    MountParameters.Create (Bind, "target", target)

/// <summary>Cache files or directories between builds, which can speed up the build 
/// process.</summary>
let cacheParams target = CacheParametersBuilder (target)

/// <summary>Cache files or directories between builds, which can speed up the build 
/// process.</summary>
let cache target =
    MountParameters.Create (Cache, "target", target)

/// <summary>Create a temporary file system in memory, which can be used to store 
/// temporary files or directories.</summary>
let tmpfsParams target = TmpfsParametersBuilder (target)

/// <summary>Create a temporary file system in memory, which can be used to store 
/// temporary files or directories.</summary>
let tmpfs target =
    MountParameters.Create (Tmpfs, "target", target)

/// <summary>Mount a secret from the Docker secret store into the container.</summary>
let secret = SecretParametersBuilder ()

/// <summary>Mount an SSH key from the host machine into the container.</summary>
let ssh = SshParametersBuilder ()

/// <summary>Sets commands to execute in a new layer on top of the current image and 
/// commit the results.</summary>
let run = RunInstructionBuilder ()

/// <summary>Sets commands to execute in a new layer on top of the current image and 
/// commit the results.</summary>
let (!>) command = run { cmd command }
