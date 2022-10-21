module FSharp.Compiler.Service.Tests.DepResolving

open System
open System.Collections.Generic
open Buildalyzer
open FSharp.Compiler.Service.Tests2.ASTVisit
open FSharp.Compiler.Syntax
open NUnit.Framework
open Newtonsoft.Json

let log (msg : string) =
    let d = DateTime.Now.ToString("HH:mm:ss.fff")
    printfn $"{d} {msg}"

/// File information for the algorithm
type FileAST =
    {
        Name : string
        Code : string
        AST : ParsedInput
    }

type List<'a> = System.Collections.Generic.List<'a>

/// All the information about a single file needed for the algorithm
type File =
    {
        // Order of the file in the project. Files with lower number cannot depend on files with higher number
        Idx : int
        Name : string
        Code : string
        AST : ParsedInput
        /// A list of top-level namespaces or a single top-level module
        Tops : LongIdent[]
        /// All partial module references found in this file's AST
        ModuleRefs : LongIdent[]
        ContainsModuleAbbreviations : bool
    }
    with member this.CodeSize = this.Code.Length

type FileResult =
    {
        Name : string
        Size : int64
        Deps : string[]
    }

type DepsGraph = IDictionary<int, int[]>

type DepsResult =
    {
        Files : File[]
        Graph : DepsGraph
    }

type References = Reference seq

/// Extract partial module references from partial module or type references
let extractModuleSegments (stuff : ReferenceOrAbbreviation seq) : LongIdent[] * bool =
    
    let refs =
        stuff
        |> Seq.choose (function | ReferenceOrAbbreviation.Reference r -> Some r | ReferenceOrAbbreviation.Abbreviation _ -> None)
        |> Seq.toArray
    let abbreviations =
        stuff
        |> Seq.choose (function | ReferenceOrAbbreviation.Reference _ -> None | ReferenceOrAbbreviation.Abbreviation a -> Some a)
        |> Seq.toArray
    
    let moduleRefs =
        refs
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
    let containsModuleAbbreviations = abbreviations.Length > 0
    
    moduleRefs, containsModuleAbbreviations

type ModuleSegment = string

