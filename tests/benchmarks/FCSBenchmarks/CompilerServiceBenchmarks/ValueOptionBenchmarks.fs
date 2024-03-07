namespace FSharp.Compiler.Benchmarks

open System
open System.Threading.Tasks
open BenchmarkDotNet.Attributes
open Internal.Utilities.Collections.Utils
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Benchmarks.Common.Categories

[<MemoryDiagnoser>]
[<BenchmarkCategory(ShortCategory)>]
type ValueOptionBenchmarks() =

    let taskCanceledException = TaskCanceledException()
    let stopProcessingException = StopProcessingExn(None)
    let otherException = Exception()
    let constBool = Const.Bool true
    let constOther = Const.Unit

    [<Benchmark>]
    member _.TaskCancelled_TargetPattern() = 
        match taskCanceledException with
        | TaskCancelled _
        | _ -> ()

    [<Benchmark>]
    member _.TaskCancelled_OtherPattern() = 
        match otherException with
        | TaskCancelled _
        | _ -> ()

    [<Benchmark>]
    member _.StopProcessing_TargetPattern() = 
        match stopProcessingException with
        | StopProcessing _
        | _ -> ()

    [<Benchmark>]
    member _.StopProcessing_OtherPattern() = 
        match otherException with
        | StopProcessing _
        | _ -> ()

    [<Benchmark>]
    member _.ConstToILFieldInit_TargetPattern() = 
        match constBool with
        | ConstToILFieldInit _ 
        | _ -> ()

    [<Benchmark>]
    member _.ConstToILFieldInit_OtherPattern() = 
        match constOther with
        | ConstToILFieldInit _ 
        | _ -> ()
