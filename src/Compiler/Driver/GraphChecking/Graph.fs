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

    let transitive<'Node when 'Node: equality> (graph: Graph<'Node>) : Graph<'Node> =
        /// Find transitive dependencies of a single node.
        let transitiveDeps (node: 'Node) =
            let visited = HashSet<'Node>()

            let rec dfs (node: 'Node) =
                graph[node]
                // Add direct dependencies.
                // Use HashSet.Add return value semantics to filter out those that were added previously.
                |> Array.filter visited.Add
                |> Array.iter dfs

            dfs node
            visited |> Seq.toArray

        graph.Keys
        |> Seq.toArray
        |> Array.Parallel.map (fun node -> node, transitiveDeps node)
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
