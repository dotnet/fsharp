/// Parallel processing of graph of work items with dependencies
module FSharp.Compiler.Service.Tests.GraphProcessing

open System.Collections.Generic
open System.Threading
open FSharp.Compiler.Service.Tests.Graph

/// Used for processing
type NodeInfo<'Item> =
    {
        Item : 'Item
        Deps : 'Item[]
        TransitiveDeps : 'Item[]
        Dependants : 'Item[]
    }
type Node<'Item, 'State, 'Result> =
    {
        Info : NodeInfo<'Item>
        mutable ProcessedDepsCount : int
        mutable Result : ('State * 'Result) option
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
    (emptyState : 'State)
    (deps : Node<'Item, 'State, 'Result>[])
    (transitiveDeps : Node<'Item, 'State, 'Result>[])
    (folder : 'State -> 'Result -> 'State)
    : 'State
    =
    match deps with
    | [||] -> emptyState
    | _ ->
    
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
    let included =
        let set = HashSet(biggestDep.Info.TransitiveDeps)
        set.Add biggestDep.Info.Item |> ignore
        set
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
    
// TODO Could be replaced with a simpler recursive approach with memoised per-item results
let processGraph<'Item, 'State, 'Result when 'Item : equality>
    (graph : Graph<'Item>)
    (doWork : 'Item -> 'State -> 'Result)
    (folder : 'State -> 'Result -> 'State)
    (emptyState : 'State)
    (parallelism : int)
    : 'State
    =
    let transitiveDeps = graph |> Graph.transitive
    let dependants = graph |> Graph.reverse
    let makeNode (item : 'Item) : Node<'Item,'State,'Result> =
        let info =
            {
                Item = item
                Deps = graph[item]
                TransitiveDeps = transitiveDeps[item]
                Dependants = dependants[item]
            }
        {
            Info = info
            Result = None
            ProcessedDepsCount = 0
        }
        
    let nodes =
        graph.Keys
        |> Seq.map (fun item -> item, makeNode item)
        |> readOnlyDict
    let lookup item = nodes[item]
    let lookupMany items = items |> Array.map lookup
        
    let leaves =
        nodes.Values
        |> Seq.filter (fun n -> n.Info.Deps.Length = 0)
        |> Seq.toArray
    
    let work
        (node : Node<'Item, 'State, 'Result>)
        : Node<'Item, 'State, 'Result>[]
        =
        let deps = lookupMany node.Info.Deps
        let transitiveDeps = lookupMany node.Info.TransitiveDeps
        let inputState = combineResults emptyState deps transitiveDeps folder
        let singleRes = doWork node.Info.Item inputState
        let state = folder inputState singleRes
        //let state,  = folder inputState singleRes
        node.Result <- Some (state, singleRes)
        
        // Need to double-check that only one dependency schedules this dependant
        let unblocked =
            node.Info.Dependants
            |> lookupMany
            |> Array.filter (fun x -> 
                let pdc =
                    // TODO Not ideal, better ways most likely exist
                    lock x (fun () ->
                        x.ProcessedDepsCount <- x.ProcessedDepsCount + 1
                        x.ProcessedDepsCount
                    )
                pdc = x.Info.Deps.Length
            )
        unblocked
    
    
    use cts = new CancellationTokenSource()
    
    Parallel.processInParallel
        leaves
        work
        parallelism
        (fun processedCount -> processedCount = nodes.Count)
        cts.Token

    let nodesArray = nodes.Values |> Seq.toArray
    let state = combineResults emptyState nodesArray nodesArray folder 
    state