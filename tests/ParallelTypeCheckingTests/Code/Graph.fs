module FSharp.Compiler.Service.Tests.Graph
#nowarn "1182"
#nowarn "40"

open System.Collections.Generic
open FSharp.Compiler.Service.Tests.Utils

/// <summary> DAG of files </summary>
type Graph<'Node> = IReadOnlyDictionary<'Node, 'Node[]>

module Graph =
    
    let fillEmptyNodes<'Node when 'Node : equality> (graph : Graph<'Node>) : Graph<'Node> =
        let missingNodes =
            graph.Values
            |> Seq.toArray
            |> Array.concat
            |> Array.except graph.Keys
        
        let toAdd =
            missingNodes
            |> Array.map (fun n -> KeyValuePair(n, [||]))
        
        let x = Array.append (graph |> Seq.toArray) toAdd
        x
        |> Dictionary<_,_> |> fun x -> x :> IReadOnlyDictionary<_,_>
    
    let transitive<'Node when 'Node : equality> (graph : Graph<'Node>) : Graph<'Node> =
        let rec calcTransitiveEdges =
            fun (node : 'Node) ->
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
        
    let print (graph : Graph<'Node>) : unit =
        printfn "Graph:"
        let join (xs : string[]) =
            System.String.Join(", ", xs)
        graph
        |> Seq.iter (fun (KeyValue(file, deps)) -> printfn $"{file} -> {deps |> Array.map (fun d -> d.ToString()) |> join}")
    