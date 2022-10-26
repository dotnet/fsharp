module FSharp.Compiler.Service.Tests2.RunCompiler

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.Service.Tests.Graph
open NUnit.Framework

[<Test>]
let runCompiler () =
    let args =
        System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
    FSharp.Compiler.CommandLineMain.main args |> ignore
    
[<CustomEquality; NoComparison>]
type Node =
    {
        Idx : FileIdx
        Deps : FileIdx[]
        TransitiveDeps : FileIdx[]
        Dependants : FileIdx[]
        mutable PartialResult : string option
        mutable ThisResult : int option
        mutable UnprocessedDepsCount : int
        _lock : Object
    }
    override this.Equals(y) =
        match y with
        | :? Node as other -> (this.Idx = other.Idx)
        | _ -> false
    override this.GetHashCode() = this.Idx.Idx

type State = string // TcState
type SingleResult = int // partial result for a single file 
    
/// <summary>
/// Combine results of all transitive dependencies for a single target node.
/// </summary>
/// <param name="graph"></param>
/// <param name="deps">Transitive deps</param>
let combineResults (graph : IDictionary<FileIdx, Node>) (node : Node) (folder : State -> SingleResult -> State) : State =
    
    // Find the child with most transitive deps
    let biggestChild =
        node.TransitiveDeps
        |> Array.map (fun d -> graph[d])
        |> Array.maxBy (fun n -> n.TransitiveDeps.Length)
    
    // Start with that child's state
    let state = biggestChild.PartialResult |> Option.defaultWith (fun () -> failwith "Unexpected lack of result")
    
    let alreadyIncluded = HashSet<FileIdx>(biggestChild.TransitiveDeps, HashIdentity.Structural)
    
    // Find individual results from all transitive deps that were not in biggestChild
    let toBeAdded =
        node.TransitiveDeps
        |> Array.filter alreadyIncluded.Add
    
    // Add those results to the initial one
    let state =
        toBeAdded
        |> Array.map (fun d -> graph[d].ThisResult |> Option.defaultWith (fun () -> failwith "Unexpected lack of result"))
        |> Array.fold folder state
        
    state

let processGraph (graph : IDictionary<FileIdx, Node>) =
    
    printfn "start"
    use q = new BlockingCollection<FileIdx>()
    
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
    
    let fold (state : string) (singleResult : int) =
        state + singleResult.ToString()
    
    let actualActualWork (idx : FileIdx) (state : State) : SingleResult * State =
        let thisResult = idx.Idx
        let state = fold state thisResult
        thisResult, state
    
    let actualWork (idx : FileIdx) =
        let node = graph[idx]
        let state = combineResults graph node fold
        let thisResult = actualActualWork idx state
        thisResult
    
    // Processing of a single node/file - gives a result
    let go (idx : FileIdx) =
        let node = graph[idx]
        printfn $"Start {idx} -> %+A{node.Deps}"
        Thread.Sleep(500)
        let singleResult, state = actualWork idx
        node.ThisResult <- Some singleResult
        node.PartialResult <- Some state
        printfn $" Stop {idx} work - SingleResult={singleResult} State={state}"
        
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


[<Test>]
let runGrapher () =
    // let args =
    //     System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
    // FSharp.Compiler.CommandLineMain.main args |> ignore
    
    let deps : FileGraph =
        [|
            0, [||]  // A
            1, [|0|] // B1 -> A
            2, [|1|] // B2 -> B1
            3, [|0|] // C1 -> A
            4, [|3|] // C2 -> C1
            5, [|2; 4|] // D -> B2, C2
        |]
        |> Array.map (fun (a, deps) -> FileIdx.make a, deps |> Array.map FileIdx.make)
        |> readOnlyDict
    
    let dependants = deps |> FileGraph.reverse
    
    let transitiveDeps = deps |> FileGraph.calcTransitiveGraph
    let transitiveDependants = transitiveDeps |> FileGraph.reverse
    
    let graph =
        transitiveDeps
        |> Seq.map (fun (KeyValue(idx, deps)) -> idx, {Idx = idx; Deps = deps; Dependants = dependants[idx]; TransitiveDeps = transitiveDependants[idx]; ThisResult = None; PartialResult = None; UnprocessedDepsCount = deps.Length; _lock = Object()})
        |> dict
    
    processGraph graph