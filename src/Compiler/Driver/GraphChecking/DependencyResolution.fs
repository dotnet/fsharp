module internal FSharp.Compiler.GraphChecking.DependencyResolution

open FSharp.Compiler.IO
open FSharp.Compiler.Syntax

/// <summary>Find a path in the Trie.</summary>
/// <remarks>This function could be cached in future if performance is an issue.</remarks>
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

/// Process namespace declaration.
let processNamespaceDeclaration (queryTrie: QueryTrie) (path: LongIdentifier) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData -> state.AddOwnNamespace path
    | QueryTrieNodeResult.NodeExposesData files -> state.AddOwnNamespace(path, files)

/// Process an "open" statement.
/// The statement could link to files and/or should be tracked as an open namespace.
let processOpenPath (queryTrie: QueryTrie) (path: LongIdentifier) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData -> state.AddOpenNamespace path
    | QueryTrieNodeResult.NodeExposesData files -> state.AddOpenNamespace(path, files)

/// Process an identifier.
let processIdentifier (queryTrie: QueryTrie) (path: LongIdentifier) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData ->
        // This can occur when you have a file that uses a known namespace (for example namespace System).
        // When any other code uses that System namespace it won't find anything in the user code.
        state
    | QueryTrieNodeResult.NodeExposesData files -> state.AddDependencies files

/// Typically used to fold FileContentEntry items over a FileContentQueryState
let rec processStateEntry (queryTrie: QueryTrie) (state: FileContentQueryState) (entry: FileContentEntry) : FileContentQueryState =
    match entry with
    | FileContentEntry.TopLevelNamespace (topLevelPath, content) ->
        let state =
            match topLevelPath with
            | [] -> state
            | _ -> processNamespaceDeclaration queryTrie topLevelPath state

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
            // path could consist of multiple segments
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

/// Return all files contained in the trie.
let filesInTrie (node: TrieNode) : Set<FileIndex> =
    let rec collect (node: TrieNode) (continuation: FileIndex list -> FileIndex list) : FileIndex list =
        let continuations: ((FileIndex list -> FileIndex list) -> FileIndex list) list =
            [
                for node in node.Children.Values do
                    yield collect node
            ]

        let finalContinuation indexes =
            continuation [ yield! node.Files; yield! List.concat indexes ]

        Continuation.sequence continuations finalContinuation

    Set.ofList (collect node id)

/// <summary>
/// For a given file's content, collect all missing ("ghost") file dependencies that the core resolution algorithm didn't return,
/// but are required to satisfy the type-checker.
/// </summary>
/// <remarks>
/// <para>Namespaces, contrary to modules, can and often are defined in multiple files.
/// When a [partial] namespace is opened, but unused, we want to avoid having to link to all the files that define it.</para>
/// <para>This is why, when:
/// - a file references a namespace, but does not explicitly reference anything within it, and
/// - the namespace does not contain any children that can be referenced implicitly (eg. by type inference),
/// then the main resolution algorithm does not create a link to any file defining the namespace.</para>
/// <para>However, to satisfy the type-checker, the namespace must be resolved.
/// This function returns a list of extra dependencies that makes sure that any such namespaces can be resolved (if it exists).
/// For each unused open namespace we return one or more file links that define it.</para>
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
                let rec find (node: TrieNode) (path: LongIdentifier) =
                    match path with
                    | [] -> node
                    | head :: tail -> find node.Children[head] tail

                find trie path

            let filesDefiningNamespace =
                filesInTrie node |> Set.filter (fun idx -> idx < fileIndex)

            let dependenciesDefiningNamespace =
                Set.intersect result.FoundDependencies filesDefiningNamespace

            [|
                if Set.isEmpty dependenciesDefiningNamespace then
                    // There is no existing dependency defining the namespace,
                    // so we need to add one.
                    if Set.isEmpty filesDefiningNamespace then
                        // No file defines inferrable symbols for this namespace, but the namespace might exist.
                        // Because we don't track what files define a namespace without any relevant content,
                        // the only way to ensure the namespace is in scope is to add a link to every preceding file.
                        yield! [| 0 .. (fileIndex - 1) |]
                    else
                        // At least one file defines the namespace - add a dependency to the first (top) one.
                        yield Seq.head filesDefiningNamespace
            |])

let mkGraph (compilingFSharpCore: bool) (filePairs: FilePairMap) (files: FileInProject array) : Graph<FileIndex> =
    // We know that implementation files backed by signatures cannot be depended upon.
    // Do not include them when building the Trie.
    let trieInput =
        files
        |> Array.choose (fun f ->
            match f.ParsedInput with
            | ParsedInput.ImplFile _ when filePairs.HasSignature f.Idx -> None
            | ParsedInput.ImplFile _
            | ParsedInput.SigFile _ -> Some f)

    let trie = TrieMapping.mkTrie trieInput
    let queryTrie: QueryTrie = queryTrieMemoized trie

    let fileContents = files |> Array.Parallel.map FileContentMapping.mkFileContent

    let findDependencies (file: FileInProject) : FileIndex array =
        let fileContent = fileContents[file.Idx]

        let knownFiles = [ 0 .. (file.Idx - 1) ] |> set
        // File depends on all files above it that define accessible symbols at the root level (global namespace).
        let filesFromRoot = trie.Files |> Set.filter (fun rootIdx -> rootIdx < file.Idx)
        // Start by listing root-level dependencies.
        let initialDepsResult =
            (FileContentQueryState.Create file.Idx knownFiles filesFromRoot), fileContent
        // Sequentially process all relevant entries of the file and keep updating the state and set of dependencies.
        let depsResult =
            initialDepsResult
            // Seq is faster than List in this case.
            ||> Seq.fold (processStateEntry queryTrie)

        // Add missing links for cases where an unused open namespace did not create a link.
        let ghostDependencies = collectGhostDependencies file.Idx trie queryTrie depsResult

        // Add a link from implementation files to their signature files.
        let signatureDependency =
            match filePairs.TryGetSignatureIndex file.Idx with
            | None -> Array.empty
            | Some sigIdx -> Array.singleton sigIdx

        // Files in FSharp.Core have an implicit dependency on `prim-types-prelude.fsi` - add it.
        let fsharpCoreImplicitDependencies =
            let filename = "prim-types-prelude.fsi"

            let implicitDepIdx =
                files
                |> Array.tryFindIndex (fun f -> FileSystemUtils.fileNameOfPath f.FileName = filename)

            [|
                if compilingFSharpCore then
                    match implicitDepIdx with
                    | Some idx ->
                        if file.Idx > idx then
                            yield idx
                    | None ->
                        exn $"Expected to find file '{filename}' during compilation of FSharp.Core, but it was not found."
                        |> raise
            |]

        let allDependencies =
            [|
                yield! depsResult.FoundDependencies
                yield! ghostDependencies
                yield! signatureDependency
                yield! fsharpCoreImplicitDependencies
            |]
            |> Array.distinct

        allDependencies

    files
    |> Array.Parallel.map (fun file -> file.Idx, findDependencies file)
    |> readOnlyDict
