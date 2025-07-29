module ArraySlicing

open BenchmarkDotNet.Attributes
open System

[<SimpleJob(launchCount = 2, warmupCount = 1, iterationCount = 2)>]
[<GcServer(true)>]
[<MemoryDiagnoser>]
[<MarkdownExporterAttribute.GitHub>]
type ArraySlicingBenchmark() =
    let b = [| for _ in 1 .. 100_000 -> byte DateTime.Now.Ticks |]

    [<Benchmark>]
    member _.x () =
        b[ 20 .. 4999 ]