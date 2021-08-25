
open System.IO
open System.Reflection
open BenchmarkDotNet.Running
open BenchmarkComparison

[<EntryPoint>]
let main _ =
    BenchmarkRunner.Run<TypeCheckingBenchmark1>() |> ignore
    0

