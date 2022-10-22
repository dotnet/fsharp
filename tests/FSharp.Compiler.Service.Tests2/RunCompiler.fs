module FSharp.Compiler.Service.Tests.RunCompiler

open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open NUnit.Framework

[<Test>]
let runCompiler () =
    // let args =
    //     System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
    // FSharp.Compiler.CommandLineMain.main args |> ignore
    
    let go (idx : int) =
        printfn $"{idx} start"
        Thread.Sleep(1000)
        printfn $"{idx} stop"
    
    printfn "start"
    use q = new BlockingCollection<int>()
    let work (idx : int) =
        printfn $"start worker {idx}"
        q.GetConsumingEnumerable()
        |> Seq.iter go
        printfn $"end worker {idx}"
    let maxParallel = 4
    printfn "workers"
    let workers =
        [|1..maxParallel|]
        |> Array.map (fun idx -> Task.Factory.StartNew(fun () -> work idx))
    
    printfn "adding"
    for i in [|1..20|] do
        q.Add(i)
        Thread.Sleep(300)
    printfn "CompleteAdding"
    q.CompleteAdding()
    printfn "waitall"
    Task.WaitAll workers
    printfn "End"
    ()
