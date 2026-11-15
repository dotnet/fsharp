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
        // Multiple files define the same module name in the same namespace.
        // Promote to a Namespace node so both file indices are preserved as dependencies.
        | TrieNodeInfo.Module(name, file1), TrieNodeInfo.Module(_, file2) ->
            TrieNodeInfo.Namespace(name, ImmutableHashSet.singleton file1 |> _.Add(file2), ImmutableHashSet.empty ())
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
    | SynModuleDecl.NestedModule(moduleInfo = compInfo; decls = decls) when compInfo.LongIdent.Length = 1 ->
        let nestedModuleIdent = compInfo.LongIdent.Head
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
    | SynModuleSigDecl.NestedModule(moduleInfo = compInfo; moduleDecls = decls) when compInfo.LongIdent.Length = 1 ->
        let nestedModuleIdent = compInfo.LongIdent.Head
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

// The names a declaration defines, beyond the modules and namespaces the trie holds: for dependencies between
// declarations rather than files.

/// The cases of an active pattern's name such as `|A|B|_|`.
let private activePatternCases (name: string) =
    if name.StartsWith "|" then
        name.Split('|') |> Array.filter (fun case -> case <> "" && case <> "_") |> List.ofArray
    else
        []

let private identNames (id: Ident) = id.idText :: activePatternCases id.idText

let rec private patternNames (pat: SynPat) : string list =
    match pat with
    | SynPat.Named(ident = SynIdent(id, _)) -> identNames id
    | SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = argPats) ->
        match List.tryLast ids with
        | None -> []
        | Some id ->
            // `F x` is a function F or a union case pattern binding x: which one depends on the environment, so an
            // uppercase head with arguments defines both.
            let args =
                if not argPats.Patterns.IsEmpty && System.Char.IsUpper id.idText[0] then
                    List.collect patternNames argPats.Patterns
                else
                    []

            identNames id @ args
    | SynPat.Typed(pat = pat)
    | SynPat.Paren(pat = pat)
    | SynPat.Attrib(pat = pat) -> patternNames pat
    | SynPat.As(lhsPat, rhsPat, _)
    | SynPat.Or(lhsPat, rhsPat, _, _)
    | SynPat.ListCons(lhsPat, rhsPat, _, _) -> patternNames lhsPat @ patternNames rhsPat
    | SynPat.Tuple(elementPats = pats)
    | SynPat.ArrayOrList(elementPats = pats)
    | SynPat.Ands(pats = pats) -> List.collect patternNames pats
    | SynPat.Record(fieldPats = fields) -> fields |> List.collect (fun field -> patternNames field.Pattern)
    | SynPat.OptionalVal(ident = id) -> [ id.idText ]
    | SynPat.Const _
    | SynPat.Wild _
    | SynPat.Null _
    | SynPat.IsInst _
    | SynPat.QuoteExpr _
    | SynPat.InstanceMember _
    | SynPat.FromParseError _ -> []

/// The operation names of `[<CustomOperation(...)>]` attributes, and whether one is not a string literal.
let private customOperationNames (attributes: SynAttributes) =
    let rec literal (expr: SynExpr) =
        match expr with
        | SynExpr.Paren(expr = expr) -> literal expr
        | SynExpr.Tuple(exprs = first :: _) -> literal first
        | SynExpr.Const(SynConst.String(text = text), _) -> Some text
        | _ -> None

    let operations =
        [
            for list in attributes do
                for attr in list.Attributes do
                    match List.tryLast attr.TypeName.LongIdent with
                    | Some id when id.idText = "CustomOperation" || id.idText = "CustomOperationAttribute" -> literal attr.ArgExpr
                    | _ -> ()
        ]

    List.choose id operations, List.exists Option.isNone operations

let private ofAll (results: (string list * bool) list) =
    List.collect fst results, List.exists snd results

let private bindingNames (SynBinding(attributes = attributes; headPat = headPat)) =
    let operations, unreadable = customOperationNames attributes

    let names =
        match headPat with
        // A member: `this.M`, or `new`
        | SynPat.LongIdent(longDotId = SynLongIdent(id = ids)) when ids.Length > 1 ->
            match List.tryLast ids with
            | Some id when id.idText <> "new" -> identNames id
            | _ -> []
        | SynPat.LongIdent(longDotId = SynLongIdent(id = [ id ])) when id.idText = "new" -> []
        | pat -> patternNames pat

    names @ operations, unreadable

