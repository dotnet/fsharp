open BenchmarkDotNet.Running
open FSharp.Compiler.Benchmarks

[<EntryPoint>]
let main args =
    BenchmarkSwitcher.FromAssembly(typeof<DecentlySizedStandAloneFileBenchmark>.Assembly).Run(args) |> ignore
    0
