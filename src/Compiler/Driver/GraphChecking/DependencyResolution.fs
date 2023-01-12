module internal FSharp.Compiler.GraphChecking.DependencyResolution

open FSharp.Compiler.Syntax

// This code just looks for a path in the trie
// It could be cached and is easy to reason about.
let queryTrie (trie: TrieNode) (path: LongIdentifier) : QueryTrieNodeResult =
    let rec visit (currentNode: TrieNode) (path: LongIdentifier) =
        match path with
        | [] -> failwith "path should not be empty"
        | [ lastNodeFromPath ] ->
            match currentNode.Children.TryGetValue(lastNodeFromPath) with
            | false, _ -> QueryTrieNodeResult.NodeDoesNotExist
            | true, childNode ->
                if Set.isEmpty childNode.Files then
                    QueryTrieNodeResult.NodeDoesNotExposeData
                else
                    QueryTrieNodeResult.NodeExposesData(childNode.Files)
        | currentPath :: restPath ->
            match currentNode.Children.TryGetValue(currentPath) with
            | false, _ -> QueryTrieNodeResult.NodeDoesNotExist
            | true, childNode -> visit childNode restPath

    visit trie path

let queryTrieMemoized (trie: TrieNode) : QueryTrie =
    Internal.Utilities.Library.Tables.memoize (queryTrie trie)

// Now how to detect the deps between files?
// Process the content of each file using some state

let processOwnNamespace (queryTrie: QueryTrie) (path: LongIdentifier) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData -> state.AddOwnNamespace path
    | QueryTrieNodeResult.NodeExposesData files -> state.AddOwnNamespace(path, files)

// Helper function to process a open statement
// The statement could link to files and/or should be tracked as an open namespace
let processOpenPath (queryTrie: QueryTrie) (path: LongIdentifier) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData -> state.AddOpenNamespace path
    | QueryTrieNodeResult.NodeExposesData files -> state.AddOpenNamespace(path, files)

// Helper function to process an identifier
let processIdentifier (queryTrie: QueryTrie) (path: LongIdentifier) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData ->
        // This can occur when you have a file that uses a known namespace (for example namespace System).
        // When any other code uses that System namespace it won't find anything in the user code.
        state
    | QueryTrieNodeResult.NodeExposesData files -> state.AddDependencies files

// Typically used to folder FileContentEntry items over a FileContentQueryState
let rec processStateEntry (queryTrie: QueryTrie) (state: FileContentQueryState) (entry: FileContentEntry) : FileContentQueryState =
    match entry with
    | FileContentEntry.TopLevelNamespace (topLevelPath, content) ->
        let state =
            match topLevelPath with
            | [] -> state
            | _ -> processOwnNamespace queryTrie topLevelPath state

        List.fold (processStateEntry queryTrie) state content

    | FileContentEntry.OpenStatement path ->
        // An open statement can directly reference file or be a partial open statement
        // Both cases need to be processed.
        let stateAfterFullOpenPath = processOpenPath queryTrie path state

        // Any existing open statement could be extended with the current path (if that node where to exists in the trie)
        // The extended path could add a new link (in case of a module or namespace with types)
        // It might also not add anything at all (in case it the extended path is still a partial one)
        (stateAfterFullOpenPath, state.OpenNamespaces)
        ||> Set.fold (fun acc openNS -> processOpenPath queryTrie [ yield! openNS; yield! path ] acc)

    | FileContentEntry.PrefixedIdentifier path ->
        match path with
        | [] ->
            // should not be possible though
            state
        | _ ->
            // path could consist out of multiple segments
            (state, [| 1 .. path.Length |])
            ||> Array.fold (fun state takeParts ->
                let path = List.take takeParts path
                // process the name was if it were a FQN
                let stateAfterFullIdentifier = processIdentifier queryTrie path state

                // Process the name in combination with the existing open namespaces
                (stateAfterFullIdentifier, state.OpenNamespaces)
                ||> Set.fold (fun acc openNS -> processIdentifier queryTrie [ yield! openNS; yield! path ] acc))

    | FileContentEntry.NestedModule (nestedContent = nestedContent) ->
        // We don't want our current state to be affect by any open statements in the nested module
        let nestedState = List.fold (processStateEntry queryTrie) state nestedContent
        // Afterward we are only interested in the found dependencies in the nested module
        let foundDependencies =
            Set.union state.FoundDependencies nestedState.FoundDependencies

        { state with
            FoundDependencies = foundDependencies
        }

let getFileNameBefore2 (files: FileInProject array) idx =
    files[0 .. (idx - 1)] |> Array.map (fun f -> f.Idx) |> Set.ofArray

