module internal FSharp.Compiler.GraphChecking.DependencyResolution

open FSharp.Compiler.Syntax

/// <summary>Find a path from a starting TrieNode and return the end node or None</summary>
let queryTriePartial (trie: TrieNode) (path: LongIdentifier) : TrieNode option =
    let rec visit (currentNode: TrieNode) (path: LongIdentifier) =
        match path with
        // When we get through all partial identifiers, we've reached the node the full path points to.
        | [] -> Some currentNode
        // More segments to get through
        | currentPath :: restPath ->
            match currentNode.Children.TryGetValue(currentPath) with
            | false, _ -> None
            | true, childNode -> visit childNode restPath

    visit trie path

let mapNodeToQueryResult (node: TrieNode option) : QueryTrieNodeResult =
    match node with
    | Some finalNode ->
        if Set.isEmpty finalNode.Files then
            QueryTrieNodeResult.NodeDoesNotExposeData
        else
            QueryTrieNodeResult.NodeExposesData(finalNode.Files)
    | None -> QueryTrieNodeResult.NodeDoesNotExist

/// <summary>Find a path in the Trie.</summary>
let queryTrie (trie: TrieNode) (path: LongIdentifier) : QueryTrieNodeResult =
    queryTriePartial trie path |> mapNodeToQueryResult

/// <summary>Same as 'queryTrie' but allows passing in a path combined from two parts, avoiding list allocation.</summary>
let queryTrieDual (trie: TrieNode) (path1: LongIdentifier) (path2: LongIdentifier) : QueryTrieNodeResult =
    match queryTriePartial trie path1 with
    | Some intermediateNode -> queryTriePartial intermediateNode path2
    | None -> None
    |> mapNodeToQueryResult

/// Process namespace declaration.
let processNamespaceDeclaration (trie: TrieNode) (path: LongIdentifier) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie trie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData -> state.AddOwnNamespace path
    | QueryTrieNodeResult.NodeExposesData files -> state.AddOwnNamespace(path, files)

/// Process an "open" statement.
/// The statement could link to files and/or should be tracked as an open namespace.
let processOpenPath (trie: TrieNode) (path: LongIdentifier) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie trie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData -> state.AddOpenNamespace path
    | QueryTrieNodeResult.NodeExposesData files -> state.AddOpenNamespace(path, files)

/// Process an identifier.
let processIdentifier (queryResult: QueryTrieNodeResult) (state: FileContentQueryState) : FileContentQueryState =
    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData ->
        // This can occur when you have a file that uses a known namespace (for example namespace System).
        // When any other code uses that System namespace it won't find anything in the user code.
        state
    | QueryTrieNodeResult.NodeExposesData files -> state.AddDependencies files

