﻿module internal FSharp.Compiler.GraphChecking.TrieMapping

open System.Collections.Generic
open FSharp.Compiler.Syntax

[<RequireQualifiedAccess>]
module private HashSet =
    /// Create a new HashSet<'T> with a single element.
    let singleton value = HashSet(Seq.singleton value)
    /// Create new new HashSet<'T> with zero elements.
    let empty () = HashSet(Seq.empty)

let autoOpenShapes =
    set
        [|
            "FSharp.Core.AutoOpenAttribute"
            "Core.AutoOpenAttribute"
            "AutoOpenAttribute"
            "FSharp.Core.AutoOpen"
            "Core.AutoOpen"
            "AutoOpen"
        |]

/// This isn't bullet proof, we do prompt a warning when the user is aliasing the AutoOpenAttribute.
let isAutoOpenAttribute (attribute: SynAttribute) =
    match attribute.ArgExpr with
    | SynExpr.Const(constant = SynConst.Unit)
    | SynExpr.Const(constant = SynConst.String _)
    | SynExpr.Paren(expr = SynExpr.Const(constant = SynConst.String _)) ->
        let attributeName =
            attribute.TypeName.LongIdent
            |> List.map (fun ident -> ident.idText)
            |> String.concat "."

        autoOpenShapes.Contains attributeName
    | _ -> false

let isAnyAttributeAutoOpen (attributes: SynAttributes) =
    attributes
    |> List.exists (fun (atl: SynAttributeList) -> List.exists isAutoOpenAttribute atl.Attributes)

/// Checks to see if the top level ModuleOrNamespace exposes content that could be inferred by any of the subsequent files.
/// This can happen when a `namespace global` is used, or when a module (with a single ident name) has the `[<AutoOpen>]` attribute.
let doesFileExposeContentToTheRoot (ast: ParsedInput) : bool =
    match ast with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = contents)) ->
        contents
        |> List.exists (fun (SynModuleOrNamespaceSig (attribs = attribs; longId = longId; kind = kind)) ->
            (isAnyAttributeAutoOpen attribs && longId.Length < 2)
            || kind = SynModuleOrNamespaceKind.GlobalNamespace)

    | ParsedInput.ImplFile (ParsedImplFileInput (contents = contents)) ->
        contents
        |> List.exists (fun (SynModuleOrNamespace (attribs = attribs; longId = longId; kind = kind)) ->
            (isAnyAttributeAutoOpen attribs && longId.Length < 2)
            || kind = SynModuleOrNamespaceKind.GlobalNamespace)

let mergeTrieNodes (defaultChildSize: int) (tries: TrieNode array) =
    let rec mergeTrieNodesAux (root: TrieNode) (KeyValue (k, v)) =
        if root.Children.ContainsKey k then
            let node = root.Children[k]

            match node.Current, v.Current with
            | TrieNodeInfo.Namespace (filesThatExposeTypes = currentFiles), TrieNodeInfo.Namespace (filesThatExposeTypes = otherFiles) ->
                for otherFile in otherFiles do
                    currentFiles.Add(otherFile) |> ignore
            | _ -> ()

            for kv in v.Children do
                mergeTrieNodesAux node kv

        else
            root.Children.Add(k, v)

    match Array.tryExactlyOne tries with
    | Some ({ Current = TrieNodeInfo.Root _ } as singleTrie) -> singleTrie
    | _ ->
        let rootFiles = HashSet.empty ()

        let root =
            {
                Current = TrieNodeInfo.Root rootFiles
                Children = Dictionary<_, _>(defaultChildSize)
            }

        for trie in tries do
            for rootIndex in trie.Files do
                rootFiles.Add rootIndex |> ignore

            match trie.Current with
            | TrieNodeInfo.Root _ -> ()
            | current -> System.Diagnostics.Debug.Assert(false, $"The top level node info of a trie should be Root, got {current}")

            for kv in trie.Children do
                mergeTrieNodesAux root kv

        root

let mkDictFromKeyValuePairs (items: KeyValuePair<'TKey, 'TValue> list) =
    let dict = Dictionary(Seq.length items)

    for KeyValue (k, v) in items do
        if not (dict.ContainsKey(k)) then
            dict.Add(k, v)

    dict

