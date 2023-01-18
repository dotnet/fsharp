namespace FSharp.Compiler.GraphChecking

open System.Collections.Generic

/// <summary> Directed Acyclic Graph (DAG) of arbitrary nodes </summary>
type internal Graph<'Node> = IReadOnlyDictionary<'Node, 'Node[]>

module internal Graph =
    val make: nodeDeps: seq<'Node * 'Node array> -> IReadOnlyDictionary<'Node, 'Node array> when 'Node: equality
    val map<'a, 'b when 'b: equality> : f: ('a -> 'b) -> graph: Graph<'a> -> Graph<'b>
    /// Create a transitive closure of the graph
    val transitive<'Node when 'Node: equality> : graph: Graph<'Node> -> Graph<'Node>
    /// Create a reverse of the graph
    val reverse<'Node when 'Node: equality> : originalGraph: Graph<'Node> -> Graph<'Node>
    val print: graph: Graph<'Node> -> unit
    val serialiseToMermaid: path: string -> graph: Graph<int * string> -> unit
