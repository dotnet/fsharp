module FSharp.Compiler.Service.Tests.DepResolving

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
type Graph = (Node * List<Node>)[]

let extractModuleSegments (stuff : Stuff) : LongIdent[] =
    stuff
    |> Seq.choose (fun x ->
        match x.Kind with
        | ModuleOrNamespace -> x.Ident |> Some
        | Type ->
            match x.Ident.Length with
            | 0
            | 1 -> None
            | n -> x.Ident.GetSlice(Some 0, n - 1 |> Some) |> Some
    )
    |> Seq.toArray

type ModuleSegment = string

type TrieNode =
    {
        // parent?
        // TODO Use ValueTuples if not already
        Children : System.Collections.Generic.IDictionary<ModuleSegment, TrieNode>
        mutable Reachable : bool
        /// Files/graph nodes represented by this TrieNode
        /// All files whose top-level module/namespace are same as this TrieNode's 'path'
        GraphNodes : System.Collections.Generic.List<Node>
    }

let emptyList<'a> () =
    System.Collections.Generic.List<'a>()

let cloneTrie (trie : TrieNode) : TrieNode =
    failwith unsupported // TODO

let emptyTrie () : TrieNode =
    {
        TrieNode.Children = dict([])
        Reachable = false
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

let search (trie : TrieNode) (path : LongIdent) =
    trie

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
        
        // Keep a list of reachable nodes
        let reachable = emptyList<TrieNode>()
        
        let markReachable (node : TrieNode) =
            if not node.Reachable then
                printfn $"New node reachable"
                node.Reachable <- true
                reachable.Add(node)
        
        // Mark two nodes as reachable:
        // - root (no prefix)
        // - top-level module/namespace
        markReachable trie
        let topNode = search trie node.Top
        markReachable topNode
        
        // Process module refs in order, marking more and more TrieNodes as reachable
        let processRef (id : LongIdent) =
            ()
        node.ModuleRefs
        |> Array.iter processRef
        
        // Collect files from all reachable TrieNodes
        let reachableItems =
            reachable
            |> Seq.collect (fun node -> node.GraphNodes)
        node, List<Node>(reachableItems)
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