module ForLoops

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Diagnosers
open System.Runtime.CompilerServices

[<DisassemblyDiagnoser>]
[<MarkdownExporterAttribute.GitHub>]
type ForLoopBenchmark() =

    [<Params(100)>]
    member val Start = 0 with get, set

    [<Params(1000)>]
    member val Finish = 0 with get, set

    [<Params(10)>]
    member val Step = 0 with get, set

    [<Benchmark>]
    member this.Benchmark() =
        let mutable x = 0
        for i in this.Start .. this.Finish .. this.Step do
            x <- i