let rec private memberNames (memberDefn: SynMemberDefn) : string list * bool =
    match memberDefn with
    | SynMemberDefn.Member(memberDefn = binding) -> bindingNames binding
    | SynMemberDefn.GetSetMember(get, set, _, _) -> Option.toList get @ Option.toList set |> List.map bindingNames |> ofAll
    | SynMemberDefn.AbstractSlot(slotSig = SynValSig(ident = SynIdent(id, _))) -> [ id.idText ], false
    | SynMemberDefn.AutoProperty(ident = id) -> [ id.idText ], false
    | SynMemberDefn.ValField(fieldInfo = SynField(idOpt = Some id)) -> [ id.idText ], false
    | SynMemberDefn.Interface(members = Some members) -> members |> List.map memberNames |> ofAll
    | SynMemberDefn.ValField _
    | SynMemberDefn.Interface _
    | SynMemberDefn.Open _
    | SynMemberDefn.ImplicitCtor _
    | SynMemberDefn.ImplicitInherit _
    | SynMemberDefn.LetBindings _
    | SynMemberDefn.Inherit _
    | SynMemberDefn.NestedType _ -> [], false

let private typeDefnNames (SynTypeDefn(typeInfo = typeInfo; typeRepr = typeRepr; members = members)) =
    let allMembers =
        match typeRepr with
        | SynTypeDefnRepr.ObjectModel(members = reprMembers) -> reprMembers @ members
        | _ -> members

    let memberNames, unreadable = allMembers |> List.map memberNames |> ofAll

    let own =
        match typeRepr with
        // `type T with ...` adds members to T, not T itself
        | SynTypeDefnRepr.ObjectModel(kind = SynTypeDefnKind.Augmentation _) -> []
        | _ -> typeInfo.LongIdent |> List.tryLast |> Option.map _.idText |> Option.toList

    let reprNames =
        match typeRepr with
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Union(unionCases = cases), _) ->
            [ for SynUnionCase(ident = SynIdent(id, _)) in cases -> id.idText ]
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Enum(cases = cases), _) ->
            [ for SynEnumCase(ident = SynIdent(id, _)) in cases -> id.idText ]
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFieldsAndSpreads = fields), _) ->
            [
                for field in fields do
                    match field with
                    | SynFieldOrSpread.Field(SynField(idOpt = Some id)) -> yield id.idText
                    | _ -> ()
            ]
        | _ -> []

    own @ reprNames @ memberNames, unreadable

/// The names a declaration of an implementation file defines: values, types, union cases, record fields, members,
/// custom operation names, modules, namespaces, and every such name inside a nested module or namespace fragment.
/// The flag says whether a custom operation's name is not a string literal, so a name may be missing.
let rec declarationNames (decl: SynModuleDecl) : string list * bool =
    match decl with
    | SynModuleDecl.Let(bindings = bindings) -> bindings |> List.map bindingNames |> ofAll
    | SynModuleDecl.Types(typeDefns = typeDefns) -> typeDefns |> List.map typeDefnNames |> ofAll
    | SynModuleDecl.Exception(exnDefn = SynExceptionDefn(SynExceptionDefnRepr(caseName = SynUnionCase(ident = SynIdent(id, _))), _, members, _)) ->
        let names, unreadable = members |> List.map memberNames |> ofAll
        id.idText :: names, unreadable
    | SynModuleDecl.NestedModule(moduleInfo = moduleInfo; decls = decls) ->
        let names, unreadable = decls |> List.map declarationNames |> ofAll
        (moduleInfo.LongIdent |> List.map _.idText) @ names, unreadable
    | SynModuleDecl.NamespaceFragment(SynModuleOrNamespace(longId = longId; decls = decls)) ->
        let names, unreadable = decls |> List.map declarationNames |> ofAll
        (longId |> List.map _.idText) @ names, unreadable
    | SynModuleDecl.ModuleAbbrev(ident = id) -> [ id.idText ], false
    | SynModuleDecl.Open _
    | SynModuleDecl.Attributes _
    | SynModuleDecl.Expr _
    | SynModuleDecl.HashDirective _ -> [], false
