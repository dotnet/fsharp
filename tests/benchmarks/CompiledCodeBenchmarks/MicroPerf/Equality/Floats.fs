namespace Equality

open BenchmarkDotNet.Attributes

[<MemoryDiagnoser>]
type Floats() =

    let numbers = Array.init 1000 (fun id -> id % 7)

    [<Benchmark>]
    member _.FloatER() = numbers |> Array.groupBy float

    [<Benchmark>]
    member _.Float32ER() = numbers |> Array.groupBy float32

    [<Benchmark>]
    member _.FloatPER() = numbers |> Array.Parallel.groupBy float

    [<Benchmark>]
    member _.Float32PER() = numbers |> Array.Parallel.groupBy float32