/// Typically used to fold FileContentEntry items over a FileContentQueryState
let rec processStateEntry (trie: TrieNode) (state: FileContentQueryState) (entry: FileContentEntry) : FileContentQueryState =
    match entry with
    | FileContentEntry.TopLevelNamespace(topLevelPath, content) ->
        let state =
            match topLevelPath with
            | [] -> state
            | _ -> processNamespaceDeclaration trie topLevelPath state

        List.fold (processStateEntry trie) state content

    | FileContentEntry.OpenStatement path ->
        // An open statement can directly reference file or be a partial open statement
        // Both cases need to be processed.
        let stateAfterFullOpenPath = processOpenPath trie path state

        // Any existing open statement could be extended with the current path (if that node were to exists in the trie)
        // The extended path could add a new link (in case of a module or namespace with types)
        // It might also not add anything at all (in case the extended path is still a partial one)
        (stateAfterFullOpenPath, state.OpenNamespaces)
        ||> Set.fold (fun acc openNS -> processOpenPath trie [ yield! openNS; yield! path ] acc)

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
                let stateAfterFullIdentifier = processIdentifier (queryTrieDual trie [] path) state

                // Process the name in combination with the existing open namespaces
                (stateAfterFullIdentifier, state.OpenNamespaces)
                ||> Set.fold (fun acc openNS ->
                    let queryResult = queryTrieDual trie openNS path
                    processIdentifier queryResult acc))

    | FileContentEntry.NestedModule(nestedContent = nestedContent) ->
        // We don't want our current state to be affect by any open statements in the nested module
        let nestedState = List.fold (processStateEntry trie) state nestedContent
        // Afterward we are only interested in the found dependencies in the nested module
        let foundDependencies =
            Set.union state.FoundDependencies nestedState.FoundDependencies

        { state with
            FoundDependencies = foundDependencies
        }

    | FileContentEntry.ModuleName name ->
        // We need to check if the module name is a hit in the Trie.
        let state' =
            let queryResult = queryTrie trie [ name ]
            processIdentifier queryResult state

        match state.OwnNamespace with
        | None -> state'
        | Some ns ->
            // If there we currently have our own namespace,
            // the combination of that namespace + module name should be checked as well.
            let queryResult = queryTrieDual trie ns [ name ]
            processIdentifier queryResult state'

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
/// This function returns an array with a potential extra dependencies that makes sure that any such namespaces can be resolved (if they exists).
/// For each unused namespace `open` we return at most one file that defines that namespace.</para>
/// </remarks>
let collectGhostDependencies (fileIndex: FileIndex) (trie: TrieNode) (result: FileContentQueryState) =
    // For each opened namespace, if none of already resolved dependencies define it, return the top-most file that defines it.
    Set.toArray result.OpenedNamespaces
    |> Array.choose (fun path ->
        match queryTrie trie path with
        | QueryTrieNodeResult.NodeExposesData _
        | QueryTrieNodeResult.NodeDoesNotExist -> None
        | QueryTrieNodeResult.NodeDoesNotExposeData ->
            // At this point we are following up if an open namespace really lead nowhere.
            let node =
                let rec find (node: TrieNode) (path: LongIdentifier) =
                    match path with
                    | [] -> node
                    | head :: tail -> find node.Children[head] tail

                find trie path

            match node.Current with
            // Both Root and module would expose data, so we can ignore them.
            | Root _
            | Module _ -> None
            | Namespace(filesDefiningNamespaceWithoutTypes = filesDefiningNamespaceWithoutTypes) ->
                if filesDefiningNamespaceWithoutTypes.Overlaps(result.FoundDependencies) then
                    // The ghost dependency is already covered by a real dependency.
                    None
                else
                    // We are only interested in any file that contained the namespace when they came before the current file.
                    // If the namespace is defined in a file after the current file then there is no way the current file can reference it.
                    // Which means that namespace would come from a different assembly.
                    filesDefiningNamespaceWithoutTypes
                    |> Seq.sort
                    |> Seq.tryFind (fun connectedFileIdx ->
                        // We pick the lowest file index from the namespace to satisfy the type-checker for the open statement.
                        connectedFileIdx < fileIndex))

let mkGraph (filePairs: FilePairMap) (files: FileInProject array) : Graph<FileIndex> * TrieNode =
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

    let fileContents =
        files
        |> Array.Parallel.map (fun file ->
            if file.Idx = 0 then
                List.empty
            else
                FileContentMapping.mkFileContent file)

    let findDependencies (file: FileInProject) : FileIndex array =
        if file.Idx = 0 then
            // First file cannot have any dependencies.
            Array.empty
        else
            let fileContent = fileContents[file.Idx]

            // The Trie we want to use is the one that contains only files before our current index.
            // As we skip implementation files (backed by a signature), we cannot just use the current file index to find the right Trie.
            let trieForFile =
                trie
                |> Array.fold (fun acc (idx, t) -> if idx < file.Idx then t else acc) TrieNode.Empty

            // File depends on all files above it that define accessible symbols at the root level (global namespace).
            let filesFromRoot =
                trieForFile.Files |> Set.filter (fun rootIdx -> rootIdx < file.Idx)
            // Start by listing root-level dependencies.
            let initialDepsResult = (FileContentQueryState.Create filesFromRoot), fileContent
            // Sequentially process all relevant entries of the file and keep updating the state and set of dependencies.
            let depsResult =
                initialDepsResult
                // Seq is faster than List in this case.
                ||> Seq.fold (processStateEntry trieForFile)

            // Add missing links for cases where an unused open namespace did not create a link.
            let ghostDependencies = collectGhostDependencies file.Idx trieForFile depsResult

            // Add a link from implementation files to their signature files.
            let signatureDependency =
                match filePairs.TryGetSignatureIndex file.Idx with
                | None -> Array.empty
                | Some sigIdx -> Array.singleton sigIdx

            let allDependencies =
                [|
                    yield! depsResult.FoundDependencies
                    yield! ghostDependencies
                    yield! signatureDependency
                |]
                |> Array.distinct

            allDependencies

    let graph =
        files
        |> Array.Parallel.map (fun file -> file.Idx, findDependencies file)
        |> readOnlyDict

    let trie = trie |> Array.last |> snd

    graph, trie
