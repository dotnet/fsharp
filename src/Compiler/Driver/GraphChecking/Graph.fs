namespace FSharp.Compiler.GraphChecking

#nowarn "1182"
#nowarn "40"

open System.Collections.Concurrent
open System.Collections.Generic
open System.Linq

/// <summary> Directed Acyclic Graph (DAG) of arbitrary nodes </summary>
type Graph<'Node> = IReadOnlyDictionary<'Node, 'Node[]>

module Graph =
    let memoize<'a, 'b when 'a: equality> f : ('a -> 'b) =
        let y = HashIdentity.Structural<'a>
        let d = new ConcurrentDictionary<'a, 'b>(y)
        fun x -> d.GetOrAdd(x, (fun r -> f r))

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
            let x: KeyValuePair<'Node, 'Node[]>[] = Array.append (graph |> Seq.toArray) toAdd
            x.ToDictionary((fun (KeyValue (x, _)) -> x), (fun (KeyValue (_, v)) -> v)) :> IReadOnlyDictionary<_, _>

    /// Create entries for nodes that don't have any dependencies but are mentioned as dependencies themselves
    let fillEmptyNodes<'Node when 'Node: equality> (graph: Graph<'Node>) : Graph<'Node> =
        let missingNodes =
            graph.Values |> Seq.toArray |> Array.concat |> Array.except graph.Keys

        addIfMissing missingNodes graph

    /// Create a transitive closure of the graph
    let transitive<'Node when 'Node: equality> (graph: Graph<'Node>) : Graph<'Node> =
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

    /// Create a reverse of the graph
    let reverse (originalGraph: Graph<'Node>) : Graph<'Node> =
        originalGraph
        // Collect all edges
        |> Seq.collect (fun (KeyValue (idx, deps)) -> deps |> Array.map (fun dep -> idx, dep))
        // Group dependants of the same dependencies together
        |> Seq.groupBy snd
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

    let serialiseToJson (path: string) (graph: Graph<string>) : unit =
        let escapeName (name: string) =
            name.Replace("\\", "\\\\") |> sprintf "\"%s\""

        let entries =
            graph
            |> Seq.map (fun (KeyValue (file, deps)) ->
                let deps = deps |> Seq.map escapeName |> String.concat "," |> sprintf "[ %s ]"

                $"    {escapeName file}: {deps}")
            |> String.concat ","

        let json = $"{{\n{entries}\n}}"
        System.IO.File.WriteAllText(path, json)
