/// Phase 2a: callvirt → call for value type methods
/// Tests struct GetHashCode/ToString/Equals performance
/// Changed: callvirt on unboxed value type → call (when boxity=AsValue)
module ILGenCodegen.CallVirtOnValueType

open System
open System.Collections.Generic
open BenchmarkDotNet.Attributes

[<Struct; CustomEquality; NoComparison>]
type MyPoint =
    val X: int
    val Y: int
    new(x, y) = { X = x; Y = y }
    override this.GetHashCode() = this.X ^^^ (this.Y <<< 16)
    override this.Equals(other) =
        match other with
        | :? MyPoint as o -> this.X = o.X && this.Y = o.Y
        | _ -> false
    override this.ToString() = sprintf "(%d,%d)" this.X this.Y

[<MemoryDiagnoser>]
type CallVirtOnValueTypeBenchmark() =

    let points = Array.init 10_000 (fun i -> MyPoint(i, i * 2))
    let _rng = Random(42)

    [<Benchmark>]
    member _.StructGetHashCode() =
        let mutable sum = 0
        for p in points do
            sum <- sum + p.GetHashCode()
        sum

    [<Benchmark>]
    member _.StructToString() =
        let mutable len = 0
        for i = 0 to 99 do
            len <- len + points.[i].ToString().Length
        len

    [<Benchmark>]
    member _.StructEquals() =
        let mutable count = 0
        for i = 0 to points.Length - 2 do
            if points.[i].Equals(points.[i + 1]) then
                count <- count + 1
        count

    [<Benchmark>]
    member _.StructInDictionary() =
        let dict = Dictionary<MyPoint, int>()
        for i = 0 to 999 do
            dict.[points.[i]] <- i
        let mutable sum = 0
        for i = 0 to 999 do
            sum <- sum + dict.[points.[i]]
        sum

    [<Benchmark>]
    member _.IntGetHashCode() =
        let mutable sum = 0
        for i = 0 to 9999 do
            sum <- sum + i.GetHashCode()
        sum

    [<Benchmark>]
    member _.DateTimeGetHashCode() =
        let now = DateTime.UtcNow
        let mutable sum = 0
        for _ = 0 to 9999 do
            sum <- sum + now.GetHashCode()
        sum
