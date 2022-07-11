(*
msbuild tests\fsharp\perf\tasks\FS\TaskPerf.fsproj /p:Configuration=Release
dotnet artifacts\bin\TaskPerf\Release\netcoreapp2.1\TaskPerf.dll
*)

namespace TaskPerf

open BenchmarkDotNet.Running

module Main = 

    [<EntryPoint>]
    let main argv = 
        printfn "Running benchmarks..."
        //let results = BenchmarkRunner.Run<Benchmarks>()
        //let results = BenchmarkRunner.Run<Async.AsyncWhileMemoryBench>()
        let results = BenchmarkRunner.Run<Collections.CollectionsBenchmark>()
        0  
