[<AutoOpen>]
module Tuffenuff.DSL.FileSystem

open Tuffenuff.Domain.Types
open Tuffenuff.Domain.CE


/// <summary>Sets the user name (or <c>UID</c>) and optionally the user group (or
/// <c>GID</c>) to use when running the image and for any <c>RUN</c>, <c>CMD</c> and
/// <c>ENTRYPOINT</c> instructions that follow it in the Dockerfile.</summary>
let user value =
    Simple { Name = "USER" ; Value = value } |> Instruction

/// <summary>Sets the user name (or <c>UID</c>) and optionally the user group (or
/// <c>GID</c>) to use when running the image and for any <c>RUN</c>, <c>CMD</c> and
/// <c>ENTRYPOINT</c> instructions that follow it in the Dockerfile.</summary>
let usr = user

/// <summary>Sets the working directory for any <c>RUN</c>, <c>CMD</c>,
/// <c>ENTRYPOINT</c>, <c>COPY</c> and <c>ADD</c> instructions that follow it in the
/// Dockerfile.</summary>
let workdir value =
    SimpleQuoted { Name = "WORKDIR" ; Value = value } |> Instruction

/// <summary>Copies files, directories, or remote file URLs from the source to the
/// destination in the container.</summary>
let addOpts args = AddBuilder (args)

/// <summary>Copies files, directories, or remote file URLs from the source to the
/// destination in the container.</summary>
let add args =
    args |> AddInstruction.Create |> Add |> Instruction

/// <summary>Copies files, directories, or remote file URLs from the source to the
/// destination in the container.</summary>
let (!@) src dst =
    [ src ; dst ] |> AddInstruction.Create |> Add |> Instruction

/// <summary>Copies files or directories from the source to the destination in the
/// container.</summary>
let copyOpts args = CopyBuilder (args)

/// <summary>Copies files or directories from the source to the destination in the
/// container.</summary>
let copy args =
    args |> CopyInstruction.Create |> Copy |> Instruction

/// <summary>Copies files or directories from the source to the destination in the
/// container.</summary>
let cp src dst =
    [ src ; dst ] |> CopyInstruction.Create |> Copy |> Instruction

/// <summary>Creates a mount point with the specified name and marks it as holding
/// externally mounted volumes from native host or other containers.</summary>
let volume value =
    SimpleQuoted { Name = "VOLUME" ; Value = value } |> Instruction

/// <summary>Creates a mount point with the specified name and marks it as holding
/// externally mounted volumes from native host or other containers.</summary>
let volumes elements =
    List
        {
            Name = "VOLUME"
            Elements = elements
        }
    |> Instruction

/// <summary>Creates a mount point with the specified name and marks it as holding
/// externally mounted volumes from native host or other containers.</summary>
let vol = volume
