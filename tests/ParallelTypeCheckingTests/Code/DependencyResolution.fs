module ParallelTypeCheckingTests.DepResolving
#nowarn "1182"
#nowarn "40"

open System
open System.Collections.Generic
open ParallelTypeCheckingTests
open ParallelTypeCheckingTests.FileInfoGathering
open ParallelTypeCheckingTests.Types
open FSharp.Compiler.Syntax

let log (msg : string) =
    let d = DateTime.Now.ToString("HH:mm:ss.fff")
    printfn $"{d} {msg}"

type List<'a> = System.Collections.Generic.List<'a>

type FileResult =
    {
        Name : string
        Size : int64
        Deps : string[]
    }

type DepsGraph = Graph<File>

type DepsResult =
    {
        Files : FileData[]
        Graph : DepsGraph
    }
    with member this.Edges() = this.Graph |> Graph.collectEdges

type References = Reference seq

/// Algorithm for automatically detecting (lack of) file dependencies based on their AST contents
[<RequireQualifiedAccess>]
module internal DependencyResolution =

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
            /// <summary>Files represented by this TrieNode.
            /// All files whose any top-level module/namespace are the same as this TrieNode's 'path'.</summary>
            Files : System.Collections.Generic.List<FileData>
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
            Files = List<_>(trie.Files)
            Children = children
            PotentialPrefix = trie.PotentialPrefix
            Reachable = trie.Reachable
        }

    let emptyTrie () : TrieNode =
        {
            TrieNode.Children = Dictionary([])
            PotentialPrefix = false
            Reachable = false
            Files = emptyList()
        }

    /// <summary>Build initial Trie from files and their top items</summary>
    let buildTrie (files : FileData[]) : TrieNode =
        let root = emptyTrie()
            
        let addFileIdent (file : FileData) (ident : SimpleId) =
            // Go down from root using segments of the identifier, possibly extending the Trie with new nodes.
            let mutable node = root
            for segment in ident do
                let child =
                    match node.Children.TryGetValue segment with
                    // Child exists
                    | true, child ->
                        child
                    // Child doesn't exist
                    | false, _ ->
                        let child = emptyTrie()
                        node.Children[segment] <- child
                        child
                node <- child
            // Add the file to the found leaf's list
            node.Files.Add(file)
            
        let addFile (file : FileData) =
            file.Data.Tops
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
    let detectFileDependencies (files : SourceFiles) : DepsResult =
        let nodes = gatherForAllFiles files
        log "ASTs traversed"
        let backed = nodes |> Array.filter (fun n -> n.File.FsiBacked)
        printfn $"{backed.Length} backed files found"
        let filesWithModuleAbbreviations =
            nodes
            |> Array.filter (fun n ->
                n.Data.Abbreviations
                |> Array.exists (function Abbreviation.ModuleAbbreviation -> true | _ -> false))
            
        let trie = buildTrie nodes
        
        let fsiFiles =
            nodes
            |> Array.filter (fun f -> match f.File.AST with | ASTOrFsix.AST (ParsedInput.SigFile _) -> true | _ -> false)
        
        let processFile (node : FileData) =
            let deps =
                let fsiDep =
                    if node.File.FsiBacked then
                        nodes
                        |> Array.find (fun x -> x.File.QualifiedName = node.File.QualifiedName)
                        |> fun x -> [|x|]
                    else
                        [||]
                // Assume that a file with module abbreviations can depend on anything
                match node.Data.Abbreviations |> Array.isEmpty |> not with
                | true -> nodes |> Array.map (fun n -> n.File)
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
                    let rec walkDownAndMark (id : SimpleId) (node : TrieNode) =
                        match id with
                        // Reached end of the identifier - new reachable node 
                        | [||] ->
                            markPotentialPrefix node
                        // More segments exist
                        | id ->
                            // Visit (not 'reach') the TrieNode
                            markReachable node
                            match node.Children.TryGetValue id[0] with
                            // A child for the segment exists - continue there
                            | true, child ->
                                walkDownAndMark id[1..] child
                            // A child for the segment doesn't exist - stop, since we don't care about the non-existent part of the Trie
                            | false, _ ->
                                ()
                    
                    let processRef (id : SimpleId) =
                        // Start at every potential prefix,
                        List<_>(potentialPrefixes) // Copy the list for iteration as the original is going to be extended.
                        // Extend potential prefixes with this 'id'
                        |> Seq.iter (walkDownAndMark id)
                    
                    // Add top-level module/namespaces as the first reference (possibly not necessary as maybe already in the list)
                    // TODO When multiple top-level namespaces exist, we should check that it's OK to add all of them at the start (out of order).
                    // Later on we might want to preserve the order by returning the top-level namespaces interleaved with module refs 
                    let moduleRefs =
                        Array.append node.Data.Tops node.Data.ModuleRefs
                    
                    // Process module refs in order, marking more and more TrieNodes as reachable and potential prefixes
                    moduleRefs
                    |> Array.iter processRef
                    
                    // Force .fsi files to depend on all other (previous) .fsi files - avoids the issue of TcEnv being overriden  
                    let additionalFsiDeps =
                        if node.File.Name.EndsWith ".fsi" then
                            nodes
                        else
                            [||]
                    
                    // Collect files from all reachable TrieNodes
                    let deps =
                        reachable
                        |> Seq.collect (fun node -> node.Files)
                        // Assume that this file depends on all files that have any module abbreviations - this is probably unnecessary.
                        // TODO Handle module abbreviations in a better way
                        |> Seq.append filesWithModuleAbbreviations
                        |> Seq.append additionalFsiDeps
                        |> Seq.append fsiDep
                        |> Seq.map (fun f -> f.File)
                        |> Seq.toArray
                    
                    deps
                // We know a file can't depend on a file further down in the project definition (or on itself)
                |> Array.filter (fun dep -> dep.Idx < node.File.Idx)
                // TODO Temporary - bring this back
                // Filter out deps onto .fs files that have backing .fsi files
                |> Array.filter (fun dep -> not dep.FsiBacked)
                |> Array.distinct
                
            // Return the node and its dependencies
            node.File, deps
        
        // Find dependencies for all files
        let graph =
            nodes
            // TODO Async + cancellations
            // |> Array.map processFile
            |> Array.Parallel.map processFile
            |> readOnlyDict
        
        {
            DepsResult.Files = nodes
            DepsResult.Graph = graph
        }

