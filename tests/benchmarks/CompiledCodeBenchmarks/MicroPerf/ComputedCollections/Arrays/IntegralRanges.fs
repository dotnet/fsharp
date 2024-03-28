module MicroPerf.ComputedCollections.Arrays.IntegralRanges

open BenchmarkDotNet.Attributes

#nowarn "77"

let inline (~~) x = (^a : (static member op_Explicit : int -> ^a) x)

module All =
    let inline ``[|start..finish|]``<'a
        when 'a : (static member (+) : 'a * 'a -> 'a)
         and 'a : (static member One : 'a)
         and 'a : comparison>
        (start : 'a) (finish : 'a)
        =
        [|start..finish|]

    let inline ``[|start..step..finish|]``<'a
        when 'a : (static member (+)  : 'a * 'a -> 'a)
         and 'a : (static member Zero : 'a)
         and 'a : (static member One  : 'a)
         and 'a : comparison>
        (start : 'a) (step : 'a) (finish : 'a)
        =
        [|start..step..finish|]

    let inline ``[|for n in start..finish -> n|]``<'a
        when 'a : (static member (+) : 'a * 'a -> 'a)
         and 'a : (static member One : 'a)
         and 'a : comparison>
        (start : 'a) (finish : 'a)
        =
        [|for n in start..finish -> n|]

    let inline ``[|for n in start..step..finish -> n|]``<'a
        when 'a : (static member (+)  : 'a * 'a -> 'a)
         and 'a : (static member Zero : 'a)
         and 'a : (static member One  : 'a)
         and 'a : comparison>
        (start : 'a) (step : 'a) (finish : 'a)
        =
        [|for n in start..step..finish -> n|]

    let inline ``[|1..127|]``<'a
        when 'a : (static member (+) : 'a * 'a -> 'a)
         and 'a : (static member One : 'a)
         and 'a : comparison
         and 'a : (static member op_Explicit : int -> 'a)> () =
        ``[|start..finish|]``<'a> ~~1 ~~127

    let inline ``[|1..2..127|]``<'a
        when 'a : (static member (+)         : 'a * 'a -> 'a)
         and 'a : (static member Zero        : 'a)
         and 'a : (static member One         : 'a)
         and 'a : (static member op_Explicit : int -> 'a)
         and 'a : comparison> () =
        ``[|start..step..finish|]``<'a> ~~1 ~~2 ~~127

    let inline ``[|127..1|]``<'a
        when 'a : (static member (+) : 'a * 'a -> 'a)
         and 'a : (static member One : 'a)
         and 'a : comparison
         and 'a : (static member op_Explicit : int -> 'a)> () =
        ``[|start..finish|]``<'a> ~~127 ~~1

    let inline ``[|127..2..1|]``<'a
        when 'a : (static member (+)         : 'a * 'a -> 'a)
         and 'a : (static member Zero        : 'a)
         and 'a : (static member One         : 'a)
         and 'a : (static member op_Explicit : int -> 'a)
         and 'a : comparison> () =
        ``[|start..step..finish|]``<'a> ~~127 ~~2 ~~1

    let inline ``[|1..32767|]``<'a
        when 'a : (static member (+) : 'a * 'a -> 'a)
         and 'a : (static member One : 'a)
         and 'a : comparison
         and 'a : (static member op_Explicit : int -> 'a)> () =
        ``[|start..finish|]``<'a> ~~1 ~~32767

    let inline ``[|1..2..32767|]``<'a
        when 'a : (static member (+)         : 'a * 'a -> 'a)
         and 'a : (static member Zero        : 'a)
         and 'a : (static member One         : 'a)
         and 'a : (static member op_Explicit : int -> 'a)
         and 'a : comparison> () =
        ``[|start..step..finish|]``<'a> ~~1 ~~2 ~~32767

