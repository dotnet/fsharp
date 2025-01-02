module internal FSharp.Compiler.GraphChecking.TrieMapping

open System.Collections.Generic
open System.Collections.Immutable
open System.Text
open FSharp.Compiler.IO
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps

[<RequireQualifiedAccess>]
module private ImmutableHashSet =
    /// Create a new HashSet<'T> with a single element.
    let singleton (value: 'T) =
        ImmutableHashSet.Create<'T>(Array.singleton value)

    /// Create a new HashSet<'T> with zero elements.
    let empty () = ImmutableHashSet.Empty

let isAnyAttributeAutoOpen (attributes: SynAttributes) = findSynAttribute "AutoOpen" attributes

/// Checks to see if the top level ModuleOrNamespace exposes content that could be inferred by any of the subsequent files.
/// This can happen when a `namespace global` is used, or when a module (with a single ident name) has the `[<AutoOpen>]` attribute.
let doesFileExposeContentToTheRoot (ast: ParsedInput) : bool =
    match ast with
    | ParsedInput.SigFile(ParsedSigFileInput(contents = contents)) ->
        contents
        |> List.exists (fun (SynModuleOrNamespaceSig(attribs = attribs; longId = longId; kind = kind)) ->
            (isAnyAttributeAutoOpen attribs && longId.Length < 2)
            || kind = SynModuleOrNamespaceKind.GlobalNamespace)

    | ParsedInput.ImplFile(ParsedImplFileInput(contents = contents)) ->
        contents
        |> List.exists (fun (SynModuleOrNamespace(attribs = attribs; longId = longId; kind = kind)) ->
            (isAnyAttributeAutoOpen attribs && longId.Length < 2)
            || kind = SynModuleOrNamespaceKind.GlobalNamespace)

/// Merge all the accumulator Trie nodes into the current Trie node.
let rec mergeTrieNodes (accumulatorTrie: TrieNode) (currentTrie: TrieNode) : TrieNode =
    let nextNodeInfo: TrieNodeInfo =
        match accumulatorTrie.Current, currentTrie.Current with
        | TrieNodeInfo.Root accFiles, TrieNodeInfo.Root currentFiles -> TrieNodeInfo.Root(accFiles.Union currentFiles)
        | TrieNodeInfo.Namespace(
            name = name; filesThatExposeTypes = currentFilesThatExposeTypes; filesDefiningNamespaceWithoutTypes = currentFilesWithoutTypes),
          TrieNodeInfo.Namespace(filesThatExposeTypes = otherFiles; filesDefiningNamespaceWithoutTypes = otherFilesWithoutTypes) ->
            TrieNodeInfo.Namespace(
                name,
                currentFilesThatExposeTypes.Union otherFiles,
                currentFilesWithoutTypes.Union otherFilesWithoutTypes
            )
        // Edge case scenario detected in https://github.com/dotnet/fsharp/issues/15985
        // Keep the namespace (as it can still have nested children).
        | TrieNodeInfo.Namespace(name, currentFilesThatExposeTypes, filesDefiningNamespaceWithoutTypes), TrieNodeInfo.Module(_name, file)
        // Replace the module in favour of the namespace (which can hold nested children).
        | TrieNodeInfo.Module(_name, file), TrieNodeInfo.Namespace(name, currentFilesThatExposeTypes, filesDefiningNamespaceWithoutTypes) ->
            TrieNodeInfo.Namespace(name, currentFilesThatExposeTypes.Add file, filesDefiningNamespaceWithoutTypes)
        | _ -> accumulatorTrie.Current

    let nextChildren =
        (accumulatorTrie.Children, currentTrie.Children)
        ||> Seq.fold (fun accChildren (KeyValue(k, v)) ->
            match accChildren.TryGetValue k with
            | false, _ -> accChildren.Add(k, v)
            | true, kVal ->
                let accNode = kVal
                accChildren.SetItem(k, mergeTrieNodes accNode v))

    {
        Current = nextNodeInfo
        Children = nextChildren
    }

let mkImmutableDictFromKeyValuePairs (items: KeyValuePair<'TKey, 'TValue> list) = ImmutableDictionary.CreateRange(items)

let mkSingletonDict key value =
    ImmutableDictionary.Empty.Add(key, value)

/// Process a top level SynModuleOrNamespace(Sig)
let processSynModuleOrNamespace<'Decl>
    (mkTrieForDeclaration: FileIndex -> 'Decl -> KeyValuePair<string, TrieNode> option)
    (idx: FileIndex)
    (name: LongIdent)
    (attributes: SynAttributes)
    (kind: SynModuleOrNamespaceKind)
    (hasTypesOrAutoOpenNestedModules: bool)
    (decls: 'Decl list)
    : TrieNode =
    let isNamespace =
        match kind with
        | SynModuleOrNamespaceKind.AnonModule
        | SynModuleOrNamespaceKind.NamedModule -> false
        | SynModuleOrNamespaceKind.DeclaredNamespace
        | SynModuleOrNamespaceKind.GlobalNamespace -> true

    let children =
        // Process the name of the ModuleOrNamespace.
        // For each part in the name a TrieNode shall be created.
        // Only the last node can be a module, depending on the SynModuleOrNamespaceKind.
        let rec visit continuation (xs: LongIdent) =
            match xs with
            | [] -> ImmutableDictionary.Empty |> continuation
            | [ finalPart ] ->
                let name = finalPart.idText

                // A module always exposes the file index, as it could expose values and functions.
                // A namespace only exposes the file when it has types or nested modules with an [<AutoOpen>] attribute.
                // The reasoning is that a type could be inferred and a nested auto open module will lift its content one level up.
                let current =
                    if isNamespace then
                        let filesThatExposeTypes, filesDefiningNamespaceWithoutTypes =
                            if hasTypesOrAutoOpenNestedModules then
                                ImmutableHashSet.singleton idx, ImmutableHashSet.empty ()
                            else
                                ImmutableHashSet.empty (), ImmutableHashSet.singleton idx

                        TrieNodeInfo.Namespace(name, filesThatExposeTypes, filesDefiningNamespaceWithoutTypes)
                    else
                        TrieNodeInfo.Module(name, idx)

                let children =
                    List.choose (mkTrieForDeclaration idx) decls |> mkImmutableDictFromKeyValuePairs

                mkSingletonDict
                    name
                    {
                        Current = current
                        Children = children
                    }
                |> continuation
            | head :: tail ->
                let name = head.idText

                visit
                    (fun node ->
                        let filesThatExposeTypes, filesDefiningNamespaceWithoutTypes =
                            match tail with
                            | [ _ ] ->
                                // In case you have:
                                // [<AutoOpen>]
                                // module A.B
                                //
                                // We should consider the namespace A to expose the current file.
                                // Due to the [<AutoOpen>] we treat A the same way we would module B.
                                let topLevelModuleOrNamespaceHasAutoOpen = isAnyAttributeAutoOpen attributes

                                if topLevelModuleOrNamespaceHasAutoOpen && not isNamespace then
                                    ImmutableHashSet.singleton idx, ImmutableHashSet.empty ()
                                else
                                    ImmutableHashSet.empty (), ImmutableHashSet.singleton idx
                            | _ -> ImmutableHashSet.empty (), ImmutableHashSet.singleton idx

                        let current =
                            TrieNodeInfo.Namespace(name, filesThatExposeTypes, filesDefiningNamespaceWithoutTypes)

                        mkSingletonDict name { Current = current; Children = node } |> continuation)
                    tail

        if kind = SynModuleOrNamespaceKind.AnonModule then
            // We collect the child nodes from the decls
            decls
            |> List.choose (mkTrieForDeclaration idx)
            |> mkImmutableDictFromKeyValuePairs
        else
            visit id name

    {
        Current = Root(ImmutableHashSet.empty ())
        Children = children
    }

let rec mkTrieNodeFor (file: FileInProject) : FileIndex * TrieNode =
    let idx = file.Idx

    if doesFileExposeContentToTheRoot file.ParsedInput then
        // If a file exposes content which does not need an open statement to access, we consider the file to be part of the root.
        idx,
        {
            Current = Root(ImmutableHashSet.singleton idx)
            Children = ImmutableDictionary.Empty
        }
    else
        let trie =
            match file.ParsedInput with
            | ParsedInput.SigFile(ParsedSigFileInput(contents = contents)) ->
                contents
                |> List.map
                    (fun
                        (SynModuleOrNamespaceSig(
                            longId = longId; kind = kind; attribs = attribs; decls = decls; accessibility = _accessibility)) ->
                        let hasTypesOrAutoOpenNestedModules =
                            decls
                            |> List.exists (function
                                | SynModuleSigDecl.Types _
                                | SynModuleSigDecl.Exception _ -> true
                                | SynModuleSigDecl.NestedModule(moduleInfo = SynComponentInfo(attributes = attributes)) ->
                                    isAnyAttributeAutoOpen attributes
                                | _ -> false)

                        processSynModuleOrNamespace mkTrieForSynModuleSigDecl idx longId attribs kind hasTypesOrAutoOpenNestedModules decls)
                |> List.reduce mergeTrieNodes
            | ParsedInput.ImplFile(ParsedImplFileInput(contents = contents)) ->
                contents
                |> List.map
                    (fun
                        (SynModuleOrNamespace(longId = longId; attribs = attribs; kind = kind; decls = decls; accessibility = _accessibility)) ->
                        let hasTypesOrAutoOpenNestedModules =
                            List.exists
                                (function
                                | SynModuleDecl.Types _
                                | SynModuleDecl.Exception _ -> true
                                | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(attributes = attributes)) ->
                                    isAnyAttributeAutoOpen attributes
                                | _ -> false)
                                decls

                        processSynModuleOrNamespace mkTrieForSynModuleDecl idx longId attribs kind hasTypesOrAutoOpenNestedModules decls)
                |> List.reduce mergeTrieNodes

        idx, trie

and mkTrieForSynModuleDecl (fileIndex: FileIndex) (decl: SynModuleDecl) : KeyValuePair<string, TrieNode> option =
    match decl with
    | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = [ nestedModuleIdent ]); decls = decls) ->
        let name = nestedModuleIdent.idText

        let children =
            decls
            |> List.choose (mkTrieForSynModuleDecl fileIndex)
            |> mkImmutableDictFromKeyValuePairs

        Some(
            KeyValuePair(
                name,
                {
                    Current = TrieNodeInfo.Module(name, fileIndex)
                    Children = children
                }
            )
        )
    | _ -> None