let mkSingletonDict key value =
    let dict = Dictionary(1)
    dict.Add(key, value)
    dict

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
            | [] -> failwith "should not be empty"
            | [ finalPart ] ->
                let name = finalPart.idText

                // A module always exposes the file index, as it could expose values and functions.
                // A namespace only exposes the file when it has types or nested modules with an [<AutoOpen>] attribute.
                // The reasoning is that a type could be inferred and a nested auto open module will lift its content one level up.
                let current =
                    if isNamespace then
                        TrieNodeInfo.Namespace(
                            name,
                            (if hasTypesOrAutoOpenNestedModules then
                                 HashSet.singleton idx
                             else
                                 HashSet.empty ())
                        )
                    else
                        TrieNodeInfo.Module(name, idx)

                let children =
                    List.choose (mkTrieForDeclaration idx) decls |> mkDictFromKeyValuePairs

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
                        let files =
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
                                    HashSet.singleton idx
                                else
                                    HashSet.empty ()
                            | _ -> HashSet.empty ()

                        let current = TrieNodeInfo.Namespace(name, files)

                        mkSingletonDict name { Current = current; Children = node } |> continuation)
                    tail

        if kind = SynModuleOrNamespaceKind.AnonModule then
            // We collect the child nodes from the decls
            decls |> List.choose (mkTrieForDeclaration idx) |> mkDictFromKeyValuePairs
        else
            visit id name

    {
        Current = Root(HashSet.empty ())
        Children = children
    }

let rec mkTrieNodeFor (file: FileInProject) : TrieNode =
    let idx = file.Idx

    if doesFileExposeContentToTheRoot file.ParsedInput then
        // If a file exposes content which does not need an open statement to access, we consider the file to be part of the root.
        {
            Current = Root(HashSet.singleton idx)
            Children = Dictionary(0)
        }
    else
        match file.ParsedInput with
        | ParsedInput.SigFile (ParsedSigFileInput (contents = contents)) ->
            contents
            |> List.map
                (fun (SynModuleOrNamespaceSig (longId = longId
                                               kind = kind
                                               attribs = attribs
                                               decls = decls
                                               accessibility = _accessibility)) ->
                    let hasTypesOrAutoOpenNestedModules =
                        decls
                        |> List.exists (function
                            | SynModuleSigDecl.Types _ -> true
                            | SynModuleSigDecl.NestedModule(moduleInfo = SynComponentInfo (attributes = attributes)) ->
                                isAnyAttributeAutoOpen attributes
                            | _ -> false)

                    processSynModuleOrNamespace mkTrieForSynModuleSigDecl idx longId attribs kind hasTypesOrAutoOpenNestedModules decls)
            |> List.toArray
            |> mergeTrieNodes contents.Length
        | ParsedInput.ImplFile (ParsedImplFileInput (contents = contents)) ->
            contents
            |> List.map
                (fun (SynModuleOrNamespace (longId = longId; attribs = attribs; kind = kind; decls = decls; accessibility = _accessibility)) ->
                    let hasTypesOrAutoOpenNestedModules =
                        List.exists
                            (function
                            | SynModuleDecl.Types _ -> true
                            | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo (attributes = attributes)) ->
                                isAnyAttributeAutoOpen attributes
                            | _ -> false)
                            decls

                    processSynModuleOrNamespace mkTrieForSynModuleDecl idx longId attribs kind hasTypesOrAutoOpenNestedModules decls)
            |> List.toArray
            |> mergeTrieNodes contents.Length

and mkTrieForSynModuleDecl (fileIndex: FileIndex) (decl: SynModuleDecl) : KeyValuePair<string, TrieNode> option =
    match decl with
    | SynModuleDecl.NestedModule (moduleInfo = SynComponentInfo(longId = [ nestedModuleIdent ]); decls = decls) ->
        let name = nestedModuleIdent.idText

        let children =
            decls
            |> List.choose (mkTrieForSynModuleDecl fileIndex)
            |> mkDictFromKeyValuePairs

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
    | SynModuleSigDecl.NestedModule (moduleInfo = SynComponentInfo(longId = [ nestedModuleIdent ]); moduleDecls = decls) ->
        let name = nestedModuleIdent.idText

        let children =
            decls
            |> List.choose (mkTrieForSynModuleSigDecl fileIndex)
            |> mkDictFromKeyValuePairs

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

let mkTrie (files: FileInProject array) : TrieNode =
    mergeTrieNodes 0 (files |> Array.Parallel.map mkTrieNodeFor)
