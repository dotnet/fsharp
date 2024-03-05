namespace Equality

open BenchmarkDotNet.Attributes

[<MemoryDiagnoser>]
type BasicTypes() =

    let bools = Array.init 1000 (fun n -> n % 2 = 0)
    let sbytes = Array.init 1000 sbyte
    let bytes = Array.init 1000 byte
    let int16s = Array.init 1000 int16
    let uint16s = Array.init 1000 uint16
    let int32s = Array.init 1000 id
    let uint32s = Array.init 1000 uint32
    let int64s = Array.init 1000 int64
    let uint64s = Array.init 1000 uint64
    let intptrs = Array.init 1000 nativeint
    let uintptrs = Array.init 1000 unativeint
    let chars = Array.init 1000 char
    let strings = Array.init 1000 string
    let decimals = Array.init 1000 decimal

    [<Benchmark>]
    member _.Bool() = bools |> Array.distinct

    [<Benchmark>]
    member _.SByte() = sbytes |> Array.distinct

    [<Benchmark>]
    member _.Byte() = bytes |> Array.distinct

    [<Benchmark>]
    member _.Int16() = int16s |> Array.distinct

    [<Benchmark>]
    member _.UInt16() = uint16s |> Array.distinct
    
    [<Benchmark>]
    member _.Int32() = int32s |> Array.distinct

    [<Benchmark>]
    member _.UInt32() = uint32s |> Array.distinct

    [<Benchmark>]
    member _.Int64() = int64s |> Array.distinct

    [<Benchmark>]
    member _.UInt64() = uint64s |> Array.distinct

    [<Benchmark>]
    member _.IntPtr() = intptrs |> Array.distinct

    [<Benchmark>]
    member _.UIntPtr() = uintptrs |> Array.distinct

    [<Benchmark>]
    member _.Char() = chars |> Array.distinct

    [<Benchmark>]
    member _.String() = strings |> Array.distinct

    [<Benchmark>]
    member _.Decimal() = decimals |> Array.distinct
