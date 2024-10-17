namespace System

open System
open System.Runtime.CompilerServices

#if !NET7_0_OR_GREATER
[<Sealed; AbstractClass; Extension>]
type internal ReadOnlySpanExtensions =
    [<Extension>]
    static member IndexOfAnyExcept: span: ReadOnlySpan<char> * value0: char * value1: char -> int

    [<Extension>]
    static member IndexOfAnyExcept: span: ReadOnlySpan<char> * values: ReadOnlySpan<char> -> int

    [<Extension>]
    static member IndexOfAnyExcept: span: ReadOnlySpan<char> * value: char -> int

    [<Extension>]
    static member LastIndexOfAnyInRange: span: ReadOnlySpan<char> * lowInclusive: char * highInclusive: char -> int

    [<Extension>]
    static member LastIndexOfAnyExcept: span: ReadOnlySpan<char> * value: char -> int
#endif
