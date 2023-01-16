/// Parallel processing of graph of work items with dependencies
module internal FSharp.Compiler.GraphChecking.GraphProcessing

open System.Threading

/// Information about the node in a graph, describing its relation with other nodes.
type NodeInfo<'Item> =
    {
        Item: 'Item
        Deps: 'Item[]
        TransitiveDeps: 'Item[]
        Dependants: 'Item[]
    }

type IncrementableInt(value: int) =
    let mutable value = value
    with
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

/// <summary>
/// A generic method to generate results for a graph of work items in parallel.
/// Processes leaves first, and after each node has been processed, schedules any now unblocked dependants.
/// Returns a list of results, per item.
/// Uses ThreadPool to schedule work.
/// </summary>
/// <param name="graph">Graph of work items</param>
/// <param name="work">A function to generate results for a single item</param>
/// <param name="ct">Cancellation token</param>
/// <remarks>
/// An alternative scheduling approach is to schedule N parallel tasks that process items from a BlockingCollection.
/// My basic tests suggested it's faster, although confirming that would require more detailed testing.
/// </remarks>
let processGraph<'Item, 'Result when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    (work: ('Item -> ProcessedNode<'Item, 'Result>) -> NodeInfo<'Item> -> 'Result)
    (ct: CancellationToken)
    : ('Item * 'Result)[] =
    let transitiveDeps = graph |> Graph.transitive
    let dependants = graph |> Graph.reverse

    let makeNode (item: 'Item) : GraphNode<'Item, 'Result> =
        let info =
            let exists = graph.ContainsKey item

            if
                not exists
                || not (transitiveDeps.ContainsKey item)
                || not (dependants.ContainsKey item)
            then
                printfn $"Unexpected inconsistent state of the graph for item '{item}'"

            {
                Item = item
                Deps = graph[item]
                TransitiveDeps = transitiveDeps[item]
                Dependants = dependants[item]
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

    let waitHandle = new ManualResetEventSlim(false)

    let getItemPublicNode item =
        let node = nodes[item]

        {
            ProcessedNode.Info = node.Info
            ProcessedNode.Result =
                node.Result
                |> Option.defaultWith (fun () -> failwith $"Results for item '{node.Info.Item}' are not yet available")
        }

    let processedCount = IncrementableInt(0)
    let mutable exn: ('Item * System.Exception) option = None

    let incrementProcessedNodesCount () =
        if processedCount.Increment() = nodes.Count then
            waitHandle.Set()

    let rec queueNode node =
        Async.Start(
            async {
                let! res = async { processNode node } |> Async.Catch

                match res with
                | Choice1Of2 () -> ()
                | Choice2Of2 ex ->
                    exn <- Some(node.Info.Item, ex)
                    waitHandle.Set()
            },
            ct
        )

    and processNode (node: GraphNode<'Item, 'Result>) : unit =

        let info = node.Info

        let singleRes = work getItemPublicNode info
        node.Result <- Some singleRes

        let unblockedDependants =
            node.Info.Dependants
            |> lookupMany
            // For every dependant, increment its number of processed dependencies,
            // and filter dependants which now have all dependencies processed (but didn't before).
            |> Array.filter (fun dependant ->
                let pdc = dependant.ProcessedDepsCount.Increment()
                // Note: We cannot read 'dependant.ProcessedDepsCount' again to avoid returning the same item multiple times.
                pdc = dependant.Info.Deps.Length)

        unblockedDependants |> Array.iter queueNode
        incrementProcessedNodesCount ()

    leaves |> Array.iter queueNode

    waitHandle.Wait(ct)

    match exn with
    | None -> ()
    | Some (item, ex) -> raise (System.Exception($"Encountered exception when processing item '{item}'", ex))

    nodes.Values
    |> Seq.map (fun node ->
        let result =
            node.Result
            |> Option.defaultWith (fun () -> failwith $"Unexpected lack of result for item '{node.Info.Item}'")

        node.Info.Item, result)
    |> Seq.sortBy fst
    |> Seq.toArray
