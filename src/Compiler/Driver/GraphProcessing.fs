/// Parallel processing of graph of work items with dependencies
module FSharp.Compiler.Service.Driver.GraphProcessing

open System.Collections.Generic
open System.Threading
open FSharp.Compiler.Service.Driver.Graph

/// Used for processing
type NodeInfo<'Item> =
    {
        Item: 'Item
        Deps: 'Item[]
        TransitiveDeps: 'Item[]
        Dependants: 'Item[]
    }

type StateMeta<'Item> =
    {
        Contributors: 'Item[]
    }

    static member Empty() = { Contributors = [||] }

type StateWrapper<'Item, 'State> =
    {
        Meta: StateMeta<'Item>
        State: 'State
    }

type ResultWrapper<'Item, 'Result> = { Item: 'Item; Result: 'Result }

type Node<'Item, 'State, 'Result> =
    {
        Info: NodeInfo<'Item>
        mutable ProcessedDepsCount: int
        mutable Result: ('State * 'Result) option
        mutable InputState: 'State option
    }

// TODO Do we need to suppress some error logging if we
// TODO apply the same partial results multiple times?
// TODO Maybe we can enable logging only for the final fold
/// <summary>
/// Combine results of dependencies needed to type-check a 'higher' node in the graph
/// </summary>
let combineResultsOld
    (emptyState: 'State)
    (deps: Node<'Item, 'State, 'Result>[])
    (transitiveDeps: Node<'Item, 'State, 'Result>[])
    (folder: 'State -> 'Result -> 'State)
    : 'State =
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
            value |> Option.defaultWith (fun () -> failwith "Unexpected lack of result")

        let firstState = biggestDep.Result |> orFail |> fst

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
            |> Array.map (fun dep -> dep.Result |> orFail |> snd)

        let state = Array.fold folder firstState resultsToAdd
        state

// TODO Do we need to suppress some error logging if we
// TODO apply the same partial results multiple times?
// TODO Maybe we can enable logging only for the final fold
/// <summary>
/// Combine results of dependencies needed to type-check a 'higher' node in the graph
/// </summary>
let combineResults
    (emptyState: 'State)
    (deps: Node<'Item, 'State, 'Result>[])
    (transitiveDeps: Node<'Item, 'State, 'Result>[])
    (folder: 'State -> 'Result -> 'State)
    (_foldingOrderer: 'Item -> int)
    : 'State =
    match deps with
    | [||] -> emptyState
    | _ ->
        let orFail value =
            value |> Option.defaultWith (fun () -> failwith "Unexpected lack of result")

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
            // TODO Use _foldingOrderer
            |> Array.sortBy (fun node -> node.Info.Item)
            |> Array.filter (fun dep -> included.Contains dep.Info.Item = false)
            |> Array.distinctBy (fun dep -> dep.Info.Item)
            |> Array.map (fun dep -> dep.Result |> orFail |> snd)

        let state = Array.fold folder firstState resultsToAdd
        state

// TODO Could be replaced with a simpler recursive approach with memoised per-item results
let processGraph<'Item, 'State, 'Result, 'FinalFileResult when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    (doWork: 'Item -> 'State -> 'Result)
    (folder: 'State -> 'Result -> 'FinalFileResult * 'State)
    (foldingOrderer: 'Item -> int)
    (emptyState: 'State)
    (includeInFinalState: 'Item -> bool)
    (parallelism: int)
    : 'FinalFileResult[] * 'State =
    let transitiveDeps = graph |> Graph.transitiveOpt
    let dependants = graph |> Graph.reverse

    let makeNode (item: 'Item) : Node<'Item, StateWrapper<'Item, 'State>, ResultWrapper<'Item, 'Result>> =
        let info =
            let exists = graph.ContainsKey item

            if
                not exists
                || not (transitiveDeps.ContainsKey item)
                || not (dependants.ContainsKey item)
            then
                printfn $"WHAT {item}"

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

    let nodes = graph.Keys |> Seq.map (fun item -> item, makeNode item) |> readOnlyDict
    let lookup item = nodes[item]
    let lookupMany items = items |> Array.map lookup

    let leaves =
        nodes.Values |> Seq.filter (fun n -> n.Info.Deps.Length = 0) |> Seq.toArray

    let emptyState =
        {
            Meta = StateMeta.Empty<'Item>()
            State = emptyState
        }

    let folder { Meta = meta; State = state } { Item = item; Result = result } =
        let finalFileResult, state = folder state result

        let state =
            {
                Meta =
                    {
                        Contributors = Array.append meta.Contributors [| item |]
                    }
                State = state
            }

        finalFileResult, state

    printfn $"Node count: {nodes.Count}"
    // let mutable cnt = 1

    let work
        (node: Node<'Item, StateWrapper<'Item, 'State>, ResultWrapper<'Item, 'Result>>)
        : Node<'Item, StateWrapper<'Item, 'State>, ResultWrapper<'Item, 'Result>>[] =
        let folder x y = folder x y |> snd
        let deps = lookupMany node.Info.Deps
        let transitiveDeps = lookupMany node.Info.TransitiveDeps
        let inputState = combineResults emptyState deps transitiveDeps folder foldingOrderer
        node.InputState <- Some inputState
        let singleRes = doWork node.Info.Item inputState.State

        let singleRes =
            {
                Item = node.Info.Item
                Result = singleRes
            }

        let state = folder inputState singleRes
        //let state,  = folder inputState singleRes
        node.Result <- Some(state, singleRes)
        // Need to double-check that only one dependency schedules this dependant
        let unblocked =
            node.Info.Dependants
            |> lookupMany
            |> Array.filter (fun x ->
                let pdc =
                    // TODO Not ideal, better ways most likely exist
                    lock x (fun () ->
                        x.ProcessedDepsCount <- x.ProcessedDepsCount + 1
                        x.ProcessedDepsCount)

                pdc = x.Info.Deps.Length)
        // printfn $"State after {node.Info.Item}"
        // nodes
        // |> Seq.map (fun (KeyValue(_, v)) ->
        //     let x = v.Info.Deps.Length - v.ProcessedDepsCount
        //     $"{v.Info.Item} - {x} deps left"
        // )
        // |> Seq.iter (fun x -> printfn $"{x}")
        // let c = cnt
        // cnt <- cnt+1
        // printfn $"Finished processing node. {unblocked.Length} nodes unblocked"
        unblocked

    use cts = new CancellationTokenSource()

    Parallel.processInParallel
        leaves
        work
        parallelism
        (fun processedCount -> processedCount = nodes.Count)
        cts.Token
        (fun x -> x.Info.Item.ToString())

    let nodesArray = nodes.Values |> Seq.toArray

    let finals, { State = state }: 'FinalFileResult[] * StateWrapper<'Item, 'State> =
        nodesArray
        |> Array.filter (fun node -> includeInFinalState node.Info.Item)
        |> Array.sortBy (fun node -> node.Info.Item)
        |> Array.fold
            (fun (fileResults, state) node ->
                let fileResult, state = folder state (node.Result.Value |> snd)
                Array.append fileResults [| fileResult |], state)
            ([||], emptyState)

    finals, state

type Node2<'Item, 'Result> =
    {
        Info: NodeInfo<'Item>
        mutable ProcessedDepsCount: int
        mutable Result: 'Result option
    }

// TODO Could be replaced with a simpler recursive approach with memoised per-item results
let processGraphSimple<'Item, 'Result when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    // Accepts item and a list of item results. Handles combining results.
    (doWork: 'Item -> ResultWrapper<'Item, 'Result>[] -> 'Result)
    (parallelism: int)
    : ResultWrapper<'Item, 'Result>[] =
    let transitiveDeps = graph |> Graph.transitiveOpt
    let dependants = graph |> Graph.reverse

    let makeNode (item: 'Item) : Node2<'Item, ResultWrapper<'Item, 'Result>> =
        let info =
            let exists = graph.ContainsKey item
            if
                not exists
                || not (transitiveDeps.ContainsKey item)
                || not (dependants.ContainsKey item)
            then
                failwith $"WHAT {item}"

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

    let nodes = graph.Keys |> Seq.map (fun item -> item, makeNode item) |> readOnlyDict
    let lookup item = nodes[item]
    let lookupMany items = items |> Array.map lookup

    let leaves =
        nodes.Values
        |> Seq.filter (fun n -> n.Info.Deps.Length = 0)
        |> Seq.toArray

    printfn $"Node count: {nodes.Count}"

    let work
        (node: Node2<'Item, ResultWrapper<'Item, 'Result>>)
        : Node2<'Item, ResultWrapper<'Item, 'Result>>[] =
        let _deps = lookupMany node.Info.Deps
        let transitiveDeps = lookupMany node.Info.TransitiveDeps
        let inputs =
            transitiveDeps
            |> Array.map (fun n -> n.Result |> Option.get)
        let singleRes = doWork node.Info.Item inputs
        let singleRes =
            {
                Item = node.Info.Item
                Result = singleRes
            }
        node.Result <- Some singleRes
        // Need to double-check that only one dependency schedules this dependant
        let unblocked =
            node.Info.Dependants
            |> lookupMany
            |> Array.filter (fun x ->
                let pdc =
                    // TODO Not ideal, better ways most likely exist
                    lock x (fun () ->
                        x.ProcessedDepsCount <- x.ProcessedDepsCount + 1
                        x.ProcessedDepsCount)
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
        (fun x -> x.Info.Item.ToString())

    let nodesArray = nodes.Values |> Seq.toArray

    nodesArray
    |> Array.map (fun n -> n.Result.Value)


/// Used for processing
type NodeInfo3<'Item> =
    {
        Item: 'Item
        Deps: 'Item[]
        Dependants: 'Item[]
    }
    
type Node3<'Item> =
    {
        Info: NodeInfo3<'Item>
        mutable ProcessedDepsCount: int
    }

/// Graph processing that doesn't handle results but just invokes the worker when dependencies are ready
let processGraphSimpler<'Item when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    // Accepts item and a list of item results. Handles combining results.
    (doWork: 'Item -> unit)
    (parallelism: int)
    : unit
    =
    let dependants = graph |> Graph.reverse

    let makeNode (item: 'Item) : Node3<'Item> =
        let info =
            let exists = graph.ContainsKey item
            if
                not exists
                || not (dependants.ContainsKey item)
            then
                failwith $"WHAT {item}"
            {
                Item = item
                Deps = graph[item]
                Dependants = dependants[item]
            }

        {
            Info = info
            ProcessedDepsCount = 0
        }

    let nodes = graph.Keys |> Seq.map (fun item -> item, makeNode item) |> readOnlyDict
    let lookup item = nodes[item]
    let lookupMany items = items |> Array.map lookup

    let leaves =
        nodes.Values
        |> Seq.filter (fun n -> n.Info.Deps.Length = 0)
        |> Seq.toArray

    // printfn $"Node count: {nodes.Count}"

    let work
        (node: Node3<'Item>)
        : Node3<'Item>[]
        =
        let _deps = lookupMany node.Info.Deps
        // printfn $"{node.Info.Item} DoWork"
        doWork node.Info.Item
        // printfn $"{node.Info.Item} DoneWork"
        // Need to double-check that only one dependency schedules this dependant
        let unblocked =
            node.Info.Dependants
            |> lookupMany
            |> Array.filter (fun x ->
                let pdc =
                    // TODO Not ideal, better ways most likely exist
                    lock x (fun () ->
                        x.ProcessedDepsCount <- x.ProcessedDepsCount + 1
                        x.ProcessedDepsCount)
                pdc = x.Info.Deps.Length
            )
        // printfn $"{node.Info.Item} unblocked gathered"
        unblocked

    use cts = new CancellationTokenSource()

    Parallel.processInParallel
        leaves
        work
        parallelism
        (fun processedCount -> processedCount = nodes.Count)
        cts.Token
        (fun x -> x.Info.Item.ToString())