and mkTrieForSynModuleSigDecl (fileIndex: FileIndex) (decl: SynModuleSigDecl) : KeyValuePair<string, TrieNode> option =
    match decl with
    | SynModuleSigDecl.NestedModule(moduleInfo = SynComponentInfo(longId = [ nestedModuleIdent ]); moduleDecls = decls) ->
        let name = nestedModuleIdent.idText

        let children =
            decls
            |> List.choose (mkTrieForSynModuleSigDecl fileIndex)
            |> mkImmutableDictFromKeyValuePairs

        Some(
            KeyValuePair(
                name,
                {
                    Current = TrieNodeInfo.Module(name, fileIndex)
                    Children = children
                }
            )
        )
    | _ -> None

let mkTrie (files: FileInProject array) : (FileIndex * TrieNode) array =
    if files.Length = 1 then
        Array.singleton (mkTrieNodeFor files[0])
    else
        files
        |> Array.take (files.Length - 1) // Do not process the last file, it will never be looked up by anything anyway.
        |> Array.Parallel.map mkTrieNodeFor
        |> Array.scan
            (fun (_, acc) (idx, current) ->
                let next = mergeTrieNodes acc current
                idx, next)
            (System.Int32.MinValue, TrieNode.Empty)
        // We can ignore the initial state that was used in the scan
        |> Array.skip 1

