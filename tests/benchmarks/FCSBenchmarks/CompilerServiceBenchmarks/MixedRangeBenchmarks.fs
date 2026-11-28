module FSharp.Benchmarks.MixedRangeBenchmarks

open BenchmarkDotNet.Attributes
open FSharp.Compiler.CodeAnalysis
open FSharp.Test.ProjectGeneration
open FSharp.Benchmarks.Common.Categories

let private literal count element =
    let elements = [ for i in 0 .. count - 1 -> element i ] |> String.concat "; "
    $"let xs = [ {elements} ]"

let private flat count = literal count string

let private mixed count =
    literal count (fun i -> if i % 10 = 0 then $"{i}..{i + 1}" else string i)

let private explicitYield count =
    literal count (fun i -> if i % 10 = 0 then $"yield! [ {i}..{i + 1} ]" else $"yield {i}")

[<MemoryDiagnoser>]
[<SimpleJob(warmupCount = 1, iterationCount = 4)>]
[<BenchmarkCategory(LongCategory)>]
type MixedRangeBenchmarks() =

    [<Params(250, 500, 1_000)>]
    member val Count = 0 with get, set

    member val Benchmark = Unchecked.defaultof<_> with get, set

    member this.setup(extraSource) =
        let project =
            { SyntheticProject.Create() with
                SourceFiles = [ { sourceFile "File" [] with ExtraSource = extraSource } ]
                OtherOptions = [ "--langversion:preview" ] }

        let checker = FSharpChecker.Create()
        this.Benchmark <- ProjectWorkflowBuilder(project, checker = checker).CreateBenchmarkBuilder()
        saveProject project false checker |> Async.RunSynchronously

    [<IterationSetup>]
    member this.StartIteration() =
        this.Benchmark.Checker.InvalidateAll()
        this.Benchmark.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    [<GlobalSetup(Target = "CheckFlat")>]
    member this.SetupFlat() = this.setup (flat this.Count)

    [<GlobalSetup(Target = "CheckMixedRange")>]
    member this.SetupMixedRange() = this.setup (mixed this.Count)

    [<GlobalSetup(Target = "CheckExplicitYield")>]
    member this.SetupExplicitYield() = this.setup (explicitYield this.Count)

    [<Benchmark(Baseline = true)>]
    member this.CheckFlat() =
        this.Benchmark { checkFile "File" expectOk }

    [<Benchmark>]
    member this.CheckMixedRange() =
        this.Benchmark { checkFile "File" expectOk }

    [<Benchmark>]
    member this.CheckExplicitYield() =
        this.Benchmark { checkFile "File" expectOk }
