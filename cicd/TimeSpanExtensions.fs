module CICD.TimeSpanExtensions

open System
open System.Runtime.CompilerServices

[<Extension>]
type TimeSpanExtensions =

    [<Extension>]
    static member ToReadableString (this : TimeSpan) = this.ToString @"hh\:mm\:ss\.fff"
