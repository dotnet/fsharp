namespace FSharp.Compiler.GraphChecking

open System.Collections.Generic
open System.Text
open FSharp.Compiler.IO

/// <summary> Directed Acyclic Graph (DAG) of arbitrary nodes </summary>
type internal Graph<'Node> = IReadOnlyDictionary<'Node, 'Node array>

module internal Graph =
    let make (nodeDeps: ('Node * 'Node array) seq) = nodeDeps |> readOnlyDict

    let map (f: 'T -> 'U) (graph: Graph<'T>) : Graph<'U> =
        graph
        |> Seq.map (fun (KeyValue (node, deps)) -> f node, deps |> Array.map f)
        |> make

    let addIfMissing<'Node when 'Node: equality> (nodes: 'Node seq) (graph: Graph<'Node>) : Graph<'Node> =
        let missingNodes = nodes |> Seq.except graph.Keys |> Seq.toArray

        let entriesToAdd =
            missingNodes |> Seq.map (fun n -> KeyValuePair(n, [||])) |> Seq.toArray

        graph
        |> Seq.toArray
        |> Array.append entriesToAdd
        |> Array.map (fun (KeyValue (k, v)) -> k, v)
        |> readOnlyDict

    /// Find transitive dependencies of a single node.
    let transitiveDeps (node: 'Node) (graph: Graph<'Node>) =
        let visited = HashSet<'Node>()

        let rec dfs (node: 'Node) =
            graph[node]
            // Add direct dependencies.
            // Use HashSet.Add return value semantics to filter out those that were added previously.
            |> Array.filter visited.Add
            |> Array.iter dfs

        dfs node
        visited |> Seq.toArray

    let transitive<'Node when 'Node: equality> (graph: Graph<'Node>) : Graph<'Node> =
        graph.Keys
        |> Seq.toArray
        |> Array.Parallel.map (fun node -> node, graph |> transitiveDeps node)
        |> readOnlyDict

    /// Get subgraph of the given graph that contains only nodes that are reachable from the given node.
    let subGraphFor node graph =
        let allDeps = graph |> transitiveDeps node
        let relevant n = n = node || allDeps |> Array.contains n

        graph
        |> Seq.choose (fun (KeyValue (src, deps)) ->
            if relevant src then
                Some(src, deps |> Array.filter relevant)
            else
                None)
        |> make

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

    /// Returns leaves of the graph and the remaining graph without the leaves.
    let cutLeaves (graph: Graph<'Node>) =
        let notLeaves =
            set
                [
                    for (KeyValue (node, deps)) in graph do
                        if deps.Length > 0 then
                            node
                ]

        let leaves =
            set
                [
                    for (KeyValue (node, deps)) in graph do
                        if deps.Length = 0 then
                            node

                        yield! deps |> Array.filter (notLeaves.Contains >> not)
                ]

        leaves,
        seq {
            for (KeyValue (node, deps)) in graph do
                if deps.Length > 0 then
                    node, deps |> Array.filter (leaves.Contains >> not)
        }
        |> make

    /// Returns layers of leaves repeatedly removed from the graph until there's nothing left
    let leafSequence (graph: Graph<'Node>) =
        let rec loop (graph: Graph<'Node>) acc =
            match graph |> cutLeaves with
            | leaves, _ when leaves.IsEmpty -> acc
            | leaves, graph ->
                seq {
                    yield! acc
                    leaves
                }
                |> loop graph

        loop graph Seq.empty

    let printCustom (graph: Graph<'Node>) (nodePrinter: 'Node -> string) : unit =
        printfn "Graph:"
        let join (xs: string[]) = System.String.Join(", ", xs)

        graph
        |> Seq.iter (fun (KeyValue (file, deps)) -> printfn $"{file} -> {deps |> Array.map nodePrinter |> join}")

    let print (graph: Graph<'Node>) : unit =
        printCustom graph (fun node -> node.ToString())

    let serialiseToMermaid path (graph: Graph<FileIndex * string>) =
        let sb = StringBuilder()
        let appendLine (line: string) = sb.AppendLine(line) |> ignore

        appendLine "```mermaid"
        appendLine "flowchart RL"

        for KeyValue ((idx, fileName), _) in graph do
            appendLine $"    %i{idx}[\"%s{fileName}\"]"

        for KeyValue ((idx, _), deps) in graph do
            for depIdx, _depFileName in deps do
                appendLine $"    %i{idx} --> %i{depIdx}"

        appendLine "```"

        use out =
            FileSystem.OpenFileForWriteShim(path, fileMode = System.IO.FileMode.Create)

        out.WriteAllText(sb.ToString())
