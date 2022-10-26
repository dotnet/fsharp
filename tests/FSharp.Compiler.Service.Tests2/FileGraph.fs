module FSharp.Compiler.Service.Tests.Graph

#nowarn "40"

open System.Collections.Concurrent
open System.Collections.Generic

type FileIdx =
    FileIdx of int
    with
        member this.Idx = match this with FileIdx idx -> idx
        override this.ToString() = this.Idx.ToString()
        static member make (idx : int) = FileIdx idx 

/// <summary> DAG of files </summary>
type FileGraph = IReadOnlyDictionary<FileIdx, FileIdx[]>

let memoize<'a, 'b when 'a : equality> f : ('a -> 'b) =
    let y = HashIdentity.Structural<'a>
    let d = new ConcurrentDictionary<'a, 'b>(y)
    fun x -> d.GetOrAdd(x, fun r -> f r)

module FileGraph =
    
    let calcTransitiveGraph (graph : FileGraph) : FileGraph =
        let transitiveGraph = Dictionary<FileIdx, FileIdx[]>()
        
        let rec calcTransitiveEdges =
            fun (idx : FileIdx) ->
                let edgeTargets = graph[idx]
                edgeTargets
                |> Array.collect calcTransitiveEdges
                |> Array.append edgeTargets
                |> Array.distinct
            |> memoize
        
        graph.Keys
        |> Seq.iter (fun idx -> calcTransitiveEdges idx |> ignore)
        
        transitiveGraph :> IReadOnlyDictionary<_,_>
        
    let collectEdges (graph : FileGraph) =
        graph
        
    let reverse (graph : FileGraph) : FileGraph =
        graph
        // Collect all edges
        |> Seq.collect (fun (KeyValue(idx, deps)) -> deps |> Array.map (fun dep -> idx, dep))
        // Group dependants of the same dependencies together
        |> Seq.groupBy (fun (idx, dep) -> dep)
        // Construct reversed graph
        |> Seq.map (fun (dep, edges) -> dep, edges |> Seq.map fst |> Seq.toArray)
        |> dict
        // Add nodes that are missing due to having no dependants
        |> fun graph ->
            graph
            |> Seq.map (fun (KeyValue(idx, deps)) ->
                match graph.TryGetValue idx with
                | true, dependants -> idx, dependants
                | false, _ -> idx, [||]
            )
        |> readOnlyDict
    