/// <summary>
/// Calculate and print some stats about the expected parallelism factor of a dependency graph 
/// </summary>
let analyseEfficiency (result : DepsResult) : unit =
    let graph = result.Graph
    let totalSize1 =
        graph
        |> Seq.sumBy (fun (KeyValue(_k,v)) -> v.Length)
    let t =
        graph
        |> Graph.transitive 
    let totalSize2 =
        t
        |> Seq.sumBy (fun (KeyValue(_k,v)) -> v.Length)
    
    printfn $"Non-transitive size: {totalSize1}, transitive size: {totalSize2}"
    
    
    let totalFileSize =
        result.Files
        |> Array.sumBy (fun file -> int64(file.CodeSize))
    
    // Use depth-first search to calculate 'depth' of each file
    let rec depthDfs =
        Utils.memoize (
            fun (file : File) ->
                let deepestChild =
                    match result.Graph[file] with
                    | [||] -> 0L
                    | d -> d |> Array.map depthDfs |> Array.max
                let depth = int64(file.CodeSize) + deepestChild
                depth
        )
    
    // Run DFS for every file node, collect the maximum depth found
    let maxDepth =
        result.Files
        |> Array.map (fun f -> depthDfs f.File)
        |> Array.max
        
    log $"Total file size: {totalFileSize}. Max depth: {maxDepth}. Max Depth/Size = %.1f{100.0 * double(maxDepth) / double(totalFileSize)}%%"
