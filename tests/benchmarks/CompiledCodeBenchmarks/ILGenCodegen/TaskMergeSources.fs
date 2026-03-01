/// Phase 4: Witness field initialization fix in state machine structs
/// Tests task CE with multiple sources (let! ... and! ...)
/// Changed: correct field init order in GenStructStateMachine for $W methods
module ILGenCodegen.TaskMergeSources

open System.Threading.Tasks
open BenchmarkDotNet.Attributes

[<MemoryDiagnoser>]
type TaskMergeSourcesBenchmark() =

    [<Benchmark>]
    member _.TaskLetBangAndBang() =
        let t =
            task {
                let! a = Task.FromResult 1
                and! b = Task.FromResult 2
                return a + b
            }
        t.Result

    [<Benchmark>]
    member _.TaskLetBangAndBang3() =
        let t =
            task {
                let! a = Task.FromResult 1
                and! b = Task.FromResult 2
                and! c = Task.FromResult 3
                return a + b + c
            }
        t.Result

    [<Benchmark>]
    member _.TaskLetBangSequential() =
        let t =
            task {
                let! a = Task.FromResult 1
                let! b = Task.FromResult 2
                return a + b
            }
        t.Result

    [<Benchmark>]
    member _.TaskSimple() =
        let t = task { return 42 }
        t.Result
