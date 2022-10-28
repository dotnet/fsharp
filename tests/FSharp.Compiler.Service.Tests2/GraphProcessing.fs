module FSharp.Compiler.Service.Tests.GraphProcessing

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading

/// Used for processing
type NodeInfo<'Item> =
    {
        Item : 'Item
        Deps : 'Item[]
        TransitiveDeps : 'Item[]
        Dependants : 'Item[]
        ProcessedDepsCount : int
    }
type Node<'Item, 'State, 'Result> =
    {
        Info : NodeInfo<'Item>
        Result : ('State * 'Result) option
    }

// TODO Do we need to suppress some error logging if we
// TODO apply the same partial results multiple times?
// TODO Maybe we can enable logging only for the final fold
/// <summary>
/// Combine results of dependencies needed to type-check a 'higher' node in the graph 
/// </summary>
/// <param name="deps">Direct dependencies of a node</param>
/// <param name="transitiveDeps">Transitive dependencies of a node</param>
/// <param name="folder">A way to fold a single result into existing state</param>
let combineResults
    (deps : Node<'Item, 'State, 'Result>[])
    (transitiveDeps : Node<'Item, 'State, 'Result>[])
    (folder : 'State -> 'Result -> 'State)
    : 'State
    =
    let biggestDep =
        let sizeMetric node =
            // Could also use eg. total file size/AST size
            node.Info.TransitiveDeps.Length
        deps
        |> Array.maxBy sizeMetric
    let orFail value =
        value
        |> Option.defaultWith (fun () -> failwith "Unexpected lack of result")
    let firstState =
        biggestDep.Result
        |> orFail
        |> fst
    
    // TODO Potential perf optimisation: Keep transDeps in a HashSet from the start,
    // avoiding reconstructing the HashSet here
    
    // Add single-file results of remaining transitive deps one-by-one using folder
    // Note: Good to preserve order here so that folding happens in file order
    let included = HashSet(biggestDep.Info.TransitiveDeps)
    let resultsToAdd =
        transitiveDeps 
        |> Array.filter (fun dep -> included.Contains dep.Info.Item = false)
        |> Array.map (fun dep ->
            dep.Result
            |> orFail
            |> snd
        )
    let state = Array.fold folder firstState resultsToAdd
    state
    
   
// TODO Test this version
/// Untested version that uses MailboxProcessor.
/// See http://www.fssnip.net/nX/title/Limit-degree-of-parallelism-using-an-agent for implementation
let processInParallelUsingMailbox
    (firstItems : 'Item[])
    (work : 'Item -> Async<'Item[]>)
    (parallelism : int)
    (notify : int -> unit)
    (ct : CancellationToken)
    : unit
    =
    let processedCountLock = Object()
    let mutable processedCount = 0
    let agent = Parallel.threadingLimitAgent 10 ct
    let rec processItem item =
        async {
            let! toSchedule = work item
            let pc = lock processedCountLock (fun () -> processedCount <- processedCount + 1; processedCount)
            notify pc
            toSchedule |> Array.iter (fun x -> agent.Post(Parallel.Start(processItem x)))   
        }
    firstItems |> Array.iter (fun x -> agent.Post(Parallel.Start(processItem x)))
    ()    
    
// TODO Could replace with MailboxProcessor+Tasks/Asyncs instead of BlockingCollection + Threads
// See http://www.fssnip.net/nX/title/Limit-degree-of-parallelism-using-an-agent    
let processInParallel
    (firstItems : 'Item[])
    (work : 'Item -> 'Item[])
    (parallelism : int)
    (stop : int -> bool)
    (ct : CancellationToken)
    : unit
    =
    let bc = new BlockingCollection<'Item>()
    firstItems |> Array.iter bc.Add
    let processedCountLock = Object()
    let mutable processedCount = 0
    let processItem item =
        let toSchedule = work item
        let processedCount = lock processedCountLock (fun () -> processedCount <- processedCount + 1; processedCount)
        toSchedule |> Array.iter bc.Add
        processedCount
    
    // TODO Could avoid workers with some semaphores
    let workerWork () : unit =
        for node in bc.GetConsumingEnumerable(ct) do
            if not ct.IsCancellationRequested then // improve
                let processedCount = processItem node
                if stop processedCount then
                    bc.CompleteAdding()

    Array.Parallel.map workerWork |> ignore // use cancellation
    ()
     
let processGraph
    (graph : FileGraph)
    (doWork : 'Item -> 'State -> 'Result * 'State)
    (folder : 'State -> 'Result -> 'State)
    (parallelism : int)
    : 'State
    =
    let transitiveDeps = graph |> calcTransitiveGraph
    let dependants = graph |> reverseGraph
    let nodes = graph.Keys |> Seq.map ...
    let leaves = nodes |> Seq.filter ...
    let work
        (node : Node<'Item, 'State, 'Result>)
        : Node<'Item, 'State, 'Result>[]
        =
        let inputState = combineResults node.Deps node.TransitiveDeps folder
        let res = doWork node.Info.Item
        node.Result <- res
        let unblocked =
            node.Info.Dependants
            |> Array.filter (fun x -> 
                let pdc =
                    lock x (fun () ->
                    x.Info.ProcessedDepsCount++
                    x.Info.PrcessedDepsCount
                )
                pdc = node.Info.Deps.Length
            )
         |> Array.map (fun x -> nodes[x])
     unblocked
     
    processInParallel
        leaves
        work
        parallelism
        (fun processedCount -> processedCount = nodes.Length)

    let state = combineResults nodes nodes addCheckResultsToTcState 
    state