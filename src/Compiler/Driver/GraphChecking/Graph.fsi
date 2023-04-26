namespace FSharp.Compiler.GraphChecking

open System.Collections.Generic

/// A Directed Acyclic Graph (DAG) of arbitrary nodes.
type internal Graph<'Node> = IReadOnlyDictionary<'Node, 'Node array>

/// Functions for operating on the Graph type.
module internal Graph =
    /// Build the graph.
    val make: nodeDeps: seq<'Node * 'Node array> -> Graph<'Node> when 'Node: equality
    val map<'T, 'U when 'U: equality> : f: ('T -> 'U) -> graph: Graph<'T> -> Graph<'U>
    /// Create a transitive closure of the graph in O(n^2) time (but parallelize it).
    /// The resulting graph contains edge A -> C iff the input graph contains a (directed) non-zero length path from A to C.
    val transitive<'Node when 'Node: equality> : graph: Graph<'Node> -> Graph<'Node>
    /// Get a sub-graph of the graph containing only the nodes reachable from the given node.
    val subGraphFor: node: 'Node -> graph: Graph<'Node> -> Graph<'Node> when 'Node: equality
    /// Create a reverse of the graph.
    val reverse<'Node when 'Node: equality> : originalGraph: Graph<'Node> -> Graph<'Node>
    /// Returns layers of leaves repeatedly removed from the graph until there's nothing left
    val leafSequence: graph: Graph<'Node> -> Set<'Node> seq
    /// Print the contents of the graph to the standard output.
    val print: graph: Graph<'Node> -> unit
    /// Create a simple Mermaid graph and save it under the path specified.
    val serialiseToMermaid: path: string -> graph: Graph<FileIndex * string> -> unit