/// Returns files contain in any node of the given Trie
let indicesUnderNode (node: TrieNode) : Set<FileIndex> =
    let rec collect (node: TrieNode) (continuation: FileIndex list -> FileIndex list) : FileIndex list =
        let continuations: ((FileIndex list -> FileIndex list) -> FileIndex list) list =
            [
                for node in node.Children.Values do
                    yield collect node
            ]

        let finalContinuation indexes =
            continuation [ yield! node.Files; yield! List.collect id indexes ]

        Continuation.sequence continuations finalContinuation

    Set.ofList (collect node id)

/// <summary>
/// For a given file's content, find all missing ("ghost") file dependencies that are required to satisfy the type-checker.
/// </summary>
/// <remarks>
/// A "ghost" dependency is a link between files that actually should be avoided.
/// The user has a partial namespace or opens a namespace that does not produce anything.
/// In order to still be able to compile the current file, the given namespace should be known to the file.
/// We did not find it via the trie, because there are no files that contribute to this namespace.
/// </remarks>
let collectGhostDependencies (fileIndex: FileIndex) (trie: TrieNode) (queryTrie: QueryTrie) (result: FileContentQueryState) =
    // Go over all open namespaces, and assert all those links eventually went anywhere
    Set.toArray result.OpenedNamespaces
    |> Array.collect (fun path ->
        match queryTrie path with
        | QueryTrieNodeResult.NodeExposesData _
        | QueryTrieNodeResult.NodeDoesNotExist -> Array.empty
        | QueryTrieNodeResult.NodeDoesNotExposeData ->
            // At this point we are following up if an open namespace really lead nowhere.
            let node =
                let rec visit (node: TrieNode) (path: LongIdentifier) =
                    match path with
                    | [] -> node
                    | head :: tail -> visit node.Children[head] tail

                visit trie path

            let children = indicesUnderNode node |> Set.filter (fun idx -> idx < fileIndex)
            let intersection = Set.intersect result.FoundDependencies children

            if Set.isEmpty intersection then
                // The partial open did not lead to anything
                // In order for it to exist in the current file we need to link it
                // to some file that introduces the namespace in the trie.
                if Set.isEmpty children then
                    // In this case not a single file is contributing to the opened namespace.
                    // As a last resort we assume all files are dependent, in order to preserve valid code.
                    [| 0 .. (fileIndex - 1) |]
                else
                    [| Seq.head children |]
            else
                // The partial open did eventually lead to a link in a file
                Array.empty)

let mkGraph (compilingFSharpCore: bool) (filePairs: FilePairMap) (files: FileInProject array) : Graph<FileIndex> =
    // Implementation files backed by signatures should be excluded to construct the trie.
    let trieInput =
        Array.choose
            (fun f ->
                match f.ParsedInput with
                | ParsedInput.SigFile _ -> Some f
                | ParsedInput.ImplFile _ -> if filePairs.HasSignature f.Idx then None else Some f)
            files

    let trie = TrieMapping.mkTrie trieInput
    let queryTrie: QueryTrie = queryTrieMemoized trie

    let fileContents = Array.Parallel.map FileContentMapping.mkFileContent files

    let findDependencies (file: FileInProject) : FileIndex * FileIndex array =
        let fileContent = fileContents[file.Idx]
        let knownFiles = set [ 0 .. (file.Idx - 1) ]
        let filesFromRoot = trie.Files |> Set.filter (fun rootIdx -> rootIdx < file.Idx)

        // Process all entries of a file and query the trie when required to find the dependent files.
        let result =
            // Seq is faster than List in this case.
            Seq.fold (processStateEntry queryTrie) (FileContentQueryState.Create file.Idx knownFiles filesFromRoot) fileContent

        // after processing the file we should verify if any of the open statements are found in the trie but do not yield any file link.
        let ghostDependencies = collectGhostDependencies file.Idx trie queryTrie result

        // Automatically add a link from an implementation to its signature file (if present)
        let signatureDependency =
            match filePairs.TryGetSignatureIndex file.Idx with
            | None -> Array.empty
            | Some sigIdx -> Array.singleton sigIdx

        // Files in FSharp.Core that occur between `prim-types-prelude` and `prim-types`
        // need to know about `prim-types-prelude` that came before it.
        let fsharpCoreBuildProperties =
            if not compilingFSharpCore then
                Array.empty
            else
                let idxOf (fileName: string) =
                    files |> Array.findIndex (fun f -> f.FileName.EndsWith fileName)

                let primTypesPreludeFsi = idxOf "prim-types-prelude.fsi"
                let primTypesFsiIdx = idxOf "prim-types.fsi"

                if primTypesPreludeFsi < file.Idx && file.Idx < primTypesFsiIdx then
                    [| primTypesPreludeFsi |]
                else
                    Array.empty

        let allDependencies =
            [|
                yield! result.FoundDependencies
                yield! ghostDependencies
                yield! signatureDependency
                yield! fsharpCoreBuildProperties
            |]
            |> Array.distinct

        file.Idx, allDependencies

    Array.Parallel.map findDependencies files |> readOnlyDict
