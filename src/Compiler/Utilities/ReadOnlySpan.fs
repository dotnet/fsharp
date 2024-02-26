namespace System

open System
open System.Runtime.CompilerServices

#if !NET7_0_OR_GREATER
[<Sealed; AbstractClass; Extension>]
type ReadOnlySpanExtensions =
    [<Extension>]
    static member IndexOfAnyExcept(span: ReadOnlySpan<char>, value0: char, value1: char) =
        let mutable i = 0
        let mutable found = false

        while not found && i < span.Length do
            let c = span[i]

            if c <> value0 && c <> value1 then
                found <- true
            else
                i <- i + 1

        if found then i else -1

    [<Extension>]
    static member IndexOfAnyExcept(span: ReadOnlySpan<char>, values: ReadOnlySpan<char>) =
        let mutable i = 0
        let mutable found = false

        while not found && i < span.Length do
            if values.IndexOf span[i] < 0 then
                found <- true
            else
                i <- i + 1

        if found then i else -1

    [<Extension>]
    static member IndexOfAnyExcept(span: ReadOnlySpan<char>, value: char) =
        let mutable i = 0
        let mutable found = false

        while not found && i < span.Length do
            let c = span[i]

            if c <> value then found <- true else i <- i + 1

        if found then i else -1

    [<Extension>]
    static member LastIndexOfAnyInRange(span: ReadOnlySpan<char>, lowInclusive: char, highInclusive: char) =
        let mutable i = span.Length - 1
        let mutable found = false
        let range = highInclusive - lowInclusive

        while not found && i >= 0 do
            if span[i] - lowInclusive <= range then
                found <- true
            else
                i <- i - 1

        if found then i else -1

    [<Extension>]
    static member LastIndexOfAnyExcept(span: ReadOnlySpan<char>, value: char) =
        let mutable i = span.Length - 1
        let mutable found = false

        while not found && i >= 0 do
            let c = span[i]

            if c <> value then found <- true else i <- i - 1

        if found then i else -1
#endif
