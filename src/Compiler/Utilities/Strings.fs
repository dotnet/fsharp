// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Internal.Utilities.Library

open System
open System.Runtime.CompilerServices

[<AutoOpen>]
module internal StringExtensions =

    type String with

        member x.StartsWithOrdinal(value: string) =
            x.StartsWith(value, StringComparison.Ordinal)

        member x.EndsWithOrdinal(value: string) =
            x.EndsWith(value, StringComparison.Ordinal)

        member x.EndsWithOrdinalIgnoreCase(value: string) =
            x.EndsWith(value, StringComparison.OrdinalIgnoreCase)

        member x.IndexOfOrdinal(value: string) =
            x.IndexOf(value, StringComparison.Ordinal)

        member x.IndexOfOrdinal(value: string, startIndex) =
            x.IndexOf(value, startIndex, StringComparison.Ordinal)

        member x.IndexOfOrdinal(value: string, startIndex, count) =
            x.IndexOf(value, startIndex, count, StringComparison.Ordinal)

    [<AbstractClass; Sealed; Extension>]
    type ReadOnlySpanCharExtensions =

        [<Extension>]
        static member EqualsOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.Equals(value, StringComparison.Ordinal)

        [<Extension>]
        static member EqualsOrdinal(str: ReadOnlySpan<char>, value: string) =
            ReadOnlySpanCharExtensions.EqualsOrdinal(str, value.AsSpan())

        [<Extension>]
        static member StartsWithOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.StartsWith(value, StringComparison.Ordinal)

        [<Extension>]
        static member StartsWithOrdinal(str: ReadOnlySpan<char>, value: string) =
            ReadOnlySpanCharExtensions.StartsWithOrdinal(str, value.AsSpan())

        [<Extension>]
        static member EndsWithOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.EndsWith(value, StringComparison.Ordinal)

        [<Extension>]
        static member EndsWithOrdinal(str: ReadOnlySpan<char>, value: string) =
            ReadOnlySpanCharExtensions.EndsWithOrdinal(str, value.AsSpan())

        [<Extension>]
        static member EndsWithOrdinalIgnoreCase(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.EndsWith(value, StringComparison.OrdinalIgnoreCase)

        [<Extension>]
        static member EndsWithOrdinalIgnoreCase(str: ReadOnlySpan<char>, value: string) =
            ReadOnlySpanCharExtensions.EndsWithOrdinalIgnoreCase(str, value.AsSpan())

        [<Extension>]
        static member IndexOf(str: ReadOnlySpan<char>, value: char) = MemoryExtensions.IndexOf(str, value)

        [<Extension>]
        static member IndexOfOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>) =
            str.IndexOf(value, StringComparison.Ordinal)

        [<Extension>]
        static member IndexOfOrdinal(str: ReadOnlySpan<char>, value: string) =
            ReadOnlySpanCharExtensions.IndexOfOrdinal(str, value.AsSpan())

        [<Extension>]
        static member IndexOfOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>, startIndex) =
            ReadOnlySpanCharExtensions.IndexOfOrdinal(str, value, startIndex, str.Length - startIndex)

        [<Extension>]
        static member IndexOfOrdinal(str: ReadOnlySpan<char>, value: string, startIndex) =
            ReadOnlySpanCharExtensions.IndexOfOrdinal(str, value.AsSpan(), startIndex)

        [<Extension>]
        static member IndexOfOrdinal(str: ReadOnlySpan<char>, value: ReadOnlySpan<char>, startIndex, count) =
            let i = str.Slice(startIndex, count).IndexOf(value, StringComparison.Ordinal)
            if i < 0 then i else i + startIndex

        [<Extension>]
        static member IndexOfOrdinal(str: ReadOnlySpan<char>, value: string, startIndex, count) =
            ReadOnlySpanCharExtensions.IndexOfOrdinal(str, value.AsSpan(), startIndex, count)
