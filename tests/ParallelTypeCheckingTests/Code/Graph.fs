namespace ParallelTypeCheckingTests

#nowarn "1182"
#nowarn "40"

open System.Collections.Generic
open System.IO
open Newtonsoft.Json
open ParallelTypeCheckingTests.Utils

/// <summary> DAG of files </summary>
type Graph<'Node> = IReadOnlyDictionary<'Node, 'Node[]>

module Graph =

    let make (nodeDeps: ('Node * 'Node[]) seq) = nodeDeps |> readOnlyDict

    let map (f: 'a -> 'b) (graph: Graph<'a>) : Graph<'b> =
        graph
        |> Seq.map (fun (KeyValue (node, deps)) -> f node, deps |> Array.map f)
        |> make

    let collectEdges<'Node when 'Node: equality> (graph: Graph<'Node>) : ('Node * 'Node)[] =
        let graph: IReadOnlyDictionary<'Node, 'Node[]> = graph

        graph
        |> Seq.collect (fun (KeyValue (node, deps)) -> deps |> Array.map (fun dep -> node, dep))
        |> Seq.toArray

    let addIfMissing<'Node when 'Node: equality> (nodes: 'Node seq) (graph: Graph<'Node>) : Graph<'Node> =
        nodes
        |> Seq.except (graph.Keys |> Seq.toArray)
        |> fun missing ->
            let toAdd = missing |> Seq.map (fun n -> KeyValuePair(n, [||])) |> Seq.toArray

            let x = Array.append (graph |> Seq.toArray) toAdd
            x |> Dictionary<_, _> |> (fun x -> x :> IReadOnlyDictionary<_, _>)

    /// Create entries for nodes that don't have any dependencies but are mentioned as dependencies themselves
    let fillEmptyNodes<'Node when 'Node: equality> (graph: Graph<'Node>) : Graph<'Node> =
        let missingNodes =
            graph.Values |> Seq.toArray |> Array.concat |> Array.except graph.Keys

        addIfMissing missingNodes graph

    /// Create a transitive closure of the graph
    let transitiveOpt<'Node when 'Node: equality> (graph: Graph<'Node>) : Graph<'Node> =
        let go (node: 'Node) =
            let visited = HashSet<'Node>()

            let rec dfs (node: 'Node) =
                graph[node] |> Array.filter visited.Add |> Array.iter dfs

            dfs node
            visited |> Seq.toArray

        graph.Keys
        |> Seq.toArray
        |> Array.Parallel.map (fun node -> node, go node)
        |> readOnlyDict

    /// Create a transitive closure of the graph
    let transitive<'Node when 'Node: equality> (graph: Graph<'Node>) : Graph<'Node> =
        let rec calcTransitiveEdges =
            fun (node: 'Node) ->
                let edgeTargets =
                    match graph.TryGetValue node with
                    | true, x -> x
                    | false, _ -> failwith "FOO"

                edgeTargets
                |> Array.collect calcTransitiveEdges
                |> Array.append edgeTargets
                |> Array.distinct
            // Dispose of memoisation context
            |> memoize

        graph.Keys
        |> Seq.map (fun node -> node, calcTransitiveEdges node)
        |> readOnlyDict

    /// Create a reverse of the graph
    let reverse (originalGraph: Graph<'Node>) : Graph<'Node> =
        originalGraph
        // Collect all edges
        |> Seq.collect (fun (KeyValue (idx, deps)) -> deps |> Array.map (fun dep -> idx, dep))
        // Group dependants of the same dependencies together
        |> Seq.groupBy (fun (_idx, dep) -> dep)
        // Construct reversed graph
        |> Seq.map (fun (dep, edges) -> dep, edges |> Seq.map fst |> Seq.toArray)
        |> readOnlyDict
        |> addIfMissing originalGraph.Keys

    let printCustom (graph: Graph<'Node>) (printer: 'Node -> string) : unit =
        printfn "Graph:"
        let join (xs: string[]) = System.String.Join(", ", xs)

        graph
        |> Seq.iter (fun (KeyValue (file, deps)) -> printfn $"{file} -> {deps |> Array.map printer |> join}")

    let print (graph: Graph<'Node>) : unit =
        printCustom graph (fun node -> node.ToString())

    let serialiseToJson (path: string) (graph: Graph<'Node>) : unit =
        let json = JsonConvert.SerializeObject(graph, Formatting.Indented)
        printfn $"Serialising graph as JSON in {path}"
        File.WriteAllText(path, json)
