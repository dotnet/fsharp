open BenchmarkDotNet.Running
open FSharp.Compiler.Benchmarks

[<EntryPoint>]
let main _ =
    //BenchmarkRunner.Run<CompilerService>() |> ignore
    BenchmarkRunner.Run<TypeCheckingBenchmark1>() |> ignore
    0
