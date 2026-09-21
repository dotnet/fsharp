// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Internal.Utilities.Library

open System
open System.Runtime.CompilerServices

[<AutoOpen>]
module internal StringExtensions =

    type String with

        member inline StartsWithOrdinal: value: string -> bool

        member inline EndsWithOrdinal: value: string -> bool

        member inline EndsWithOrdinalIgnoreCase: value: string -> bool

        member inline IndexOfOrdinal: value: string -> int

        member inline IndexOfOrdinal: value: string * startIndex: int -> int

        member inline IndexOfOrdinal: value: string * startIndex: int * count: int -> int

    [<AbstractClass; Sealed; Extension>]
    type ReadOnlySpanCharExtensions =

        [<Extension>]
        static member inline EqualsOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> bool

        [<Extension>]
        static member inline EqualsOrdinal: str: ReadOnlySpan<char> * value: string -> bool

        [<Extension>]
        static member inline StartsWithOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> bool

        [<Extension>]
        static member inline StartsWithOrdinal: str: ReadOnlySpan<char> * value: string -> bool

        [<Extension>]
        static member inline EndsWithOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> bool

        [<Extension>]
        static member inline EndsWithOrdinal: str: ReadOnlySpan<char> * value: string -> bool

        [<Extension>]
        static member inline EndsWithOrdinalIgnoreCase: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> bool

        [<Extension>]
        static member inline EndsWithOrdinalIgnoreCase: str: ReadOnlySpan<char> * value: string -> bool

        [<Extension>]
        static member IndexOf: str: ReadOnlySpan<char> * value: char -> int

        [<Extension>]
        static member inline IndexOfOrdinal: str: ReadOnlySpan<char> * value: ReadOnlySpan<char> -> int

        [<Extension>]
        static member inline IndexOfOrdinal: str: ReadOnlySpan<char> * value: string -> int

        [<Extension>]
        static member inline IndexOfOrdinal:
            str: ReadOnlySpan<char> * value: ReadOnlySpan<char> * startIndex: int -> int

        [<Extension>]
        static member inline IndexOfOrdinal: str: ReadOnlySpan<char> * value: string * startIndex: int -> int

        [<Extension>]
        static member inline IndexOfOrdinal:
            str: ReadOnlySpan<char> * value: ReadOnlySpan<char> * startIndex: int * count: int -> int

        [<Extension>]
        static member inline IndexOfOrdinal:
            str: ReadOnlySpan<char> * value: string * startIndex: int * count: int -> int
