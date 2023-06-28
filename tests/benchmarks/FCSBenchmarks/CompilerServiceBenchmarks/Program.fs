open BenchmarkDotNet.Running
open FSharp.Compiler.Benchmarks
open BenchmarkDotNet.Configs

[<EntryPoint>]
let main args =
    let cfg = ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator)
    BenchmarkSwitcher.FromAssembly(typeof<DecentlySizedStandAloneFileBenchmark>.Assembly).Run(args,cfg) |> ignore
    0
