/// Phase 2b: CastThenBr for interface join points
/// Tests match expressions returning interface types
/// Changed: adds castclass before join point branch when result is interface type
module ILGenCodegen.CastThenBr

open System
open System.Collections.Generic
open BenchmarkDotNet.Attributes

type IShape =
    abstract Area: unit -> float

type Circle(r: float) =
    interface IShape with
        member _.Area() = Math.PI * r * r

type Square(s: float) =
    interface IShape with
        member _.Area() = s * s

type Triangle(b: float, h: float) =
    interface IShape with
        member _.Area() = 0.5 * b * h

[<MemoryDiagnoser>]
type CastThenBrBenchmark() =

    let rng = Random(42)
    let indices = Array.init 10_000 (fun _ -> rng.Next(3))
    // Pre-allocate objects to eliminate allocation noise
    let circle = Circle(1.0) :> IShape
    let square = Square(2.0) :> IShape
    let triangle = Triangle(3.0, 4.0) :> IShape

    [<Benchmark>]
    member _.MatchReturningInterface_NoAlloc() =
        let mutable total = 0.0
        for i in indices do
            let shape: IShape =
                match i with
                | 0 -> circle
                | 1 -> square
                | _ -> triangle
            total <- total + shape.Area()
        total

    [<Benchmark>]
    member _.MatchReturningInterface_Alloc() =
        let mutable total = 0.0
        for i in indices do
            let shape: IShape =
                match i with
                | 0 -> Circle(1.0)
                | 1 -> Square(2.0)
                | _ -> Triangle(3.0, 4.0)
            total <- total + shape.Area()
        total

    [<Benchmark>]
    member _.MatchReturningIComparable() =
        let mutable sum = 0
        for i in indices do
            let c: IComparable =
                match i with
                | 0 -> 42 :> IComparable
                | 1 -> "hello" :> IComparable
                | _ -> DateTime.UtcNow :> IComparable
            sum <- sum + c.GetHashCode()
        sum
