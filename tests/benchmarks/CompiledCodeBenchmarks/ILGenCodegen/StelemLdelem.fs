/// Phase 1: stelem/ldelem specialization
/// Tests array read/write performance for primitive types
/// Changed: ldelem System.Boolean → ldelem.u1, stelem System.Int32 → stelem.i4, etc.
module ILGenCodegen.StelemLdelem

open BenchmarkDotNet.Attributes

[<MemoryDiagnoser>]
type StelemLdelemBenchmark() =

    let boolArr = Array.create 10_000 true
    let intArr = Array.init 10_000 id
    let charArr = Array.create 10_000 'x'
    let sbyteArr = Array.create 10_000 42y
    let byteArr = Array.create 10_000 42uy

    [<Benchmark>]
    member _.BoolArrayReadWrite() =
        let arr = boolArr
        let mutable sum = 0
        for i = 0 to arr.Length - 1 do
            if arr.[i] then sum <- sum + 1
            arr.[i] <- (i % 2 = 0)
        sum

    [<Benchmark>]
    member _.IntArrayReadWrite() =
        let arr = intArr
        let mutable sum = 0
        for i = 0 to arr.Length - 1 do
            sum <- sum + arr.[i]
            arr.[i] <- i
        sum

    [<Benchmark>]
    member _.CharArrayReadWrite() =
        let arr = charArr
        let mutable sum = 0
        for i = 0 to arr.Length - 1 do
            sum <- sum + int arr.[i]
            arr.[i] <- char (i % 128)
        sum

    [<Benchmark>]
    member _.SByteArrayReadWrite() =
        let arr = sbyteArr
        let mutable sum = 0
        for i = 0 to arr.Length - 1 do
            sum <- sum + int arr.[i]
            arr.[i] <- sbyte (i % 127)
        sum

    [<Benchmark>]
    member _.ByteArrayReadWrite() =
        let arr = byteArr
        let mutable sum = 0
        for i = 0 to arr.Length - 1 do
            sum <- sum + int arr.[i]
            arr.[i] <- byte (i % 256)
        sum

    [<Benchmark>]
    member _.IntArrayFilterToArray() =
        intArr |> Array.filter (fun x -> x % 2 = 0) |> ignore

    [<Benchmark>]
    member _.BoolArrayCountTrue() =
        let mutable count = 0
        for b in boolArr do
            if b then count <- count + 1
        count
