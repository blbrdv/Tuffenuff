[<AutoOpen>]
module Tuffenuff.DSL.Configuration

open Tuffenuff.Domain.Collections
open Tuffenuff.Domain.Types


/// <summary>Specifies the default command to be executed when a container is launched.
/// </summary>
let cmd elements =
    List
        {
            Name = "CMD"
            Elements = Arguments elements
        }
    |> Instruction

/// <summary>Informs Docker that the container will listen on the specified network ports
/// at runtime.</summary>
let exp value =
    Simple { Name = "EXPOSE" ; Value = value } |> Instruction

/// <summary>Informs Docker that the container will listen on the specified network ports
/// at runtime.</summary>
let expose elements =
    List
        {
            Name = "EXPOSE"
            Elements = elements
        }
    |> Instruction

/// <summary>Specifies the command to be executed when a container is started.</summary>
let entrypoint elements =
    List
        {
            Name = "ENTRYPOINT"
            Elements = Arguments elements
        }
    |> Instruction

/// <summary>Specifies the command to be executed when a container is started.</summary>
let entry = entrypoint

/// <summary>Sets the system call signal that will be sent to the container to stop
/// it gracefully.</summary>
let stopsignal value =
    Simple { Name = "STOPSIGNAL" ; Value = value } |> Instruction

/// <summary>Sets the system call signal that will be sent to the container to stop
/// it gracefully.</summary>
let (!!) = stopsignal

/// <summary>Sets the default shell used for the RUN, CMD, and ENTRYPOINT instructions.
/// </summary>
let shell elements =
    List { Name = "SHELL" ; Elements = elements } |> Instruction

/// <summary>Adds a trigger instruction to the image that will be executed when the image
/// is used as a base for another image.</summary>
let onbuild entity =
    Onbuild { Instruction = entity } |> Instruction

/// <summary>Adds a trigger instruction to the image that will be executed when the image
/// is used as a base for another image.</summary>
let (~~) = onbuild
