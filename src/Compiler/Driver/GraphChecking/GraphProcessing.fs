module internal FSharp.Compiler.GraphChecking.GraphProcessing

open System.Threading
open FSharp.Compiler.GraphChecking
open System.Threading.Tasks
open System
open Internal.Utilities.Library

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

type GraphProcessingException(msg, ex: Exception) =
    inherit exn(msg, ex)

let processGraphAsync<'Item, 'Result when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    (work: ('Item -> ProcessedNode<'Item, 'Result>) -> NodeInfo<'Item> -> Async2<'Result>)
    : Async2<('Item * 'Result)[]> =

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

    let rec queueNode node =
        async2 {
            try
                do! processNode node
            with ex ->
                return raise (GraphProcessingException($"Encountered exception when processing item '{node.Info.Item}'", ex))
        }

    and processNode (node: GraphNode<'Item, 'Result>) =
        async2 {
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

            do! unblockedDependents |> Seq.map queueNode |> Async2.Parallel |> Async2.Ignore
        }

    async2 {
        do! leaves |> Seq.map queueNode |> Async2.Parallel |> Async2.Ignore

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

let processGraph<'Item, 'Result when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    (work: ('Item -> ProcessedNode<'Item, 'Result>) -> NodeInfo<'Item> -> 'Result)
    (parentCt: CancellationToken)
    : ('Item * 'Result)[] =
    let work node info = async2 { return work node info }
    Async2.RunSynchronously(processGraphAsync graph (fun lookup info -> async2 { return! work lookup info }), parentCt)
