open BenchmarkDotNet.Running
open FSharp.Compiler.Benchmarks

[<EntryPoint>]
let main args =
    BenchmarkSwitcher.FromAssembly(typeof<TypeCheckingBenchmark>.Assembly).Run(args) |> ignore
    0
