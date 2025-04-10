[<AutoOpen>]
module Tuffenuff.DSL.Comments

open System
open Tuffenuff.Domain.Types
open Tuffenuff.String
open Tuffenuff.StringCE

/// <summary>
/// Comment for providing information about the Dockerfile or explaining the purpose of
/// individual instructions.
/// </summary>
/// <param name="text">Raw text of a comment.</param>
/// <example><c>comment "This is comment text"</c> -> <c># This is comment text</c>
/// </example>
/// <seealso cref="(!/)"/>
let comment (text : string) : Entity =
    Simple { Name = "#" ; Value = text } |> Instruction

/// <summary>
/// Short version of comment.
/// </summary>
/// <param name="text">Raw text of a comment.</param>
/// <example><c>!/ "This is comment text"</c> -> <c># This is comment text</c>
/// </example>
/// <seealso cref="comment"/>
let (!/) (text : string) : Entity = comment text

/// <summary>
/// Sets the set of instructions and arguments used to create a Docker container image
/// </summary>
/// <param name="version">Syntax version.</param>
/// <example>
/// <c>syntax "docker/dockerfile:1"</c> -> <c># syntax=docker/dockerfile:1</c>
/// </example>
/// <seealso cref="v1"/>
/// <seealso cref="v1_14_0"/>
/// <seealso cref="labs"/>
/// <seealso cref="experemental"/>
let syntax (version : string) : Entity = !/ $"syntax=%s{version}"

/// <summary>
/// Sets the character used to escape characters in a Dockerfile. Default is <c>\</c>.
/// </summary>
/// <param name="value">Character to use as escape.</param>
/// <example>
/// <c>escape '`'</c> -> <c># escape=`</c>
/// </example>
let escape (value : char) : Entity = !/ $"escape=%c{value}"

/// <summary>
/// Sets how build checks evaluated. By default, all checks are run, and failures are
/// treated as warnings.
/// </summary>
/// <param name="directives">Specify which directives will be skipped. Allowed value -
/// <c>(directive+|all)</c>, where directives can found at
/// <see href="https://docs.docker.com/reference/build-checks/" />.
/// All directive are case sensitive.
/// </param>
/// <param name="warnAsError">Indicates to treat warnings as failures or not.</param>
/// <example>
/// 1. <c>check [ "all" ] true</c> -> <c># check=skip=all;error=true</c><br/>
/// 2. <c>check [ "StageNameCasing"; "FromAsCasing" ] false</c> ->
/// <c># check=skip=StageNameCasing,FromAsCasing;error=false</c>
/// </example>
/// <exception cref="ArgumentOutOfRangeException">Throw when directives are empty.
/// </exception>
/// <seealso cref="warnAsError"/>
/// <seealso cref="skip"/>
/// <seealso cref="skipAll"/>
let check (directives : string seq) (warnAsError : bool) : Entity =
    if Seq.length directives < 1 then
        raise (
            ArgumentOutOfRangeException(
                "Sequence of directives must not be empty",
                nameof directives)
        )
    
    str {
        if directives = [ "all" ] then
            printKV "skip" "all"
        else
            directives
            |> String.concat ","
            |> printKV "skip"
            
        ";"
        
        warnAsError.ToString().ToLower()
        |> printKV "error"
        
    }
    |> printKV "check"
    |> comment

/// <summary>
/// Set to threat warnings as failures for build process.
/// </summary>
/// <example>
/// <c>warnAsError</c> -> <c># check=error=true</c>
/// </example>
/// <seealso cref="check"/>
/// <seealso cref="skip"/>
/// <seealso cref="skipAll"/>
let warnAsError : Entity = printKV "error" "true" |> printKV "check" |> comment

/// <summary>
/// Specify which directives will be skipped during build process.
/// </summary>
/// <param name="directives">Specify which directives will be skipped. Allowed value -
/// <c>(directive+|all)</c>, where directives can found at
/// <see href="https://docs.docker.com/reference/build-checks/" />.
/// All directive are case sensitive.
/// </param>
/// <example>
/// <c>skip [ "StageNameCasing"; "FromAsCasing" ]</c> ->
/// <c># check=skip=StageNameCasing,FromAsCasing</c>
/// </example>
/// <exception cref="ArgumentOutOfRangeException">Throw when directives are empty.
/// </exception>
/// <seealso cref="check"/>
/// <seealso cref="warnAsError"/>
/// <seealso cref="skipAll"/>
let skip (directives : string seq) : Entity =
    if Seq.length directives < 1 then
        raise (
            ArgumentOutOfRangeException(
                "Sequence of directives must not be empty",
                nameof directives)
        )
    
    directives
    |> String.concat ","
    |> printKV "skip"
    |> printKV "check"
    |> comment

/// <summary>
/// Specify all directives to be skipped during build process.
/// </summary>
/// <example>
/// <c>skipAll</c> -> <c># check=skip=all</c>
/// </example>
/// <seealso cref="check"/>
/// <seealso cref="warnAsError"/>
/// <seealso cref="skip"/>
let skipAll : Entity = skip [ "all" ]
