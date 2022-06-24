open BenchmarkDotNet.Running
open FSharp.Compiler.Benchmarks

[<EntryPoint>]
let main _ =
    //BenchmarkRunner.Run<CompilerServiceBenchmarks>() |> ignore
    BenchmarkRunner.Run<TypeCheckingBenchmark>() |> ignore
    0
