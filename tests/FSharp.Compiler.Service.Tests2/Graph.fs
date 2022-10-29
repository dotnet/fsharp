module FSharp.Compiler.Service.Tests.Graph

#nowarn "40"

open System.Collections.Generic
open FSharp.Compiler.Service.Tests.Utils

/// <summary> DAG of files </summary>
type Graph<'Node> = IReadOnlyDictionary<'Node, 'Node[]>

module Graph =
    
    let transitive<'Node when 'Node : equality> (graph : Graph<'Node>) : Graph<'Node> =
        let rec calcTransitiveEdges =
            fun (node : 'Node) ->
                let edgeTargets = graph[node]
                edgeTargets
                |> Array.collect calcTransitiveEdges
                |> Array.append edgeTargets
                |> Array.distinct
            // Dispose of memoisation context
            |> memoize
        
        graph.Keys
        |> Seq.map (fun node -> node, calcTransitiveEdges node)
        |> readOnlyDict
        
    let reverse (originalGraph : Graph<'Node>) : Graph<'Node> =
        originalGraph
        // Collect all edges
        |> Seq.collect (fun (KeyValue(idx, deps)) -> deps |> Array.map (fun dep -> idx, dep))
        // Group dependants of the same dependencies together
        |> Seq.groupBy (fun (idx, dep) -> dep)
        // Construct reversed graph
        |> Seq.map (fun (dep, edges) -> dep, edges |> Seq.map fst |> Seq.toArray)
        |> dict
        // Add nodes that are missing due to having no dependants
        |> fun graph ->
            originalGraph
            |> Seq.map (fun (KeyValue(idx, deps)) ->
                match graph.TryGetValue idx with
                | true, dependants -> idx, dependants
                | false, _ -> idx, [||]
            )
        |> readOnlyDict
    