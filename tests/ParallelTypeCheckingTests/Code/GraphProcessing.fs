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
    
type StateMeta<'Item> =
    {
        Contributors : 'Item[]
    }
    with static member Empty () = {Contributors = [||]}

type StateWrapper<'Item, 'State> =
    {
        Meta : StateMeta<'Item>
        State : 'State
    }
type ResultWrapper<'Item, 'Result> =
    {
        Item : 'Item
        Result : 'Result
    }

type Node<'Item, 'State, 'Result> =
    {
        Info : NodeInfo<'Item>
        mutable ProcessedDepsCount : int
        mutable Result : ('State * 'Result) option
        mutable InputState : 'State option
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
let combineResultsOld
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
            node.Info.Item 
            // node.Info.TransitiveDeps.Length
        deps
        // TODO To workaround a problem with merging state in the wrong order,
        // we temporarily effectively pick the child with the lowest index.
        // This means children are folded fully in sequence
        |> Array.minBy sizeMetric
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
        |> Array.distinctBy (fun dep -> dep.Info.Item)
        |> Array.map (fun dep ->
            dep.Result
            |> orFail
            |> snd
        )
    let state = Array.fold folder firstState resultsToAdd
    state


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
    let orFail value =
        value
        |> Option.defaultWith (fun () -> failwith "Unexpected lack of result")
    let firstState = emptyState    
    // TODO Potential perf optimisation: Keep transDeps in a HashSet from the start,
    // avoiding reconstructing the HashSet here
    
    // Add single-file results of remaining transitive deps one-by-one using folder
    // Note: Good to preserve order here so that folding happens in file order
    let included =
        let set = HashSet()
        //set.Add biggestDep.Info.Item |> ignore
        set
    let resultsToAdd =
        transitiveDeps
        // Sort it by effectively file index.
        // For some reason this is needed, otherwise gives 'missing namespace' and other errors when using the resulting state.
        // Does this make sense? Should the results be foldable in any order?
        |> Array.sortBy (fun d -> d.Info.Item)
        |> Array.filter (fun dep -> included.Contains dep.Info.Item = false)
        |> Array.distinctBy (fun dep -> dep.Info.Item)
        |> Array.map (fun dep ->
            dep.Result
            |> orFail
            |> snd
        )
    let state = Array.fold folder firstState resultsToAdd
    state


// TODO Could be replaced with a simpler recursive approach with memoised per-item results
let processGraph<'Item, 'State, 'Result, 'FinalFileResult when 'Item : equality and 'Item : comparison>
    (graph : Graph<'Item>)
    (doWork : 'Item -> 'State -> 'Result)
    (folder : 'State -> 'Result -> 'FinalFileResult * 'State)
    (emptyState : 'State)
    (includeInFinalState : 'Item -> bool)
    (parallelism : int)
    : 'FinalFileResult[] * 'State
    =
    let transitiveDeps = graph |> Graph.transitive
    let dependants = graph |> Graph.reverse
    let makeNode (item : 'Item) : Node<'Item, StateWrapper<'Item, 'State>, ResultWrapper<'Item, 'Result>> =
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
            InputState = None
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
    
    let emptyState =
        {
            Meta = StateMeta.Empty<'Item>()
            State = emptyState
        }
    
    let folder {Meta = meta; State = state} {Item = item; Result = result} =
        let finalFileResult, state = folder state result
        let state =
            {
                Meta = {Contributors = Array.append meta.Contributors [|item|]}
                State = state
            }
        finalFileResult, state
    
    let work
        (node : Node<'Item, StateWrapper<'Item, 'State>, ResultWrapper<'Item, 'Result>>)
        : Node<'Item, StateWrapper<'Item, 'State>, ResultWrapper<'Item, 'Result>>[]
        =
        let folder x y = folder x y |> snd
        let deps = lookupMany node.Info.Deps
        let transitiveDeps = lookupMany node.Info.TransitiveDeps
        let inputState = combineResults emptyState deps transitiveDeps folder
        node.InputState <- Some inputState
        let singleRes = doWork node.Info.Item inputState.State
        let singleRes = {Item = node.Info.Item; Result = singleRes}
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
    let finals, {State = state}: 'FinalFileResult[] * StateWrapper<'Item, 'State> =
        nodesArray
        |> Array.sortBy (fun node -> node.Info.Item)
        |> fun nodes ->
            printfn $"%+A{nodes |> Array.map (fun n -> n.Info.Item.ToString())}"
            nodes
        |> Array.fold (fun (fileResults, state) node ->
            let fileResult, newState = folder state (node.Result.Value |> snd)
            let state = if includeInFinalState node.Info.Item then newState else state
            Array.append fileResults [|fileResult|], state
        ) ([||], emptyState)
    
    let x = nodesArray[22]
    let _y = x.Info
    let _z = x.Result |> Option.get
    let _a = x.InputState |> Option.get
    let _b = x.ProcessedDepsCount
    finals, state