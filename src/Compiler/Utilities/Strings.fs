// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

// Compiled into every assembly that uses it rather than referenced: these members are `inline`, and
// optimization info for anything non-public is dropped at the assembly boundary, so an assembly compiled
// with --optimize+ that only references them fails with FS1116/FS1118.
namespace Internal.Utilities.Library

open System
open System.Runtime.CompilerServices

[<AutoOpen>]
module internal StringExtensions =

    type String with

        member inline x.StartsWithOrdinal(value: string) =
            x.StartsWith(value, StringComparison.Ordinal)

        member inline x.EndsWithOrdinal(value: string) =
            x.EndsWith(value, StringComparison.Ordinal)

        member inline x.EndsWithOrdinalIgnoreCase(value: string) =
            x.EndsWith(value, StringComparison.OrdinalIgnoreCase)

        member inline x.IndexOfOrdinal(value: string) =
            x.IndexOf(value, StringComparison.Ordinal)

        member inline x.IndexOfOrdinal(value: string, startIndex) =
            x.IndexOf(value, startIndex, StringComparison.Ordinal)

        member inline x.IndexOfOrdinal(value: string, startIndex, count) =
            x.IndexOf(value, startIndex, count, StringComparison.Ordinal)

    [<AbstractClass; Sealed; Extension>]
    type ReadOnlySpanCharExtensions =

        [<Extension>]
        static member inline EqualsOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.Equals(value, StringComparison.Ordinal)

        [<Extension>]
        static member inline EqualsOrdinal(str: ReadOnlySpan<char>, value: string) =
            str.Equals(value.AsSpan(), StringComparison.Ordinal)

        [<Extension>]
        static member inline StartsWithOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.StartsWith(value, StringComparison.Ordinal)

        [<Extension>]
        static member inline StartsWithOrdinal(str: ReadOnlySpan<char>, value: string) =
            str.StartsWith(value.AsSpan(), StringComparison.Ordinal)

        [<Extension>]
        static member inline EndsWithOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.EndsWith(value, StringComparison.Ordinal)

        [<Extension>]
        static member inline EndsWithOrdinal(str: ReadOnlySpan<char>, value: string) =
            str.EndsWith(value.AsSpan(), StringComparison.Ordinal)

        [<Extension>]
        static member inline EndsWithOrdinalIgnoreCase(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.EndsWith(value, StringComparison.OrdinalIgnoreCase)

        [<Extension>]
        static member inline EndsWithOrdinalIgnoreCase(str: ReadOnlySpan<char>, value: string) =
            str.EndsWith(value.AsSpan(), StringComparison.OrdinalIgnoreCase)

        [<Extension>]
        static member IndexOf(str: ReadOnlySpan<char>, value: char) =
            let mutable index = -1
            let mutable i = 0

            while i < str.Length && index = -1 do
                if str[i] = value then index <- i else i <- i + 1

            index

        [<Extension>]
        static member inline IndexOfOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.IndexOf(value, StringComparison.Ordinal)

        [<Extension>]
        static member inline IndexOfOrdinal(str: ReadOnlySpan<char>, value: string) =
            str.IndexOf(value.AsSpan(), StringComparison.Ordinal)

        // Searching a slice answers with an index into that slice, so the offset goes back on to
        // report a position in `str` - what the String siblings these mirror return. A miss stays -1.

        [<Extension>]
        static member inline IndexOfOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>, startIndex) =
            let i = str.Slice(startIndex).IndexOf(value, StringComparison.Ordinal)
            if i < 0 then i else i + startIndex

        [<Extension>]
        static member inline IndexOfOrdinal(str: ReadOnlySpan<char>, value: string, startIndex) =
            let i = str.Slice(startIndex).IndexOf(value.AsSpan(), StringComparison.Ordinal)
            if i < 0 then i else i + startIndex

        [<Extension>]
        static member inline IndexOfOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>, startIndex, count) =
            let i = str.Slice(startIndex, count).IndexOf(value, StringComparison.Ordinal)
            if i < 0 then i else i + startIndex

        [<Extension>]
        static member inline IndexOfOrdinal(str: ReadOnlySpan<char>, value: string, startIndex, count) =
            let i =
                str.Slice(startIndex, count).IndexOf(value.AsSpan(), StringComparison.Ordinal)

            if i < 0 then i else i + startIndex
