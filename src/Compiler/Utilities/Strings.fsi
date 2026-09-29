// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Internal.Utilities.Library

open System
open System.Runtime.CompilerServices

[<AutoOpen>]
module internal StringExtensions =

    type String with

        member StartsWithOrdinal: value: string -> bool

        member EndsWithOrdinal: value: string -> bool

        member EndsWithOrdinalIgnoreCase: value: string -> bool

        member IndexOfOrdinal: value: string -> int

        member IndexOfOrdinal: value: string * startIndex: int -> int

        member IndexOfOrdinal: value: string * startIndex: int * count: int -> int

    [<AbstractClass; Sealed; Extension>]
    type ReadOnlySpanCharExtensions =

        [<Extension>]
        static member EqualsOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> bool

        [<Extension>]
        static member EqualsOrdinal: str: ReadOnlySpan<char> * value: string -> bool

        [<Extension>]
        static member StartsWithOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> bool

        [<Extension>]
        static member StartsWithOrdinal: str: ReadOnlySpan<char> * value: string -> bool

        [<Extension>]
        static member EndsWithOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> bool

        [<Extension>]
        static member EndsWithOrdinal: str: ReadOnlySpan<char> * value: string -> bool

        [<Extension>]
        static member EndsWithOrdinalIgnoreCase: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> bool

        [<Extension>]
        static member EndsWithOrdinalIgnoreCase: str: ReadOnlySpan<char> * value: string -> bool

        [<Extension>]
        static member IndexOf: str: ReadOnlySpan<char> * value: char -> int

        [<Extension>]
        static member IndexOfOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> int

        [<Extension>]
        static member IndexOfOrdinal: str: ReadOnlySpan<char> * value: string -> int

        [<Extension>]
        static member IndexOfOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> * startIndex: int -> int

        [<Extension>]
        static member IndexOfOrdinal: str: ReadOnlySpan<char> * value: string * startIndex: int -> int

        [<Extension>]
        static member IndexOfOrdinal:
            str: ReadOnlySpan<char> * value: ReadOnlySpan<char> * startIndex: int * count: int -> int

        [<Extension>]
        static member IndexOfOrdinal: str: ReadOnlySpan<char> * value: string * startIndex: int * count: int -> int
