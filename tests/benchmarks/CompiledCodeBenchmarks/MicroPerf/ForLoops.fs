module ForLoops

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Diagnosers
open System.Runtime.CompilerServices

[<DisassemblyDiagnoser>]
[<MarkdownExporterAttribute.GitHub>]
type ForLoopBenchmark() =

    [<Params(100)>]
    member val Start = 0 with get, set

    [<Params(1000, 1000000)>]
    member val Finish = 0 with get, set

    [<Params(10)>]
    member val Step = 0 with get, set

    [<Benchmark>]
    member this.VariableStep() =
        let mutable x = 0
        for i in this.Start .. this.Step .. this.Finish do
            x <- i

    [<Benchmark>]
    member this.ConstantStep() =
        let mutable x = 0
        for i in this.Start .. 10 .. this.Finish do
            x <- i
