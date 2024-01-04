namespace TaskPerf

open BenchmarkDotNet.Running

module Main = 

    [<EntryPoint>]
    let main _ = 
        printfn "Running benchmarks..."
        let _ = BenchmarkRunner.Run<Collections.CollectionsBenchmark>()
        0  
