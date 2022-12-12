open BenchmarkDotNet.Running
open FSharp.Compiler.Benchmarks
open BenchmarkDotNet.Configs

[<EntryPoint>]
let main args =
    BenchmarkSwitcher.FromAssembly(typeof<DecentlySizedStandAloneFileBenchmark>.Assembly).Run(args, new DebugInProcessConfig()) |> ignore
    0
