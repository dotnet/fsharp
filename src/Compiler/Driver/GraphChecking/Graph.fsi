namespace FSharp.Compiler.GraphChecking

open System.Collections.Generic

/// A Directed Acyclic Graph (DAG) of arbitrary nodes.
type internal Graph<'Node> = IReadOnlyDictionary<'Node, 'Node[]>

/// Functions for operating on the Graph type.
module internal Graph =
    /// Build the graph.
    val make: nodeDeps: seq<'Node * 'Node array> -> IReadOnlyDictionary<'Node, 'Node array> when 'Node: equality
    val map<'a, 'b when 'b: equality> : f: ('a -> 'b) -> graph: Graph<'a> -> Graph<'b>
    /// Create a transitive closure of the graph in O(n^2) time (but parallelise it).
    /// The resulting graph contains edge A -> C iff the input graph contains a (directed) non-zero length path from A to C.
    val transitive<'Node when 'Node: equality> : graph: Graph<'Node> -> Graph<'Node>
    /// Create a reverse of the graph.
    val reverse<'Node when 'Node: equality> : originalGraph: Graph<'Node> -> Graph<'Node>
    /// Print the contents of the graph to the standard output.
    val print: graph: Graph<'Node> -> unit
    /// Create a simple Mermaid graph and save it under the path specified.
    val serialiseToMermaid: path: string -> graph: Graph<FileIndex * string> -> unit