module Signed =
    let inline ``[|127..-1..-128|]``<'a
        when 'a : (static member (+)         : 'a * 'a -> 'a)
         and 'a : (static member (~-)        : 'a -> 'a)
         and 'a : (static member One         : 'a)
         and 'a : (static member Zero        : 'a)
         and 'a : (static member op_Explicit : int -> 'a)
         and 'a : comparison> () =
        All.``[|start..step..finish|]``<'a> ~~127 (- ~~1) (- ~~128)

    let inline ``[|127..-2..-128|]``<'a
        when 'a : (static member (+)         : 'a * 'a -> 'a)
         and 'a : (static member (~-)        : 'a -> 'a)
         and 'a : (static member One         : 'a)
         and 'a : (static member Zero        : 'a)
         and 'a : (static member op_Explicit : int -> 'a)
         and 'a : comparison> () =
        All.``[|start..step..finish|]``<'a> ~~127 (- ~~2) (- ~~128)

    let inline ``[|32767..-1..-32768|]``<'a
        when 'a : (static member (+)         : 'a * 'a -> 'a)
         and 'a : (static member (~-)        : 'a -> 'a)
         and 'a : (static member One         : 'a)
         and 'a : (static member Zero        : 'a)
         and 'a : (static member op_Explicit : int -> 'a)
         and 'a : comparison> () =
        All.``[|start..step..finish|]``<'a> ~~32767 (- ~~1) (- ~~32768)

    let inline ``[|32767..-2..-32768|]``<'a
        when 'a : (static member (+)         : 'a * 'a -> 'a)
         and 'a : (static member (~-)        : 'a -> 'a)
         and 'a : (static member One         : 'a)
         and 'a : (static member Zero        : 'a)
         and 'a : (static member op_Explicit : int -> 'a)
         and 'a : comparison> () =
        All.``[|start..step..finish|]``<'a> ~~32767 (- ~~2) (- ~~32769)

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_SByte () =
    member _.StartFinish : obj array seq =
        [
            [|-128y; 127y|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|127y; -1y; -128y|]
            [|-128y; 2y; 127y|]
        ]

    [<Benchmark; BenchmarkCategory("SByte", "[|127y..1y|]")>]
    member _.M1 () = All.``[|127..1|]``<sbyte> ()

    [<Benchmark; BenchmarkCategory("SByte", "[|127y..2y..1y|]")>]
    member _.M2 () = All.``[|127..2..1|]``<sbyte> ()

    [<Benchmark; BenchmarkCategory("SByte", "[|1y..127y|]")>]
    member _.M3 () = All.``[|1..127|]``<sbyte> ()

    [<Benchmark; BenchmarkCategory("SByte", "[|1y..2y..127y|]")>]
    member _.M4 () = All.``[|1..2..127|]``<sbyte> ()

    [<Benchmark; BenchmarkCategory("SByte", "[|127y..-1y..-128y|]")>]
    member _.M5 () = Signed.``[|127..-1..-128|]``<sbyte> ()

    [<Benchmark; BenchmarkCategory("SByte", "[|127y..-2y..-128y|]")>]
    member _.M6 () = Signed.``[|127..-2..-128|]``<sbyte> ()

    [<Benchmark; BenchmarkCategory("SByte", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_SByte>.StartFinish)>]
    member _.M7 (start, finish) = All.``[|start..finish|]``<sbyte> start finish

    [<Benchmark; BenchmarkCategory("SByte", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_SByte>.StartStepFinish)>]
    member _.M8 (start, step, finish) = All.``[|start..step..finish|]``<sbyte> start step finish

    [<Benchmark; BenchmarkCategory("SByte", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_SByte>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|for n in start..finish -> n|]``<sbyte> start finish

    [<Benchmark; BenchmarkCategory("SByte", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_SByte>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<sbyte> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_Byte () =
    member _.StartFinish : obj array seq =
        [
            [|box 0uy; 255uy|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|box 0uy; 2uy; 255uy|]
        ]

    [<Benchmark; BenchmarkCategory("Byte", "[|127uy..1uy|]")>]
    member _.M1 () = All.``[|127..1|]``<byte> ()

    [<Benchmark; BenchmarkCategory("Byte", "[|127uy..2uy..1uy|]")>]
    member _.M2 () = All.``[|127..2..1|]``<byte> ()

    [<Benchmark; BenchmarkCategory("Byte", "[|1uy..127uy|]")>]
    member _.M3 () = All.``[|1..127|]``<byte> ()

    [<Benchmark; BenchmarkCategory("Byte", "[|1uy..2uy..127uy|]")>]
    member _.M4 () = All.``[|1..2..127|]``<byte> ()

    [<Benchmark; BenchmarkCategory("Byte", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Byte>.StartFinish)>]
    member _.M5 (start, finish) = All.``[|start..finish|]``<byte> start finish

    [<Benchmark; BenchmarkCategory("Byte", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Byte>.StartStepFinish)>]
    member _.M6 (start, step, finish) = All.``[|start..step..finish|]``<byte> start step finish

    [<Benchmark; BenchmarkCategory("Byte", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Byte>.StartFinish)>]
    member _.M7 (start, finish) = All.``[|for n in start..finish -> n|]``<byte> start finish

    [<Benchmark; BenchmarkCategory("Byte", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Byte>.StartStepFinish)>]
    member _.M8 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<byte> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_Char () =
    member _.StartFinish : obj array seq =
        [
            [|'\000'; '\u7fff'|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|'\000'; '\002'; '\u7fff'|]
        ]

    [<Benchmark; BenchmarkCategory("Char", "'\127'..'\001'")>]
    member _.M1 () = All.``[|127..1|]``<char> ()

    [<Benchmark; BenchmarkCategory("Char", "'\127'..'\002'..'\001'")>]
    member _.M2 () = All.``[|127..2..1|]``<char> ()

    [<Benchmark; BenchmarkCategory("Char", "'\001'..'\127'")>]
    member _.M3 () = All.``[|1..127|]``<char> ()

    [<Benchmark; BenchmarkCategory("Char", "'\001'..'\002'..'\127'")>]
    member _.M4 () = All.``[|1..2..127|]``<char> ()

    [<Benchmark; BenchmarkCategory("Char", "'\001'..'\u7fff'")>]
    member _.M5 () = All.``[|1..32767|]``<char> ()

    [<Benchmark; BenchmarkCategory("Char", "'\001'..'\002'..'\u7fff'")>]
    member _.M6 () = All.``[|1..2..32767|]``<char> ()

    [<Benchmark; BenchmarkCategory("Char", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Char>.StartFinish)>]
    member _.M7 (start : char) finish = All.``[|start..finish|]``<char> start finish

    [<Benchmark; BenchmarkCategory("Char", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Char>.StartStepFinish)>]
    member _.M8 (start : char) step finish = All.``[|start..step..finish|]``<char> start step finish

    [<Benchmark; BenchmarkCategory("Char", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Char>.StartFinish)>]
    member _.M9 (start : char) finish = All.``[|for n in start..finish -> n|]``<char> start finish

    [<Benchmark; BenchmarkCategory("Char", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Char>.StartStepFinish)>]
    member _.M10 (start : char) step finish = All.``[|for n in start..step..finish -> n|]``<char> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_Int16 () =
    member _.StartFinish : obj array seq =
        [
            [|-32768s; 32767s|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|32767s; -1s; -32768s|]
            [|-32768s; 2s; 32767s|]
        ]

    [<Benchmark; BenchmarkCategory("Int16", "[|127s..1s|]")>]
    member _.M1 () = All.``[|127..1|]``<int16> ()

    [<Benchmark; BenchmarkCategory("Int16", "[|127s..2s..1s|]")>]
    member _.M2 () = All.``[|127..2..1|]``<int16> ()

    [<Benchmark; BenchmarkCategory("Int16", "[|1s..127s|]")>]
    member _.M3 () = All.``[|1..127|]``<int16> ()

    [<Benchmark; BenchmarkCategory("Int16", "[|1s..2s..127s|]")>]
    member _.M4 () = All.``[|1..2..127|]``<int16> ()

    [<Benchmark; BenchmarkCategory("Int16", "[|1s..32767s|]")>]
    member _.M5 () = All.``[|1..32767|]``<int16> ()

    [<Benchmark; BenchmarkCategory("Int16", "[|1s..2s..32767s|]")>]
    member _.M6 () = All.``[|1..2..32767|]``<int16> ()

    [<Benchmark; BenchmarkCategory("Int16", "[|32767s..-1s..-32768s|]")>]
    member _.M7 () = Signed.``[|32767..-1..-32768|]``<int16> ()

    [<Benchmark; BenchmarkCategory("Int16", "[|32767s..-2s..-32768s|]")>]
    member _.M8 () = Signed.``[|32767..-2..-32768|]``<int16> ()

    [<Benchmark; BenchmarkCategory("Int16", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int16>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|start..finish|]``<int16> start finish

    [<Benchmark; BenchmarkCategory("Int16", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int16>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|start..step..finish|]``<int16> start step finish

    [<Benchmark; BenchmarkCategory("Int16", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int16>.StartFinish)>]
    member _.M11 (start, finish) = All.``[|for n in start..finish -> n|]``<int16> start finish

    [<Benchmark; BenchmarkCategory("Int16", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int16>.StartStepFinish)>]
    member _.M12 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<int16> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_UInt16 () =
    member _.StartFinish : obj array seq =
        [
            [|box 0us; 32767us|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|box 0us; 2us; 32767us|]
        ]

    [<Benchmark; BenchmarkCategory("UInt16", "[|127us..1us|]")>]
    member _.M1 () = All.``[|127..1|]``<uint16> ()

    [<Benchmark; BenchmarkCategory("UInt16", "[|127us..2us..1us|]")>]
    member _.M2 () = All.``[|127..2..1|]``<uint16> ()

    [<Benchmark; BenchmarkCategory("UInt16", "[|1us..127us|]")>]
    member _.M3 () = All.``[|1..127|]``<uint16> ()

    [<Benchmark; BenchmarkCategory("UInt16", "[|1us..2us..127us|]")>]
    member _.M4 () = All.``[|1..2..127|]``<uint16> ()

    [<Benchmark; BenchmarkCategory("UInt16", "[|1us..32767us|]")>]
    member _.M5 () = All.``[|1..32767|]``<uint16> ()

    [<Benchmark; BenchmarkCategory("UInt16", "[|1us..2us..32767us|]")>]
    member _.M6 () = All.``[|1..2..32767|]``<uint16> ()

    [<Benchmark; BenchmarkCategory("UInt16", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt16>.StartFinish)>]
    member _.M7 (start, finish) = All.``[|start..finish|]``<uint16> start finish

    [<Benchmark; BenchmarkCategory("UInt16", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt16>.StartStepFinish)>]
    member _.M8 (start, step, finish) = All.``[|start..step..finish|]``<uint16> start step finish

    [<Benchmark; BenchmarkCategory("UInt16", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt16>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|for n in start..finish -> n|]``<uint16> start finish

    [<Benchmark; BenchmarkCategory("UInt16", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt16>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<uint16> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_Int32 () =
    member _.StartFinish : obj array seq =
        [
            [|-32768; 32767|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|32767; -1; -32768|]
            [|-32768; 2; 32767|]
        ]

    [<Benchmark; BenchmarkCategory("Int32", "[|127..1|]")>]
    member _.M1 () = All.``[|127..1|]``<int32> ()

    [<Benchmark; BenchmarkCategory("Int32", "[|127..2..1|]")>]
    member _.M2 () = All.``[|127..2..1|]``<int32> ()

    [<Benchmark; BenchmarkCategory("Int32", "[|1..127|]")>]
    member _.M3 () = All.``[|1..127|]``<int32> ()

    [<Benchmark; BenchmarkCategory("Int32", "[|1..2..127|]")>]
    member _.M4 () = All.``[|1..2..127|]``<int32> ()

    [<Benchmark; BenchmarkCategory("Int32", "[|1..32767|]")>]
    member _.M5 () = All.``[|1..32767|]``<int32> ()

    [<Benchmark; BenchmarkCategory("Int32", "[|1..2..32767|]")>]
    member _.M6 () = All.``[|1..2..32767|]``<int32> ()

    [<Benchmark; BenchmarkCategory("Int32", "[|32767..-1..-32768|]")>]
    member _.M7 () = Signed.``[|32767..-1..-32768|]``<int32> ()

    [<Benchmark; BenchmarkCategory("Int32", "[|32767..-2..-32768|]")>]
    member _.M8 () = Signed.``[|32767..-2..-32768|]``<int32> ()

    [<Benchmark; BenchmarkCategory("Int32", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int32>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|start..finish|]``<int32> start finish

    [<Benchmark; BenchmarkCategory("Int32", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int32>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|start..step..finish|]``<int32> start step finish

    [<Benchmark; BenchmarkCategory("Int32", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int32>.StartFinish)>]
    member _.M11 (start, finish) = All.``[|for n in start..finish -> n|]``<int32> start finish

    [<Benchmark; BenchmarkCategory("Int32", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int32>.StartStepFinish)>]
    member _.M12 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<int32> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_UInt32 () =
    member _.StartFinish : obj array seq =
        [
            [|0u; 32767u|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|0u; 2u; 32767u|]
        ]

    [<Benchmark; BenchmarkCategory("UInt32", "[|127u..1u|]")>]
    member _.M1 () = All.``[|127..1|]``<uint32> ()

    [<Benchmark; BenchmarkCategory("UInt32", "[|127u..2u..1u|]")>]
    member _.M2 () = All.``[|127..2..1|]``<uint32> ()

    [<Benchmark; BenchmarkCategory("UInt32", "[|1u..127u|]")>]
    member _.M3 () = All.``[|1..127|]``<uint32> ()

    [<Benchmark; BenchmarkCategory("UInt32", "[|1u..2u..127u|]")>]
    member _.M4 () = All.``[|1..2..127|]``<uint32> ()

    [<Benchmark; BenchmarkCategory("UInt32", "[|1u..32767u|]")>]
    member _.M5 () = All.``[|1..32767|]``<uint32> ()

    [<Benchmark; BenchmarkCategory("UInt32", "[|1u..2u..32767u|]")>]
    member _.M6 () = All.``[|1..2..32767|]``<uint32> ()

    [<Benchmark; BenchmarkCategory("UInt32", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt32>.StartFinish)>]
    member _.M7 (start, finish) = All.``[|start..finish|]``<uint32> start finish

    [<Benchmark; BenchmarkCategory("UInt32", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt32>.StartStepFinish)>]
    member _.M8 (start, step, finish) = All.``[|start..step..finish|]``<uint32> start step finish

    [<Benchmark; BenchmarkCategory("UInt32", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt32>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|for n in start..finish -> n|]``<uint32> start finish

    [<Benchmark; BenchmarkCategory("UInt32", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt32>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<uint32> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_Int64 () =
    member _.StartFinish : obj array seq =
        [
            [|-32768L; 32767L|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|32767L; -1L; -32768L|]
            [|-32768L; 2L; 32767L|]
        ]

    [<Benchmark; BenchmarkCategory("Int64", "[|127L..1L|]")>]
    member _.M1 () = All.``[|127..1|]``<int64> ()

    [<Benchmark; BenchmarkCategory("Int64", "[|127L..2L..1L|]")>]
    member _.M2 () = All.``[|127..2..1|]``<int64> ()

    [<Benchmark; BenchmarkCategory("Int64", "[|1L..127L|]")>]
    member _.M3 () = All.``[|1..127|]``<int64> ()

    [<Benchmark; BenchmarkCategory("Int64", "[|1L..2L..127L|]")>]
    member _.M4 () = All.``[|1..2..127|]``<int64> ()

    [<Benchmark; BenchmarkCategory("Int64", "[|1L..32767L|]")>]
    member _.M5 () = All.``[|1..32767|]``<int64> ()

    [<Benchmark; BenchmarkCategory("Int64", "[|1L..2L..32767L|]")>]
    member _.M6 () = All.``[|1..2..32767|]``<int64> ()

    [<Benchmark; BenchmarkCategory("Int64", "[|32767L..-1L..-32768L|]")>]
    member _.M7 () = Signed.``[|32767..-1..-32768|]``<int64> ()

    [<Benchmark; BenchmarkCategory("Int64", "[|32767L..-2L..-32768L|]")>]
    member _.M8 () = Signed.``[|32767..-2..-32768|]``<int64> ()

    [<Benchmark; BenchmarkCategory("Int64", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int64>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|start..finish|]``<int64> start finish

    [<Benchmark; BenchmarkCategory("Int64", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int64>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|start..step..finish|]``<int64> start step finish

    [<Benchmark; BenchmarkCategory("Int64", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int64>.StartFinish)>]
    member _.M11 (start, finish) = All.``[|for n in start..finish -> n|]``<int64> start finish

    [<Benchmark; BenchmarkCategory("Int64", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_Int64>.StartStepFinish)>]
    member _.M12 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<int64> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_UInt64 () =
    member _.StartFinish : obj array seq =
        [
            [|0UL; 32767UL|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|0UL; 2UL; 32767UL|]
        ]

    [<Benchmark; BenchmarkCategory("UInt64", "[|127UL..1UL|]")>]
    member _.M1 () = All.``[|127..1|]``<uint64> ()

    [<Benchmark; BenchmarkCategory("UInt64", "[|127UL..2UL..1UL|]")>]
    member _.M2 () = All.``[|127..2..1|]``<uint64> ()

    [<Benchmark; BenchmarkCategory("UInt64", "[|1UL..127UL|]")>]
    member _.M3 () = All.``[|1..127|]``<uint64> ()

    [<Benchmark; BenchmarkCategory("UInt64", "[|1UL..2UL..127UL|]")>]
    member _.M4 () = All.``[|1..2..127|]``<uint64> ()

    [<Benchmark; BenchmarkCategory("UInt64", "[|1UL..32767UL|]")>]
    member _.M5 () = All.``[|1..32767|]``<uint64> ()

    [<Benchmark; BenchmarkCategory("UInt64", "[|1UL..2UL..32767UL|]")>]
    member _.M6 () = All.``[|1..2..32767|]``<uint64> ()

    [<Benchmark; BenchmarkCategory("UInt64", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt64>.StartFinish)>]
    member _.M7 (start, finish) = All.``[|start..finish|]``<uint64> start finish

    [<Benchmark; BenchmarkCategory("UInt64", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt64>.StartStepFinish)>]
    member _.M8 (start, step, finish) = All.``[|start..step..finish|]``<uint64> start step finish

    [<Benchmark; BenchmarkCategory("UInt64", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt64>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|for n in start..finish -> n|]``<uint64> start finish

    [<Benchmark; BenchmarkCategory("UInt64", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UInt64>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<uint64> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_IntPtr () =
    member _.StartFinish : obj array seq =
        [
            [|-32768n; 32767n|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|32767n; -1n; -32768n|]
            [|-32768n; 2n; 32767n|]
        ]

    [<Benchmark; BenchmarkCategory("IntPtr", "[|127n..1n|]")>]
    member _.M1 () = All.``[|127..1|]``<nativeint> ()

    [<Benchmark; BenchmarkCategory("IntPtr", "[|127n..2n..1n|]")>]
    member _.M2 () = All.``[|127..2..1|]``<nativeint> ()

    [<Benchmark; BenchmarkCategory("IntPtr", "[|1n..127n|]")>]
    member _.M3 () = All.``[|1..127|]``<nativeint> ()

    [<Benchmark; BenchmarkCategory("IntPtr", "[|1n..2n..127n|]")>]
    member _.M4 () = All.``[|1..2..127|]``<nativeint> ()

    [<Benchmark; BenchmarkCategory("IntPtr", "[|1n..32767n|]")>]
    member _.M5 () = All.``[|1..32767|]``<nativeint> ()

    [<Benchmark; BenchmarkCategory("IntPtr", "[|1n..2n..32767n|]")>]
    member _.M6 () = All.``[|1..2..32767|]``<nativeint> ()

    [<Benchmark; BenchmarkCategory("IntPtr", "[|32767n..-1n..-32768n|]")>]
    member _.M7 () = Signed.``[|32767..-1..-32768|]``<nativeint> ()

    [<Benchmark; BenchmarkCategory("IntPtr", "[|32767n..-2n..-32768n|]")>]
    member _.M8 () = Signed.``[|32767..-2..-32768|]``<nativeint> ()

    [<Benchmark; BenchmarkCategory("IntPtr", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_IntPtr>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|start..finish|]``<nativeint> start finish

    [<Benchmark; BenchmarkCategory("IntPtr", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_IntPtr>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|start..step..finish|]``<nativeint> start step finish

    [<Benchmark; BenchmarkCategory("IntPtr", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_IntPtr>.StartFinish)>]
    member _.M11 (start, finish) = All.``[|for n in start..finish -> n|]``<nativeint> start finish

    [<Benchmark; BenchmarkCategory("IntPtr", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_IntPtr>.StartStepFinish)>]
    member _.M12 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<nativeint> start step finish

[<MemoryDiagnoser; CategoriesColumn; HideColumns("Method"); BenchmarkCategory("ComputedCollections", "Arrays", "IntegralRanges")>]
type ComputedCollections_Arrays_IntegralRanges_UIntPtr () =
    member _.StartFinish : obj array seq =
        [
            [|0un; 32767un|]
        ]

    member _.StartStepFinish : obj array seq =
        [
            [|0un; 2un; 32767un|]
        ]

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|127un..1un|]")>]
    member _.M1 () = All.``[|127..1|]``<unativeint> ()

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|127un..2un..1un|]")>]
    member _.M2 () = All.``[|127..2..1|]``<unativeint> ()

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|1un..127un|]")>]
    member _.M3 () = All.``[|1..127|]``<unativeint> ()

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|1un..2un..127un|]")>]
    member _.M4 () = All.``[|1..2..127|]``<unativeint> ()

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|1un..32767un|]")>]
    member _.M5 () = All.``[|1..32767|]``<unativeint> ()

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|1un..2un..32767un|]")>]
    member _.M6 () = All.``[|1..2..32767|]``<unativeint> ()

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|start..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UIntPtr>.StartFinish)>]
    member _.M7 (start, finish) = All.``[|start..finish|]``<unativeint> start finish

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|start..step..finish|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UIntPtr>.StartStepFinish)>]
    member _.M8 (start, step, finish) = All.``[|start..step..finish|]``<unativeint> start step finish

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|for n in start..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UIntPtr>.StartFinish)>]
    member _.M9 (start, finish) = All.``[|for n in start..finish -> n|]``<unativeint> start finish

    [<Benchmark; BenchmarkCategory("UIntPtr", "[|for n in start..step..finish -> n|]")>]
    [<ArgumentsSource(nameof Unchecked.defaultof<ComputedCollections_Arrays_IntegralRanges_UIntPtr>.StartStepFinish)>]
    member _.M10 (start, step, finish) = All.``[|for n in start..step..finish -> n|]``<unativeint> start step finish
