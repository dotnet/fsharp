module FSharp.Compiler.Service.Tests2.DepResolving

open System
open System.Collections.Generic
open FSharp.Compiler.Service.Tests2.ASTVisit
open FSharp.Compiler.Syntax

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

let calcTransitiveGraph (graph : IDictionary<int, int[]>) : IDictionary<int, int[]> =
    let transitiveDeps = Dictionary<int, int[]>()
    
    let rec calcTransitiveDepsInner (idx : int) =
        match transitiveDeps.TryGetValue idx with
        | true, deps -> deps
        | false, _ ->
            let directDeps = graph[idx]
            let deps =
                directDeps
                |> Array.collect (
                    fun dep ->
                        calcTransitiveDepsInner dep
                        |> Array.append [|dep|]
                )
                |> Array.distinct
            transitiveDeps[idx] <- deps
            deps
    
    graph.Keys
    |> Seq.map (fun idx -> idx, calcTransitiveDepsInner idx)
    |> dict


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
                // Remove the last segment as it contains the type name
                match x.Ident.Length with
                | 0
                | 1 -> None
                | n -> x.Ident.GetSlice(Some 0, n - 2 |> Some) |> Some
        )
        |> Seq.toArray
    let containsModuleAbbreviations = abbreviations.Length > 0
    
    moduleRefs, containsModuleAbbreviations

