module FSharp.Compiler.Service.Tests2.RunCompiler

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.Service.Tests2
open NUnit.Framework

type Node =
    {
        Idx : int
        Deps : int[]
        Dependants : int[]
        mutable PartialResult : string option
        mutable UnprocessedDepsCount : int
        _lock : Object
    }

[<Test>]
let runCompiler () =
    let args =
        System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
    FSharp.Compiler.CommandLineMain.main args |> ignore

[<Test>]
let runGrapher () =
    // let args =
    //     System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
    // FSharp.Compiler.CommandLineMain.main args |> ignore
    
    let fileDeps =
        [|
            0, [||]  // A
            1, [|0|] // B1 -> A
            2, [|1|] // B2 -> B1
            3, [|0|] // C1 -> A
            4, [|3|] // C2 -> C1
            5, [|2; 4|] // D -> B2, C2
        |]
        |> dict
        |> DepResolving.calcTransitiveGraph
        |> Seq.map (fun (KeyValue(k, v)) -> k, v)
        |> Seq.toArray
    
    let fileDependants =
        fileDeps
        // Collect all edges
        |> Array.collect (fun (idx, deps) -> deps |> Array.map (fun dep -> idx, dep))
        // Group dependants of the same dependencies together
        |> Array.groupBy (fun (idx, dep) -> dep)
        // Construct reversed graph
        |> Array.map (fun (dep, edges) -> dep, edges |> Array.map fst)
        |> dict
        // Add nodes that are missing due to having no dependants
        |> fun graph ->
            fileDeps
            |> Array.map (fun (idx, deps) ->
                match graph.TryGetValue idx with
                | true, dependants -> idx, dependants
                | false, _ -> idx, [||]
            )
        |> dict
    
    let graph =
        fileDeps
        |> Seq.map (fun (idx, deps) -> idx, {Idx = idx; Deps = deps; Dependants = fileDependants[idx]; PartialResult = None; UnprocessedDepsCount = deps.Length; _lock = Object()})
        |> dict
    
    printfn "start"
    use q = new BlockingCollection<int>()
    
    // Add leaves to the queue
    let filesWithoutDeps =
        graph
        |> Seq.filter (fun x -> x.Value.UnprocessedDepsCount = 0)
    filesWithoutDeps
    |> Seq.iter (fun f -> q.Add(f.Key))
    
    // Keep track of the number of items to be processed
    let l = Object()
    let mutable unprocessedCount = graph.Count
    
    let decrementProcessedCount () =
        lock l (fun () ->
            unprocessedCount <- unprocessedCount - 1
            printfn $"UnprocessedCount = {unprocessedCount}"
        )
    
    let actualWork (idx : int) =
        let node = graph[idx]
        let depsResult =
            node.Deps
            |> Array.map (fun dep -> match graph[dep].PartialResult with Some result -> result | None -> failwith $"Unexpected lack of result for a dependency {idx} -> {dep}")
            |> Array.fold (fun state item -> state + item) ""
        let thisResult = idx.ToString()
        $"{thisResult}"
    
    // Processing of a single node/file - gives a result
    let go (idx : int) =
        let node = graph[idx]
        printfn $"Start {idx} -> %+A{node.Deps}"
        Thread.Sleep(500)
        let res = actualWork idx
        node.PartialResult <- Some res
        printfn $" Stop {idx} work - result {res}"
        
        // Increment processed deps count for all dependants and schedule those who are now unblocked
        node.Dependants
        |> Array.iter (fun dependant ->
            let node = graph[dependant]
            let unprocessedDepsCount =
                lock node._lock (fun () ->
                    node.UnprocessedDepsCount <- node.UnprocessedDepsCount - 1
                    node.UnprocessedDepsCount
                )
            printfn $"{idx}'s dependant {dependant} now has {unprocessedDepsCount} unprocessed deps left"
            // Dependant is unblocked - schedule it
            if unprocessedDepsCount = 0 then
                printfn $"Scheduling {dependant}"
                q.Add(dependant)
        )
        
        printfn $"Quitting {idx}"
        decrementProcessedCount ()
        ()
        
    let workerWork (idx : int) =
        printfn $"start worker {idx}"
        q.GetConsumingEnumerable()
        |> Seq.iter go
        printfn $"end worker {idx}"
        
    let maxParallel = 4
    printfn "workers"
    let workers =
        [|1..maxParallel|]
        |> Array.map (fun idx -> Task.Factory.StartNew(fun () -> workerWork idx))
        
    while unprocessedCount > 0 do
        Thread.Sleep(100)
    
    printfn "CompleteAdding"
    q.CompleteAdding()
    printfn "waitall"
    Task.WaitAll workers
    
    let fullResult =
        graph
        |> Seq.map (fun (KeyValue(idx, node)) -> node.PartialResult |> Option.get) // TODO Oops
        |> Seq.fold (fun state item -> state + item) ""
    
    printfn $"End result: {fullResult}"
