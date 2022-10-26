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
type GenericNode<'State, 'SingleResult> =
    {
        Idx : FileIdx
        mutable Deps : GenericNode<'State, 'SingleResult>[]
        mutable TransitiveDeps : GenericNode<'State, 'SingleResult>[]
        mutable Dependants : GenericNode<'State, 'SingleResult>[]
        mutable Result : ('SingleResult * 'State) option
        mutable UnprocessedDepsCount : int
        _lock : Object
    }
    override this.Equals(y) =
        match y with
        | :? GenericNode<'State, 'SingleResult> as other -> (this.Idx = other.Idx)
        | _ -> false
    override this.GetHashCode() = this.Idx.Idx
    override this.ToString() = this.Idx.ToString()

module Node =
    let idx (node : GenericNode<_,_>) = node.Idx

type State = string // TcState
type SingleResult = int // partial result for a single file 

type Node = GenericNode<State, SingleResult>

/// <summary>
/// Combine results of all transitive dependencies
/// </summary>
/// <param name="graph"></param>
/// <param name="deps">Transitive deps</param>
let combineResults<'State, 'SingleResult>
    (transitiveDeps : GenericNode<'State, 'SingleResult>[])
    (folder : 'State -> 'SingleResult -> 'State) : 'State
    =
    // Find the child with most transitive deps
    let biggestChild =
        transitiveDeps
        |> Array.maxBy (fun n -> n.TransitiveDeps.Length)
    
    // Start with that child's state
    let state = biggestChild.Result |> Option.defaultWith (fun () -> failwith "Unexpected lack of result") |> snd
    
    let alreadyIncluded = HashSet<GenericNode<'State, 'SingleResult>>(biggestChild.TransitiveDeps, HashIdentity.Structural)
    
    // Find individual results from all transitive deps that were not in biggestChild
    let toBeAdded =
        transitiveDeps
        |> Array.filter alreadyIncluded.Add
    
    // Add those results to the initial one
    let state =
        toBeAdded
        |> Array.map (fun d -> d.Result |> Option.defaultWith (fun () -> failwith "Unexpected lack of result") |> fst)
        |> Array.fold folder state
        
    state
    
let fold (state : string) (singleResult : int) =
    state + singleResult.ToString()

let actualActualWork (idx : FileIdx) (state : State) : SingleResult =
    let thisResult = idx.Idx
    thisResult
        
let processGraph<'State, 'SingleResult>
    (graph : GenericNode<'State, 'SingleResult>[])
    (work : GenericNode<'State, 'SingleResult> -> 'State -> 'SingleResult)
    (folder : 'State -> 'SingleResult -> 'State)
    =
    printfn "start"
    use q = new BlockingCollection<GenericNode<'State, 'SingleResult>>()
    
    // Add leaves to the queue
    let filesWithoutDeps =
        graph
        |> Seq.filter (fun x -> x.UnprocessedDepsCount = 0)
        |> Seq.iter (fun f -> q.Add(f))
    
    // Keep track of the number of items to be processed
    let _lock = Object()
    let mutable unprocessedCount = graph.Length
    
    let decrementProcessedCount () =
        lock _lock (fun () ->
            unprocessedCount <- unprocessedCount - 1
            printfn $"UnprocessedCount = {unprocessedCount}"
        )
    
    // Processing of a single node/file
    let go (node : GenericNode<'State, 'SingleResult>) : unit =
        printfn $"Start {node} -> %+A{node.Deps}"
        Thread.Sleep(500)
        let state = combineResults node.TransitiveDeps folder
        let singleResult = work node state
        node.Result <- Some (singleResult, state)
        printfn $" Stop {node} work - SingleResult={singleResult} State={state}"
        
        // Increment processed deps count for all dependants and schedule those who are now unblocked
        node.Dependants
        |> Array.iter (fun dependant ->
            let unprocessedDepsCount =
                lock dependant._lock (fun () ->
                    dependant.UnprocessedDepsCount <- dependant.UnprocessedDepsCount - 1
                    dependant.UnprocessedDepsCount
                )
            printfn $"{node}'s dependant {dependant} now has {unprocessedDepsCount} unprocessed deps left"
            // Dependant is unblocked - schedule it
            if unprocessedDepsCount = 0 then
                printfn $"Scheduling {dependant}"
                q.Add(dependant)
        )
        
        printfn $"Quitting {node}"
        decrementProcessedCount ()
        ()
        
    let workerWork (idx : int) =
        printfn $"start worker {idx}"
        q.GetConsumingEnumerable()
        |> Seq.iter go
        printfn $"end worker {idx}"
        
    let maxParallel = 4 // TODO Change - base on CPU count?
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
    
    let fullResult = combineResults graph
    
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
    
    let nodes =
        deps.Keys
        |> Seq.map (fun idx -> idx, {Idx = idx; Deps = [||]; Dependants = [||]; TransitiveDeps = [||]; Result = None; UnprocessedDepsCount = 0; _lock = Object()})
        |> readOnlyDict
    
    let processs deps = deps |> Array.map (fun d -> nodes[d])
    
    let graph =
        nodes
        |> Seq.iter (fun (KeyValue(idx, node)) ->
            node.Deps <- processs deps[idx]
            node.TransitiveDeps <- processs transitiveDeps[idx]
            node.Dependants <- processs dependants[idx]
            node.UnprocessedDepsCount <- node.Deps.Length
        )
        nodes.Values |> Seq.toArray
    
    processGraph graph