/// Algorithm for automatically detecting (lack of) file dependencies based on their AST contents
[<RequireQualifiedAccess>]
module internal AutomatedDependencyResolving =

    /// Eg. 'A' and 'B' in "module A.B"
    type ModuleSegment = string

    /// <summary>A node in the <a href="https://en.wikipedia.org/wiki/Trie">Trie</a> structure used in the algorithm</summary>
    type TrieNode =
        {
            Children : IDictionary<ModuleSegment, TrieNode>
            /// <summary>Is this node a potential prefix that can be extended with subsequent partial references?</summary>
            /// <example>A potential prefix 'A' is extended by 'B' in here: "module A.B = (); open A; open B".</example>
            mutable PotentialPrefix : bool
            /// <summary>Is this node reachable?</summary>
            /// <example>The following: "let x = A.B.x" has three reachable nodes: empty, 'A', 'A.B'.</example>
            mutable Reachable : bool
            /// <summary>Files/graph nodes represented by this TrieNode.
            /// All files whose any top-level module/namespace are the same as this TrieNode's 'path'.</summary>
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
                    KeyValuePair(segment, cloneTrie child)
                )
            Dictionary<_,_>(children)
        {
            // TODO Can avoid copying here by using an immutable structure or just using the same reference
            GraphNodes = List<_>(trie.GraphNodes)
            Children = children
            PotentialPrefix = trie.PotentialPrefix
            Reachable = trie.Reachable
        }

    let emptyTrie () : TrieNode =
        {
            TrieNode.Children = Dictionary([])
            PotentialPrefix = false
            Reachable = false
            GraphNodes = emptyList()
        }

    /// <summary>Build initial Trie from files and their top items</summary>
    let buildTrie (files : File[]) : TrieNode =
        let root = emptyTrie()
            
        let addFileIdent (file : File) (ident : LongIdent) =
            // Go down from root using segments of the identifier, possibly extending the Trie with new nodes.
            let mutable node = root
            for segment in ident do
                let child =
                    match node.Children.TryGetValue segment.idText with
                    // Child exists
                    | true, child ->
                        child
                    // Child doesn't exist
                    | false, _ ->
                        let child = emptyTrie()
                        node.Children[segment.idText] <- child
                        child
                node <- child
            // Add the file to the found leaf's list
            node.GraphNodes.Add(file)
            
        let addFile (file : File) =
            file.Tops
            |> Array.iter (addFileIdent file)
        
        // For every file add all its top-level modules/namespaces to the Trie
        files
        |> Array.iter addFile
            
        root

    let rec searchInTrie (trie : TrieNode) (path : LongIdent) : TrieNode option =
        match path with
        // We reached the end of the path - return the current node 
        | [] -> Some trie
        // More segments to go
        | segment :: rest ->
            match trie.Children.TryGetValue(segment.idText) with
            | true, child ->
                searchInTrie child rest
            | false, _ ->
                None

    /// <summary>
    /// Detect file dependencies based on their AST contents
    /// </summary>
    /// <remarks>
    /// Highly-parallel algorithm that uses AST contents to figure out which files might depend on what other files.
    /// Uses the <a href="https://en.wikipedia.org/wiki/Trie">Trie</a> data structure.
    /// </remarks>
    /// <param name="nodes">A list of already parsed source files</param>
    let detectFileDependencies (nodes : FileAST[]) : DepsResult =
        // Extract necessary information from all files in parallel - top-level items and all (partial) module references
        let nodes =
            nodes
            |> Array.Parallel.mapi (fun i {Name = name; Code = code; AST = ast} -> 
                let typeAndModuleRefs = extractModuleRefs ast
                let moduleRefs, containsModuleAbbreviations = extractModuleSegments typeAndModuleRefs
                let top = topModuleOrNamespaces ast
                {
                    Idx = i
                    Name = name
                    Code = code
                    AST = ast
                    Tops = top
                    ModuleRefs = moduleRefs
                    ContainsModuleAbbreviations = containsModuleAbbreviations
                }
            )
            
        log "ASTs traversed"
        
        let filesWithModuleAbbreviations =
            nodes
            |> Array.filter (fun n -> n.ContainsModuleAbbreviations)
            |> Array.map (fun f -> f.Idx)
            
        let allIndices =
            nodes
            |> Array.map (fun f -> f.Idx)
            
        let trie = buildTrie nodes
        
        // Find dependencies for all files
        let graph =
            nodes
            |> Array.Parallel.map (fun node ->
                let deps =
                    // Assume that a file with module abbreviations can depend on anything
                    match node.ContainsModuleAbbreviations with
                    | true -> allIndices
                    | false ->
                        // Clone the original Trie as we're going to mutate the copy
                        let trie = cloneTrie trie
                        
                        // Keep a list of reachable nodes (ie. potential prefixes and their ancestors)
                        let reachable = emptyList<TrieNode>()
                        let markReachable (node : TrieNode) =
                            if not node.Reachable then
                                node.Reachable <- true
                                reachable.Add(node)
                        
                        // Keep a list of potential prefixes
                        let potentialPrefixes = emptyList<TrieNode>()
                        let markPotentialPrefix (node : TrieNode) =
                            if not node.PotentialPrefix then
                                node.PotentialPrefix <- true
                                potentialPrefixes.Add(node)
                            // Every potential prefix is reachable
                            markReachable node
                        
                        // Mark root (empty prefix) as a potential prefix
                        markPotentialPrefix trie
                        
                        /// <summary>
                        /// Walk down from 'node' using 'id' as the path.
                        /// Mark all visited nodes as reachable, and the final node as a potential prefix.
                        /// Short-circuit when a leaf is reached.
                        /// </summary>
                        /// <remarks>
                        /// When the path leads outside the Trie, the Trie is not extended and no node is marked as a potential prefix.
                        /// This is just a performance optimisation - all the files are linked to already existing nodes, so there is no need to create and visit deeper nodes. 
                        /// </remarks>
                        let rec walkDownAndMark (id : LongIdent) (node : TrieNode) =
                            match id with
                            // Reached end of the identifier - new reachable node 
                            | [] ->
                                markPotentialPrefix node
                            // More segments exist
                            | segment :: rest ->
                                // Visit (not 'reach') the TrieNode
                                markReachable node
                                match node.Children.TryGetValue(segment.idText) with
                                // A child for the segment exists - continue there
                                | true, child ->
                                    walkDownAndMark rest child
                                // A child for the segment doesn't exist - stop, since we don't care about the non-existent part of the Trie
                                | false, _ ->
                                    ()
                        
                        let processRef (id : LongIdent) =
                            // Start at every potential prefix,
                            List<_>(potentialPrefixes) // Copy the list for iteration as the original is going to be extended.
                            // Extend potential prefixes with this 'id'
                            |> Seq.iter (walkDownAndMark id)
                        
                        // Add top-level module/namespaces as the first reference (possibly not necessary as maybe already in the list)
                        // TODO When multiple top-level namespaces exist, we should check that it's OK to add all of them at the start (out of order).
                        // Later on we might want to preserve the order by returning the top-level namespaces interleaved with module refs 
                        let moduleRefs =
                            Array.append node.Tops node.ModuleRefs
                        
                        // Process module refs in order, marking more and more TrieNodes as reachable and potential prefixes
                        moduleRefs
                        |> Array.iter processRef
                        
                        // Collect files from all reachable TrieNodes
                        let deps =
                            reachable
                            |> Seq.collect (fun node -> node.GraphNodes)
                            |> Seq.map (fun n -> n.Idx)
                            // Assume that this file depends on all files that have any module abbreviations
                            // TODO Handle module abbreviations in a better way  
                            |> Seq.append filesWithModuleAbbreviations
                            |> Seq.toArray
                        
                        deps
                    // We know a file can't depend on a file further down in the project definition (or on itself)
                    |> Array.filter (fun depIdx -> depIdx < node.Idx)
                    
                // Return the node and its dependencies
                node.Idx, deps
            )
            |> dict
        
        // Calculate transitive closure of the graph
        let graph = calcTransitiveGraph graph
        
        let res =
            {
                DepsResult.Files = nodes
                DepsResult.Graph = graph
            }
        res

/// <summary>
/// Calculate and print some stats about the expected parallelism factor of a dependency graph 
/// </summary>
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
            depth
        | depth ->
            // Already visited
            depth
    
    // Run DFS for every file node, collect the maximum depth found
    let maxDepth =
        result.Files
        |> Array.map (fun f -> depthDfs f.Idx)
        |> Array.max
        
    log $"Total file size: {totalFileSize}. Max depth: {maxDepth}. Max Depth/Size = %.1f{100.0 * double(maxDepth) / double(totalFileSize)}%%"
