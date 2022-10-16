module FSharp.Compiler.Service.Tests.DepResolving

open System.Collections.Generic
open FSharp.Compiler.Service.Tests2.SyntaxTreeTests.TypeTests
open FSharp.Compiler.Syntax
open NUnit.Framework

/// File * AST
type FileAST = string * ParsedInput

type List<'a> = System.Collections.Generic.List<'a>

type Node =
    {
        Name : string
        AST : ParsedInput
        Top : LongIdent
        ModuleRefs : LongIdent[]
    }

/// Filenames with dependencies
type Graph = (Node * Node[])[]

let extractModuleSegments (stuff : Stuff) : LongIdent[] =
    stuff
    |> Seq.choose (fun x ->
        match x.Kind with
        | ModuleOrNamespace -> x.Ident |> Some
        | Type ->
            match x.Ident.Length with
            | 0
            | 1 -> None
            | n -> x.Ident.GetSlice(Some 0, n - 2 |> Some) |> Some
    )
    |> Seq.toArray

type ModuleSegment = string

type TrieNode =
    {
        // parent?
        // TODO Use ValueTuples if not already
        Children : System.Collections.Generic.IDictionary<ModuleSegment, TrieNode>
        mutable Reachable : bool
        mutable Visited : bool
        /// Files/graph nodes represented by this TrieNode
        /// All files whose top-level module/namespace are same as this TrieNode's 'path'
        GraphNodes : System.Collections.Generic.List<Node>
    }

let emptyList<'a> () =
    System.Collections.Generic.List<'a>()

let rec cloneTrie (trie : TrieNode) : TrieNode =
    let children =
        // TODO Perf
        let children =
            trie.Children
            |> Seq.map (fun (KeyValue(segment, child)) ->
                segment, cloneTrie child
            )
            |> dict
        // TODO Avoid tow dicts
        System.Collections.Generic.Dictionary<_,_>(children)
    {
        GraphNodes = List<_>(trie.GraphNodes)
        Children = children
        Reachable = trie.Reachable
        Visited = trie.Visited
    }

let emptyTrie () : TrieNode =
    {
        TrieNode.Children = Dictionary([])
        Reachable = false
        Visited = false
        GraphNodes = emptyList()
    }

/// Build initial Trie from files
let buildTrie (nodes : Node[]) : TrieNode =
    let root = emptyTrie()
        
    // Add every file
    let addItem (node : Node) =
        let ident = node.Top
        
        let mutable trieNode = root
        // Go through module segments, possibly extending the Trie with new nodes
        for segment in ident do
            let child =
                match trieNode.Children.TryGetValue segment.idText with
                // Child exists
                | true, child ->
                    child
                // Child doesn't exist
                | false, _ ->
                    let child = emptyTrie()
                    trieNode.Children[segment.idText] <- child
                    child
            trieNode <- child
            
        // Add the node to the found leaf's list
        trieNode.GraphNodes.Add(node)
    
    // Add all files to the Trie
    nodes
    |> Array.iter addItem
        
    root

let rec search (trie : TrieNode) (path : LongIdent) : TrieNode option =
    let mutable node = trie
    match path with
    | [] -> Some trie
    | segment :: rest ->
        match trie.Children.TryGetValue(segment.idText) with
        | true, child ->
            search child rest
        | false, _ ->
            None

let algorithm (nodes : FileAST list) : Graph =
    // Create ASTs, extract module refs
    let nodes =
        nodes
        |> List.map (fun (name, ast) ->
            let typeAndModuleRefs = visit ast 
            let top = topModuleOrNamespace ast
            let moduleRefs = extractModuleSegments typeAndModuleRefs
            {
                Name = name
                AST = ast
                Top = top
                ModuleRefs = moduleRefs
            }
        )
        |> List.toArray
        
    let trie = buildTrie nodes
    
    // Find dependencies for all files (can be in parallel)
    nodes
    |> Array.map (fun node ->
        let trie = cloneTrie trie
        
        // Keep a list of visited nodes (ie. all reachable nodes and all their ancestors)
        let visited = emptyList<TrieNode>()
        
        let markVisited (node : TrieNode) =
            if not node.Visited then
                printfn $"New node visited"
                node.Visited <- true
                visited.Add(node)
        
        // Keep a list of reachable nodes (ie. ones that can be prefixes for later module/type references)
        let reachable = emptyList<TrieNode>()
        
        let markReachable (node : TrieNode) =
            if not node.Reachable then
                printfn $"New node reachable"
                node.Reachable <- true
                reachable.Add(node)
        
        // Mark root (no prefix) as reachable and visited
        markReachable trie
        markVisited trie
        
        let rec extend (id : LongIdent) (node : TrieNode) =
            let rec extend (node : TrieNode) (id : LongIdent) =
                match id with
                // Reached end of the identifier - new reachable node 
                | [] ->
                    Some node
                // More segments exist
                | segment :: rest ->
                    // Visit (not 'reach') the TrieNode
                    markVisited node
                    match node.Children.TryGetValue(segment.idText) with
                    // A child for the segment exists - continue there
                    | true, child ->
                        extend child rest
                    // A child for the segment doesn't exist - stop, since we don't care about the non-existent part of the Trie
                    | false, _ ->
                        None
            extend node id
        
        // Process module refs in order, marking more and more TrieNodes as reachable
        let processRef (id : LongIdent) =
            let newReachables =
                // Start at every reachable node,
                reachable
                // extend a reachable node by 'id', but without creating new nodes, mark all seen nodes as visited and the final one as reachable
                |> Seq.choose (extend id)
                |> Seq.toArray
            reachable.AddRange(newReachables)
        
        // Add top-level module/namespace as the first reference (possibly not necessary as maybe already in the list)
        let moduleRefs =
            Array.append [|node.Top|] node.ModuleRefs
        
        // Process all refs
        moduleRefs
        |> Array.iter processRef
        
        // Collect files from all visited TrieNodes
        let reachableItems =
            visited
            |> Seq.collect (fun node -> node.GraphNodes)
            |> Seq.toArray
            
        // Return the node and its dependencies
        node, reachableItems
    )

[<Test>]
let Foo() =
    
    let A =
        """
module A
open B
let x = B.x
"""
    let B =
        """
module B
let x = 3
"""

    let files = [
        "A", A
        "B", B
    ]
    let nodes =
        files
        |> List.map (fun (name, code) -> name, getParseResults code)
    
    let graph = algorithm nodes
    
    printfn $"%+A{graph}"