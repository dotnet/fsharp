﻿module internal FSharp.Compiler.GraphChecking.GraphProcessing

open System.Threading
open FSharp.Compiler.GraphChecking

/// Information about the node in a graph, describing its relation with other nodes.
type NodeInfo<'Item> =
    {
        Item: 'Item
        Deps: 'Item[]
        TransitiveDeps: 'Item[]
        Dependents: 'Item[]
    }

type IncrementableInt(value: int) =
    let mutable value = value
    member this.Value = value
    // Increment the value in a thread-safe manner and return the new value.
    member this.Increment() = Interlocked.Increment(&value)

type GraphNode<'Item, 'Result> =
    {
        Info: NodeInfo<'Item>
        /// Used to determine when all dependencies of this node have been resolved.
        ProcessedDepsCount: IncrementableInt
        mutable Result: 'Result option
    }

/// An already processed node in the graph, with its result available
type ProcessedNode<'Item, 'Result> =
    {
        Info: NodeInfo<'Item>
        Result: 'Result
    }

type GraphProcessingException(msg, ex: System.Exception) =
    inherit exn(msg, ex)

let processGraph<'Item, 'Result when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    (work: ('Item -> ProcessedNode<'Item, 'Result>) -> NodeInfo<'Item> -> 'Result)
    (parentCt: CancellationToken)
    : ('Item * 'Result)[] =
    let transitiveDeps = graph |> Graph.transitive
    let dependents = graph |> Graph.reverse
    // Cancellation source used to signal either an exception in one of the items or end of processing.
    use localCts = new CancellationTokenSource()
    use cts = CancellationTokenSource.CreateLinkedTokenSource(parentCt, localCts.Token)

    let makeNode (item: 'Item) : GraphNode<'Item, 'Result> =
        let info =
            let exists = graph.ContainsKey item

            if
                not exists
                || not (transitiveDeps.ContainsKey item)
                || not (dependents.ContainsKey item)
            then
                printfn $"Unexpected inconsistent state of the graph for item '{item}'"

            {
                Item = item
                Deps = graph[item]
                TransitiveDeps = transitiveDeps[item]
                Dependents = dependents[item]
            }

        {
            Info = info
            Result = None
            ProcessedDepsCount = IncrementableInt(0)
        }

    let nodes = graph.Keys |> Seq.map (fun item -> item, makeNode item) |> readOnlyDict

    let lookupMany items =
        items |> Array.map (fun item -> nodes[item])

    let leaves =
        nodes.Values |> Seq.filter (fun n -> n.Info.Deps.Length = 0) |> Seq.toArray

    let getItemPublicNode item =
        let node = nodes[item]

        {
            ProcessedNode.Info = node.Info
            ProcessedNode.Result =
                node.Result
                |> Option.defaultWith (fun () -> failwith $"Results for item '{node.Info.Item}' are not yet available")
        }

    let processedCount = IncrementableInt(0)

    /// Create a setter and getter for an exception raised in one of the work items.
    /// Only the first exception encountered is stored - this can cause non-deterministic errors if more than one item fails.
    let raiseExn, getExn =
        let mutable exn: ('Item * System.Exception) option = None
        let lockObj = obj ()
        // Only set the exception if it hasn't been set already
        let setExn newExn =
            lock lockObj (fun () ->
                match exn with
                | Some _ -> ()
                | None -> exn <- newExn

                localCts.Cancel())

        let getExn () = exn
        setExn, getExn

    let incrementProcessedNodesCount () =
        if processedCount.Increment() = nodes.Count then
            localCts.Cancel()

    let rec queueNode node =
        Async.Start(
            async {
                let! res = async { processNode node } |> Async.Catch

                match res with
                | Choice1Of2() -> ()
                | Choice2Of2 ex -> raiseExn (Some(node.Info.Item, ex))
            },
            cts.Token
        )

    and processNode (node: GraphNode<'Item, 'Result>) : unit =

        let info = node.Info

        let singleRes = work getItemPublicNode info
        node.Result <- Some singleRes

        let unblockedDependents =
            node.Info.Dependents
            |> lookupMany
            // For every dependent, increment its number of processed dependencies,
            // and filter dependents which now have all dependencies processed (but didn't before).
            |> Array.filter (fun dependent ->
                let pdc = dependent.ProcessedDepsCount.Increment()
                // Note: We cannot read 'dependent.ProcessedDepsCount' again to avoid returning the same item multiple times.
                pdc = dependent.Info.Deps.Length)

        unblockedDependents |> Array.iter queueNode
        incrementProcessedNodesCount ()

    leaves |> Array.iter queueNode

    // Wait for end of processing, an exception, or an external cancellation request.
    cts.Token.WaitHandle.WaitOne() |> ignore
    // If we stopped early due to external cancellation, throw.
    parentCt.ThrowIfCancellationRequested()

    // If we stopped early due to an exception, reraise it.
    match getExn () with
    | None -> ()
    | Some(item, ex) -> raise (GraphProcessingException($"Encountered exception when processing item '{item}'", ex))

    // All calculations succeeded - extract the results and sort in input order.
    nodes.Values
    |> Seq.map (fun node ->
        let result =
            node.Result
            |> Option.defaultWith (fun () -> failwith $"Unexpected lack of result for item '{node.Info.Item}'")

        node.Info.Item, result)
    |> Seq.sortBy fst
    |> Seq.toArray

let processGraphAsync<'Item, 'Result when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    (work: ('Item -> ProcessedNode<'Item, 'Result>) -> NodeInfo<'Item> -> Async<'Result>)
    : Async<('Item * 'Result)[]> =
    async {
        let transitiveDeps = graph |> Graph.transitive
        let dependents = graph |> Graph.reverse

        let makeNode (item: 'Item) : GraphNode<'Item, 'Result> =
            let info =
                let exists = graph.ContainsKey item

                if
                    not exists
                    || not (transitiveDeps.ContainsKey item)
                    || not (dependents.ContainsKey item)
                then
                    printfn $"Unexpected inconsistent state of the graph for item '{item}'"

                {
                    Item = item
                    Deps = graph[item]
                    TransitiveDeps = transitiveDeps[item]
                    Dependents = dependents[item]
                }

            {
                Info = info
                Result = None
                ProcessedDepsCount = IncrementableInt(0)
            }

        let nodes = graph.Keys |> Seq.map (fun item -> item, makeNode item) |> readOnlyDict

        let lookupMany items =
            items |> Array.map (fun item -> nodes[item])

        let leaves =
            nodes.Values |> Seq.filter (fun n -> n.Info.Deps.Length = 0) |> Seq.toArray

        let getItemPublicNode item =
            let node = nodes[item]

            {
                ProcessedNode.Info = node.Info
                ProcessedNode.Result =
                    node.Result
                    |> Option.defaultWith (fun () -> failwith $"Results for item '{node.Info.Item}' are not yet available")
            }

        let rec processNode (node: GraphNode<'Item, 'Result>) =
            async {
                let info = node.Info

                let! singleRes = work getItemPublicNode info
                node.Result <- Some singleRes

                let unblockedDependents =
                    node.Info.Dependents
                    |> lookupMany
                    // For every dependent, increment its number of processed dependencies,
                    // and filter dependents which now have all dependencies processed (but didn't before).
                    |> Array.filter (fun dependent ->
                        let pdc = dependent.ProcessedDepsCount.Increment()
                        // Note: We cannot read 'dependent.ProcessedDepsCount' again to avoid returning the same item multiple times.
                        pdc = dependent.Info.Deps.Length)

                do! unblockedDependents |> Array.map processNode |> Async.Parallel |> Async.Ignore
            }

        do! leaves |> Array.map processNode |> Async.Parallel |> Async.Ignore

        // All calculations succeeded - extract the results and sort in input order.
        return
            nodes.Values
            |> Seq.map (fun node ->
                let result =
                    node.Result
                    |> Option.defaultWith (fun () -> failwith $"Unexpected lack of result for item '{node.Info.Item}'")

                node.Info.Item, result)
            |> Seq.sortBy fst
            |> Seq.toArray
    }