type MermaidBoxPos =
    | First
    | Second

let serializeToMermaid (path: string) (filesInProject: FileInProject array) (trie: TrieNode) =
    let sb = StringBuilder()
    let appendLine (line: string) = sb.AppendLine(line) |> ignore
    let discovered = HashSet<TrieNodeInfo>()

    let getName (node: TrieNodeInfo) =
        match node with
        | Root _ -> "root"
        | Module(name, _) -> $"mod_{name}"
        | Namespace(name, _, _) -> $"ns_{name}"

    let toBoxList (boxPos: MermaidBoxPos) (files: ImmutableHashSet<FileIndex>) =
        let sb = StringBuilder()
        let orderedIndexes = Seq.sort files

        let opening, closing =
            match boxPos with
            | First -> "[", "]"
            | Second -> "(", ")"

        for file in orderedIndexes do
            let fileName = System.IO.Path.GetFileName(filesInProject[file].FileName)
            sb.Append($"    {fileName}{opening}{file}{closing}\n") |> ignore

        sb.ToString()

    let printNode (parent: TrieNode, node: TrieNode) =
        match node.Current with
        | TrieNodeInfo.Root files ->
            let firstBox = toBoxList First files

            if System.String.IsNullOrWhiteSpace firstBox then
                appendLine "class root\n"
            else
                appendLine $"class root {{\n{firstBox}}}\n"
        | TrieNodeInfo.Module(_name, file) as md ->
            let name = getName md
            let fileName = System.IO.Path.GetFileName(filesInProject[file].FileName)
            appendLine $"{getName parent.Current} <|-- {name}"
            appendLine $"class {name} {{\n    {fileName}[{file}]\n}}\n"
        | TrieNodeInfo.Namespace(_name, filesThatExposeTypes, filesDefiningNamespaceWithoutTypes) as ns ->
            let name = getName ns
            let firstBox = toBoxList First filesThatExposeTypes
            let secondBox = toBoxList Second filesDefiningNamespaceWithoutTypes
            appendLine $"{getName parent.Current} <|-- {name}"

            if
                System.String.IsNullOrWhiteSpace(firstBox)
                && System.String.IsNullOrWhiteSpace(secondBox)
            then
                appendLine $"class {name}"
            else
                appendLine $"class {name} {{\n{firstBox}\n{secondBox}}}\n"

    let rec traverse (v: TrieNode) =
        discovered.Add(v.Current) |> ignore

        for c in v.Children do
            if not (discovered.Contains(c.Value.Current)) then
                printNode (v, c.Value)
                traverse c.Value

    appendLine "```mermaid"
    appendLine "classDiagram\n"

    printNode (trie, trie)
    traverse trie

    appendLine "```"

    use out =
        FileSystem.OpenFileForWriteShim(path, fileMode = System.IO.FileMode.Create)

    out.WriteAllText(sb.ToString())
