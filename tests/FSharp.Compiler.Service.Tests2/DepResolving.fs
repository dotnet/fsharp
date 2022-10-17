module FSharp.Compiler.Service.Tests.DepResolving

open System
open System.Collections.Generic
open Buildalyzer
open FSharp.Compiler.Service.Tests2.SyntaxTreeTests.TypeTests
open FSharp.Compiler.Syntax
open NUnit.Framework
open Newtonsoft.Json

let log (msg : string) =
    let d = DateTime.Now.ToString("HH:mm:ss.fff")
    printfn $"{d} {msg}"

/// File * AST
type FileAST = string * ParsedInput

type List<'a> = System.Collections.Generic.List<'a>

type Node =
    {
        Name : string
        AST : ParsedInput
        Top : LongIdent
        ModuleRefs : LongIdent[]
        // Order of the file in the project. Files with lower number cannot depend on files with higher number
        Idx : int
    }

/// Filenames with dependencies
type Graph = (string * string[])[]

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
        Children : IDictionary<ModuleSegment, TrieNode>
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
        Dictionary<_,_>(children)
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

let rec searchInTrie (trie : TrieNode) (path : LongIdent) : TrieNode option =
    let mutable node = trie
    match path with
    | [] -> Some trie
    | segment :: rest ->
        match trie.Children.TryGetValue(segment.idText) with
        | true, child ->
            searchInTrie child rest
        | false, _ ->
            None

let detectFileDependencies (nodes : FileAST[]) : Graph =
    // Create ASTs, extract module refs
    let nodes =
        nodes
        |> Array.Parallel.mapi (fun i (name, ast) -> 
            let typeAndModuleRefs =
                try
                    visit ast |> Some
                with ex when (ex.Message.Contains("sig") || ex.Message.Contains("abbreviations")) ->
                    None
            match typeAndModuleRefs with
            | None -> None
            | Some typeAndModuleRefs ->
                let top = topModuleOrNamespace ast
                let moduleRefs = extractModuleSegments typeAndModuleRefs
                {
                    Name = name
                    AST = ast
                    Top = top
                    ModuleRefs = moduleRefs
                    Idx = i
                }
                |> Some
        )
        |> Array.choose id
        
    let trie = buildTrie nodes
    
    // Find dependencies for all files (can be in parallel)
    nodes
    |> Array.Parallel.map (fun node ->
        let trie = cloneTrie trie
        
        // Keep a list of visited nodes (ie. all reachable nodes and all their ancestors)
        let visited = emptyList<TrieNode>()
        
        let markVisited (node : TrieNode) =
            if not node.Visited then
                node.Visited <- true
                visited.Add(node)
        
        // Keep a list of reachable nodes (ie. ones that can be prefixes for later module/type references)
        let reachable = emptyList<TrieNode>()
        
        let markReachable (node : TrieNode) =
            if not node.Reachable then
                node.Reachable <- true
                reachable.Add(node)
            markVisited node
        
        // Mark root (no prefix) as reachable and visited
        markReachable trie
        
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
            newReachables
            |> Array.iter markReachable
        
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
        let deps =
            reachableItems
            // We know a file can't depend on a file further down in the project definition (or on itself)
            |> Seq.filter (fun n -> n.Idx < node.Idx)
            |> Seq.map (fun n -> n.Name)
            |> Seq.toArray
        node.Name, deps
    )

[<Test>]
let TestDepsResolver() =
   
    let A =
        """
module A
let a = 3
type X = int
""" 
    let B =
        """
namespace B
let b = 3
"""
    let C =
        """
module C.X
let c = 3
"""
    let D =
        """
module D
let d : A.X = 3
"""
    let E =
        """
module E
let e = C.X.x
open A
let x = a
"""
    let F =
        """
module F
open C
let x = X.c
"""
    let G =
        """
namespace GH
type A = int
"""
    let H =
        """
namespace GH
type B = int
"""

    let files = [
        "A.fs", A
        "B.fs", B
        "C.fs", C
        "D.fs", D
        "E.fs", E
        "F.fs", F
        "G.fs", G
        "H.fs", H
    ]
    let nodes =
        files
        |> List.map (fun (name, code) -> name, parseSourceCode(name, code))
        |> List.toArray
    
    let graph = detectFileDependencies nodes

    printfn "Detected file dependencies:"
    graph
    |> Array.iter (fun (file, deps) -> printfn $"{file} -> %+A{deps}")

[<Test>]
let Test () =
    log "start"
    let m = AnalyzerManager()
    //let projectFile = @"C:\projekty\fsharp\fsharp_main\src\Compiler\FSharp.Compiler.Service.fsproj"
    let projectFile = @"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj"
    let analyzer = m.GetProject(projectFile)
    let results = analyzer.Build()
    log "built"
    
    let res = results.Results |> Seq.head
    let files = res.SourceFiles
    // Filter out FSI files as they are not supported (ignore FSX too)
    let files =
        files
        |> Array.filter (fun f -> f.EndsWith(".fs"))
    let n =
        let args = Environment.GetCommandLineArgs()
        match args.Length with
        | 0 | 1 -> files.Length
        | l ->
            match System.Int32.TryParse(args[1]) with
            | true, n -> n
            | false, _ -> files.Length
    let files =
        files
        |> Array.take n
        |> Array.Parallel.map (fun f ->
            let code = System.IO.File.ReadAllText(f)
            let ast = getParseResults code
            f, ast
        )
    let N = files.Length
    log $"{N} files read and parsed"
    
    let graph = detectFileDependencies files
    log "deps detected"
    
    let totalDeps = graph |> Array.sumBy (fun (f, deps) -> deps.Length)
    let maxPossibleDeps = (N * (N-1)) / 2 
    //graph
    //|> Array.iter (fun (file, deps) -> printfn $"{file} -> %+A{deps}")
    
    let graph = graph |> dict
    let json = JsonConvert.SerializeObject(graph, Formatting.Indented)
    System.IO.File.WriteAllText("deps_graph.json", json)
    
    printfn $"Analysed {N} files, detected {totalDeps}/{maxPossibleDeps} file dependencies:"
    printfn "Wrote graph as json in deps_graph.json"