type TrieNode =
    {
        // TODO Use ValueTuples if not already
        Children : IDictionary<ModuleSegment, TrieNode>
        mutable Reachable : bool
        mutable Visited : bool
        /// Files/graph nodes represented by this TrieNode
        /// All files whose top-level module/namespace are same as this TrieNode's 'path'
        GraphNodes : System.Collections.Generic.List<File>
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
let buildTrie (files : File[]) : TrieNode =
    let root = emptyTrie()
        
    let addPath (node : File) (ident : LongIdent) =
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
        
    // Add every file
    let addNode (node : File) =
        node.Tops
        |> Array.iter (addPath node)
    
    // Add all files to the Trie
    files
    |> Array.iter addNode
        
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

let analyseEfficiency (result : DepsResult) : unit =
    let totalFileSize =
        result.Files
        |> Array.sumBy (fun file -> int64(file.CodeSize))
    
    let filesDict =
        result.Files
        |> Seq.map (fun file -> file.Idx, file)
        |> dict
    
    let depths =
        result.Files
        |> Seq.map (fun file -> KeyValuePair(file.Idx, -1L))
        |> Dictionary<_,_>
    
    // Use depth-first search to calculate 'depth' of each file
    let rec depthDfs (idx : int) =
        let file = filesDict[idx]
        match depths[idx] with
        | -1L ->
            // Visit this node
            let file = filesDict[idx]
            let deepestChild =
                match result.Graph[file.Idx] with
                | [||] -> 0L
                | d -> d |> Array.map depthDfs |> Array.max
            let depth = int64(file.CodeSize) + deepestChild
            depths[idx] <- depth
            printfn $"Depth[{idx}, {file.Name}] = {depth}"
            depth
        | depth ->
            depth
    
    // Run DFS for every file node, collect the maximum depth found
    let maxDepth =
        result.Files
        |> Array.map (fun f -> depthDfs f.Idx)
        |> Array.max
        
    printfn $"Total file size: {totalFileSize}. Max depth: {maxDepth}. Max Depth/Size = {maxDepth / totalFileSize}"

let detectFileDependencies (nodes : FileAST[]) : DepsResult =
    // Create ASTs, extract module refs
    let nodes =
        nodes
        |> Array.Parallel.mapi (fun i {Name = name; Code = code; AST = ast} -> 
            let typeAndModuleRefs =
                try
                    visit ast |> Some
                with ex when (ex.Message.Contains("sig") || ex.Message.Contains("abbreviations")) ->
                    None
            match typeAndModuleRefs with
            | None -> None
            | Some typeAndModuleRefs ->
                let top = topModuleOrNamespaces ast
                let moduleRefs, containsModuleAbbreviations = extractModuleSegments typeAndModuleRefs
                {
                    Idx = i
                    Name = name
                    Code = code
                    AST = ast
                    Tops = top
                    ModuleRefs = moduleRefs
                    ContainsModuleAbbreviations = containsModuleAbbreviations
                }
                |> Some
        )
        |> Array.choose id
        
    log "ASTs traversed"
    
    let filesWithModuleAbbreviations =
        nodes
        |> Array.filter (fun n -> n.ContainsModuleAbbreviations)
        |> Array.map (fun f -> f.Idx)
        
    let allIndices =
        nodes
        |> Array.map (fun f -> f.Idx)
        
    let trie = buildTrie nodes
    
    // Find dependencies for all files (can be in parallel)
    let graph =
        nodes
        |> Array.Parallel.map (fun node ->
            match node.ContainsModuleAbbreviations with
            | true -> node.Idx, allIndices
            | false ->
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
            
            // Add top-level module/namespaces as the first reference (possibly not necessary as maybe already in the list)
            // TODO When multiple top-level namespaces exist, we should check that it's OK to add all of them at the start (out of order). 
            let moduleRefs =
                Array.append node.Tops node.ModuleRefs
            
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
                |> Seq.map (fun n -> n.Idx)
                |> Seq.toArray
                
            let finalDeps = Array.append deps filesWithModuleAbbreviations
                
            node.Idx, finalDeps
        )
        |> dict
    
    let res =
        {
            DepsResult.Files = nodes
            DepsResult.Graph = graph
        }
    log "Done"
    //analyseEfficiency graph
    res

[<Test>]
let TestDepsResolver() =
   
    let WithAbbreviations =
        """
module Abbr

module X = A
"""
    
    let A_fsi =
        """
module A
val a : int
type X = int
"""
    
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
        "Abbr.fs", WithAbbreviations
        "A.fsi", A_fsi
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
        |> List.map (fun (name, code) -> {FileAST.Name = name; FileAST.Code = code; FileAST.AST = parseSourceCode(name, code)})
        |> List.toArray
    
    let graph = detectFileDependencies nodes

    printfn "Detected file dependencies:"
    graph.Graph
    |> Seq.iter (fun (KeyValue(idx, deps)) -> printfn $"{graph.Files[idx].Name} -> %+A{deps |> Array.map(fun d -> graph.Files[d].Name)}")

[<Test>]
let Test () =
    log "start"
    let m = AnalyzerManager()
    let projectFile = @"C:\projekty\fsharp\fsharp_main\src\Compiler\FSharp.Compiler.Service.fsproj"
    // let projectFile = @"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj"
    let analyzer = m.GetProject(projectFile)
    let results = analyzer.Build()
    log "built"
    
    let res = results.Results |> Seq.head
    let files = res.SourceFiles
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
            {Name = f; Code = code; AST = ast}
        )
    let N = files.Length
    log $"{N} files read and parsed"
    
    let graph = detectFileDependencies files
    log "deps detected"
    
    let totalDeps = graph.Graph |> Seq.sumBy (fun (KeyValue(idx, deps)) -> deps.Length)
    let maxPossibleDeps = (N * (N-1)) / 2 
    
    let graph = graph.Graph |> Seq.map (fun (KeyValue(idx, deps)) -> graph.Files[idx].Name, deps |> Array.map (fun d -> graph.Files[d].Name)) |> dict
    let json = JsonConvert.SerializeObject(graph, Formatting.Indented)
    System.IO.File.WriteAllText("deps_graph.json", json)
    
    printfn $"Analysed {N} files, detected {totalDeps}/{maxPossibleDeps} file dependencies:"
    printfn "Wrote graph as json in deps_graph.json"