
open System.IO
open System.Reflection
open BenchmarkDotNet.Running
open BenchmarkComparison

[<EntryPoint>]
let main args =
    BenchmarkSwitcher.FromAssembly(typeof<TypeCheckingBenchmark>.Assembly).Run(args) |> ignore
    0

