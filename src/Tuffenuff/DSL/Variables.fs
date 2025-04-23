[<AutoOpen>]
module Tuffenuff.DSL.Variables

/// <summary>
/// Set variable that will be interpreted by the Dockerfile.
/// </summary>
/// <example><c>variable "foo"</c> -> <c>${foo}</c></example>
/// <param name="name">Name of the variable.</param>
/// <seealso cref="(!~)"/>
let variable (name : string) : string = $"${{%s{name}}}"

/// <summary>
/// Set variable that will be interpreted by the Dockerfile.
/// </summary>
/// <example><c>!~ "foo"</c> -> <c>${foo}</c></example>
/// <param name="name">Name of the variable.</param>
/// <seealso cref="variable"/>
let (!~) (name : string) : string = name |> variable

/// <summary>
/// Set variable with bash minus modifier that will be interpreted by the Dockerfile.
/// </summary>
/// <remarks>
/// If variable is set, then result will be it's value.<br/>
/// If variable is not set, then result will be value of '<paramref name="value"/>' param.
/// </remarks>
/// <example><c>!~- "foo" "bar"</c> -> <c>${foo:-bar}</c></example>
/// <param name="name">Name of the variable.</param>
/// <param name="value">The result if variable is not set.</param>
/// <seealso cref="(!~+)"/>
let (!~-) (name : string) (value : string) : string = $"%s{name}:-%s{value}" |> variable

/// <summary>
/// Set variable with bash plus modifier that will be interpreted by the Dockerfile.
/// </summary>
/// <remarks>
/// If variable is set, then result will be value of '<paramref name="value"/>' param.
/// If variable is not set, then result will be empty.<br/>
/// </remarks>
/// <example><c>!~+ "foo" "bar"</c> -> <c>${foo:+bar}</c></example>
/// <param name="name">Name of the variable.</param>
/// <param name="value">The result if variable is set.</param>
/// <seealso cref="(!~-)"/>
let (!~+) (name : string) (value : string) : string = $"%s{name}:+%s{value}" |> variable

/// <summary>
/// Set variable and remove the shortest match of pattern,
/// seeking from the start of the value.
/// </summary>
/// <example><c>!~? "foo" "b*r"</c> -> <c>${foo#b*r}</c></example>
/// <param name="name">Name of the variable.</param>
/// <param name="pattern">Pattern to search match.</param>
/// <seealso cref="(!~??)"/>
let (!~?) (name : string) (pattern : string) : string =
    $"%s{name}#%s{pattern}" |> variable

/// <summary>
/// Set variable and remove the longest match of pattern,
/// seeking from the start of the value.
/// </summary>
/// <example><c>!~?? "foo" "b*r"</c> -> <c>${foo##b*r}</c></example>
/// <param name="name">Name of the variable.</param>
/// <param name="pattern">Pattern to search match.</param>
/// <seealso cref="(!~?)"/>
let (!~??) (name : string) (pattern : string) : string =
    $"%s{name}##%s{pattern}" |> variable

/// <summary>
/// Set variable and remove the shortest match of pattern,
/// seeking backwards from the end of the value.
/// </summary>
/// <example><c>!~% "foo" "b*r"</c> -> <c>${foo%b*r}</c></example>
/// <param name="name">Name of the variable.</param>
/// <param name="pattern">Pattern to search match.</param>
/// <seealso cref="(!~%%)"/>
let (!~%) (name : string) (pattern : string) : string =
    $"%s{name}%%%s{pattern}" |> variable

/// <summary>
/// Set variable and remove the longest match of pattern,
/// seeking backwards from the end of the value.
/// </summary>
/// <example><c>!~%% "foo" "b*r"</c> -> <c>${foo%%b*r}</c></example>
/// <param name="name">Name of the variable.</param>
/// <param name="pattern">Pattern to search match.</param>
/// <seealso cref="(!~%)"/>
let (!~%%) (name : string) (pattern : string) : string =
    $"%s{name}%%%%%s{pattern}" |> variable

/// <summary>
/// Set variable and replace the first occurrence of pattern, with replacement in its
/// value.
/// </summary>
/// <example><c>!~/ "foo" "b*r" "baz"</c> -> <c>${foo/b*r/baz}</c></example>
/// <param name="name">Name of the variable.</param>
/// <param name="pattern">Pattern to search match.</param>
/// <param name="replacement">Value to replace first occurrence.</param>
/// <seealso cref="(!~//)"/>
let (!~/) (name : string) (pattern : string) (replacement : string) : string =
    $"%s{name}/%s{pattern}/%s{replacement}" |> variable

/// <summary>
/// Set variable and replace the all occurrences of pattern, with replacement in its
/// value.
/// </summary>
/// <example><c>!~/ "foo" "b*r" "baz"</c> -> <c>${foo/b*r/baz}</c></example>
/// <param name="name">Name of the variable.</param>
/// <param name="pattern">Pattern to search match.</param>
/// <param name="replacement">Value to replace all occurrence.</param>
/// <seealso cref="(!~/)"/>
let (!~//) (name : string) (pattern : string) (replacement : string) : string =
    $"%s{name}//%s{pattern}/%s{replacement}" |> variable
