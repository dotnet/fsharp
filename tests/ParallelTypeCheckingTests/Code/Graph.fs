namespace ParallelTypeCheckingTests
#nowarn "1182"
#nowarn "40"

open System.Collections.Generic
open ParallelTypeCheckingTests.Utils

/// <summary> DAG of files </summary>
type Graph<'Node> = IReadOnlyDictionary<'Node, 'Node[]>

module Graph =
    
    let collectEdges<'Node when 'Node : equality> (graph : Graph<'Node>) : ('Node * 'Node)[] =
        let graph : IReadOnlyDictionary<'Node, 'Node[]> = graph
        graph
        |> Seq.collect (fun (KeyValue(node, deps)) -> deps |> Array.map (fun dep -> node, dep))
        |> Seq.toArray
    
    /// Create entries for nodes that don't have any dependencies but are mentioned as dependencies themselves
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
    
    /// Create a transitive closure of the graph
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
        
    /// Create a reverse of the graph
    let reverse (originalGraph : Graph<'Node>) : Graph<'Node> =
        originalGraph
        // Collect all edges
        |> Seq.collect (fun (KeyValue(idx, deps)) -> deps |> Array.map (fun dep -> idx, dep))
        // Group dependants of the same dependencies together
        |> Seq.groupBy (fun (_idx, dep) -> dep)
        // Construct reversed graph
        |> Seq.map (fun (dep, edges) -> dep, edges |> Seq.map fst |> Seq.toArray)
        |> readOnlyDict
        |> fillEmptyNodes
    
    let printCustom (graph : Graph<'Node>) (printer : 'Node -> string) : unit =
        printfn "Graph:"
        let join (xs : string[]) =
            System.String.Join(", ", xs)
        graph
        |> Seq.iter (fun (KeyValue(file, deps)) -> printfn $"{file} -> {deps |> Array.map printer |> join}")
    
    let print (graph : Graph<'Node>) : unit = printCustom graph (fun node -> node.ToString())
    