[<AutoOpen>]
module Tuffenuff.DSL.User

open Tuffenuff.Domain.Types

/// <summary>Sets the user name (or <c>UID</c>) and optionally the user group (or
/// <c>GID</c>) to use when running the image and for any <c>RUN</c>, <c>CMD</c> and
/// <c>ENTRYPOINT</c> instructions that follow it in the Dockerfile.</summary>
let user value =
    Simple { Name = "USER" ; Value = value } |> Instruction

/// <summary>Sets the user name (or <c>UID</c>) and optionally the user group (or
/// <c>GID</c>) to use when running the image and for any <c>RUN</c>, <c>CMD</c> and
/// <c>ENTRYPOINT</c> instructions that follow it in the Dockerfile.</summary>
let usr = user
