namespace MicroPerf

open BenchmarkDotNet.Running

module Main =

    [<EntryPoint>]
    let main args = 
        printfn "Running benchmarks..."
        BenchmarkSwitcher.FromAssembly(typeof<Equality.FSharpCoreFunctions>.Assembly).Run(args) |> ignore
        0
