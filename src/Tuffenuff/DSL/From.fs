[<AutoOpen>]
module Tuffenuff.DSL.From

open Tuffenuff.CE

/// <summary>
/// Initialise a new build stage and sets the base image and other options.
/// </summary>
/// <example>
/// <code>
/// based "alpine:latest" {
///     as' "build"
///     platform "linux/amd64"
/// }
/// </code>
/// ->
/// <c>FROM --platform="linux/amd64" alpine:latest AS build</c>
/// </example>
/// <param name="reference">Docker image reference.</param>
/// <seealso cref="stage"/>
/// <seealso cref="from"/>
/// <seealso cref="fresh"/>
let based (reference : string) = FromBuilder reference

/// <summary>
/// Initialise a new build stage and sets the base image and alias.
/// </summary>
/// <example>
/// <c>stage "alpine:latest" "build"</c> -> <c>FROM alpine:latest AS build</c>
/// </example>
/// <param name="reference">Docker image reference.</param>
/// <param name="alias">Alias for this build stage.</param>
/// <seealso cref="based"/>
let stage (reference : string) (alias : string) = based reference { as' alias }

/// <summary>
/// Initialise a new build stage and sets the base image.
/// </summary>
/// <example><c>from "alpine:latest"</c> -> <c>FROM alpine:latest</c></example>
/// <param name="reference">Docker image reference.</param>
/// <seealso cref="based"/>
let from (reference : string) = based reference { () }

/// <summary>
/// Initialise a new build stage and sets the base image to "scratch".
/// </summary>
/// <example><c>fresh</c> -> <c>FROM scratch</c></example>
/// <seealso cref="based"/>
let fresh = based "scratch" { () }
