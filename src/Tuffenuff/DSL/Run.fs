[<AutoOpen>]
module Tuffenuff.DSL.Run

open Tuffenuff.Domain.Types
open Tuffenuff.CE

/// <summary>
/// Mount a file or directory from the host machine into the container.
/// </summary>
let bindParams target = BindBuilder target

/// <summary>Mount a file or directory from the host machine into the container.
/// </summary>
let bind (target : string) =
    MountParameters.Create (Bind, "target", target)

/// <summary>Cache files or directories between builds, which can speed up the build
/// process.</summary>
let cacheParams target = CacheBuilder target

/// <summary>Cache files or directories between builds, which can speed up the build
/// process.</summary>
let cache target =
    MountParameters.Create (Cache, "target", target)

/// <summary>Create a temporary file system in memory, which can be used to store
/// temporary files or directories.</summary>
let tmpfsParams target = TmpfsBuilder target

/// <summary>Create a temporary file system in memory, which can be used to store
/// temporary files or directories.</summary>
let tmpfs target =
    MountParameters.Create (Tmpfs, "target", target)

/// <summary>Mount a secret from the Docker secret store into the container.</summary>
let secret = SecretBuilder ()

/// <summary>Mount an SSH key from the host machine into the container.</summary>
let ssh = SshBuilder ()

/// <summary>
/// Sets commands to execute in a new layer on top of the current image and commit the
/// results.
/// </summary>
/// <seealso cref="(!>)"/>
let run = RunBuilder ()

/// <summary>
/// Sets commands to execute in a new layer on top of the current image and commit the
/// results.
/// </summary>
/// <example>
/// <c>!> "apt-get dist-upgrade -y"</c>
/// ->
/// <code>
/// RUN \
///     apt-get dist-upgrade -y
/// </code>
/// </example>
/// <param name="command">Command to run.</param>
/// <seealso cref="run"/>
let (!>) (command : string) = run { cmd command }
