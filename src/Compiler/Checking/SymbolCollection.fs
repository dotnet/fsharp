// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Symbol collection pre-pass ("Enter" phase) for order-independent compilation.
/// Scans all parsed files and collects top-level declarations into stub Entity shells
/// that are folded into TcEnv before type checking begins.
module internal FSharp.Compiler.SymbolCollection

open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.Import
open FSharp.Compiler.Xml
open FSharp.Compiler.GraphChecking

/// What we know about a type declaration from syntax alone
type TypeDeclStub =
    { Name: Ident
      Kind: SynTypeDefnKind
      TypeParamCount: int
      Accessibility: SynAccess option
      RecordFieldNames: Ident list
      UnionCaseNames: Ident list
      Range: range
      FileIndex: int }

/// What we know about a value/function declaration from syntax alone
type ValueDeclStub =
    { Name: Ident
      Accessibility: SynAccess option
      Range: range
      FileIndex: int }

/// What we know about a module from syntax alone
type ModuleDeclStub =
    { Name: Ident
      QualifiedName: Ident list
      Accessibility: SynAccess option
      IsAutoOpen: bool
      Kind: SynModuleOrNamespaceKind
      Types: TypeDeclStub list
      Values: ValueDeclStub list
      NestedModules: ModuleDeclStub list
      Range: range
      FileIndex: int }

/// The collected declarations for one file
type FileDeclarations =
    { FileIndex: int
      FileName: string
      QualifiedName: QualifiedNameOfFile
      TopLevelModules: ModuleDeclStub list
      Opens: LongIdent list
      IdentifierRefs: LongIdent list }

// ---------------------------------------------------------------
// AST walker: collectFileDeclarations
// ---------------------------------------------------------------

let private isAutoOpen (attribs: SynAttributes) =
    findSynAttribute "AutoOpen" attribs

/// Extract the name from a binding's head pattern.
/// For top-level let bindings, the pattern is typically SynPat.LongIdent or SynPat.Named.
let private tryGetBindingName (binding: SynBinding) =
    let (SynBinding(headPat = pat; accessibility = access)) = binding

    let rec loop pat =
        match pat with
        | SynPat.Named(ident = SynIdent(ident, _)) -> Some ident
        | SynPat.LongIdent(longDotId = SynLongIdent(id = ids)) ->
            // For function bindings like "let f x = ...", the head pat is LongIdent
            match ids with
            | [ name ] -> Some name
            | _ :: _ -> Some(List.last ids)
            | [] -> None
        | SynPat.Typed(pat = innerPat) -> loop innerPat
        | SynPat.Attrib(pat = innerPat) -> loop innerPat
        | SynPat.Paren(pat = innerPat) -> loop innerPat
        | _ -> None

    loop pat
    |> Option.map (fun name ->
        { Name = name
          Accessibility = access
          Range = name.idRange
          FileIndex = 0 })

/// Extract type declaration stubs from a SynTypeDefn
let private collectTypeDeclStub (fileIndex: int) (synTypeDefn: SynTypeDefn) : TypeDeclStub =
    let (SynTypeDefn(typeInfo = SynComponentInfo(typeParams = typarDecls; longId = ids; accessibility = access); typeRepr = repr)) =
        synTypeDefn

    let name =
        match ids with
        | [ id ] -> id
        | _ -> List.last ids

    let typeParamCount =
        match typarDecls with
        | Some(SynTyparDecls.PostfixList(decls = decls)) -> decls.Length
        | Some(SynTyparDecls.PrefixList(decls = decls)) -> decls.Length
        | Some(SynTyparDecls.SinglePrefix _) -> 1
        | None -> 0

    let kind =
        match repr with
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record _, _) -> SynTypeDefnKind.Record
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Union _, _) -> SynTypeDefnKind.Union
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Enum _, _) -> SynTypeDefnKind.Unspecified
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.TypeAbbrev _, _) -> SynTypeDefnKind.Abbrev
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.None _, _) -> SynTypeDefnKind.Opaque
        | SynTypeDefnRepr.ObjectModel(kind = k) -> k
        | SynTypeDefnRepr.Exception _ -> SynTypeDefnKind.Unspecified
        | SynTypeDefnRepr.Simple _ -> SynTypeDefnKind.Unspecified

    let recordFields =
        match repr with
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFields = fields), _) ->
            fields
            |> List.choose (fun (SynField(idOpt = idOpt)) -> idOpt)
        | _ -> []

    let unionCases =
        match repr with
        | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Union(unionCases = cases), _) ->
            cases
            |> List.map (fun (SynUnionCase(ident = SynIdent(ident, _))) -> ident)
        | _ -> []

    { Name = name
      Kind = kind
      TypeParamCount = typeParamCount
      Accessibility = access
      RecordFieldNames = recordFields
      UnionCaseNames = unionCases
      Range = name.idRange
      FileIndex = fileIndex }

/// Extract type declaration stubs from a SynTypeDefnSig (signature file)
let private collectTypeDeclStubFromSig (fileIndex: int) (synTypeDefnSig: SynTypeDefnSig) : TypeDeclStub =
    let (SynTypeDefnSig(typeInfo = SynComponentInfo(typeParams = typarDecls; longId = ids; accessibility = access); typeRepr = repr)) =
        synTypeDefnSig

    let name =
        match ids with
        | [ id ] -> id
        | _ -> List.last ids

    let typeParamCount =
        match typarDecls with
        | Some(SynTyparDecls.PostfixList(decls = decls)) -> decls.Length
        | Some(SynTyparDecls.PrefixList(decls = decls)) -> decls.Length
        | Some(SynTyparDecls.SinglePrefix _) -> 1
        | None -> 0

    let kind =
        match repr with
        | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.Record _, _) -> SynTypeDefnKind.Record
        | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.Union _, _) -> SynTypeDefnKind.Union
        | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.Enum _, _) -> SynTypeDefnKind.Unspecified
        | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.TypeAbbrev _, _) -> SynTypeDefnKind.Abbrev
        | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.None _, _) -> SynTypeDefnKind.Opaque
        | SynTypeDefnSigRepr.ObjectModel(kind = k) -> k
        | SynTypeDefnSigRepr.Exception _ -> SynTypeDefnKind.Unspecified
        | SynTypeDefnSigRepr.Simple _ -> SynTypeDefnKind.Unspecified

    let recordFields =
        match repr with
        | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFields = fields), _) ->
            fields
            |> List.choose (fun (SynField(idOpt = idOpt)) -> idOpt)
        | _ -> []

    let unionCases =
        match repr with
        | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.Union(unionCases = cases), _) ->
            cases
            |> List.map (fun (SynUnionCase(ident = SynIdent(ident, _))) -> ident)
        | _ -> []

    { Name = name
      Kind = kind
      TypeParamCount = typeParamCount
      Accessibility = access
      RecordFieldNames = recordFields
      UnionCaseNames = unionCases
      Range = name.idRange
      FileIndex = fileIndex }

/// Extract an open statement's path as a LongIdent
let private tryGetOpenPath (target: SynOpenDeclTarget) =
    match target with
    | SynOpenDeclTarget.ModuleOrNamespace(longId = SynLongIdent(id = ids)) -> Some ids
    | SynOpenDeclTarget.Type _ -> None

/// Collect declarations from implementation file module declarations
let rec private collectImplDecls (fileIndex: int) (parentPath: Ident list) (decls: SynModuleDecl list) =
    let mutable types = []
    let mutable values = []
    let mutable nestedModules = []
    let mutable opens = []

    for decl in decls do
        match decl with
        | SynModuleDecl.Types(typeDefns = typeDefs) ->
            for td in typeDefs do
                types <- collectTypeDeclStub fileIndex td :: types

        | SynModuleDecl.Let(bindings = bindings) ->
            for binding in bindings do
                match tryGetBindingName binding with
                | Some stub -> values <- { stub with FileIndex = fileIndex } :: values
                | None -> ()

        | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(attributes = attribs; longId = ids; accessibility = access); decls = nestedDecls; range = m) ->
            let name =
                match ids with
                | [ id ] -> id
                | _ -> List.last ids

            let qualName = parentPath @ [ name ]
            let innerTypes, innerValues, innerModules, innerOpens = collectImplDecls fileIndex qualName nestedDecls

            nestedModules <-
                { Name = name
                  QualifiedName = qualName
                  Accessibility = access
                  IsAutoOpen = isAutoOpen attribs
                  Kind = SynModuleOrNamespaceKind.NamedModule
                  Types = innerTypes
                  Values = innerValues
                  NestedModules = innerModules
                  Range = m
                  FileIndex = fileIndex }
                :: nestedModules

            opens <- innerOpens @ opens

        | SynModuleDecl.Open(target = target) ->
            match tryGetOpenPath target with
            | Some path -> opens <- path :: opens
            | None -> ()

        | SynModuleDecl.Exception(exnDefn = SynExceptionDefn(exnRepr = SynExceptionDefnRepr(caseName = SynUnionCase(ident = SynIdent(ident, _)); accessibility = access))) ->
            types <-
                { Name = ident
                  Kind = SynTypeDefnKind.Unspecified
                  TypeParamCount = 0
                  Accessibility = access
                  RecordFieldNames = []
                  UnionCaseNames = []
                  Range = ident.idRange
                  FileIndex = fileIndex }
                :: types

        | _ -> ()

    (List.rev types, List.rev values, List.rev nestedModules, List.rev opens)

/// Collect declarations from signature file module declarations
let rec private collectSigDecls (fileIndex: int) (parentPath: Ident list) (decls: SynModuleSigDecl list) =
    let mutable types = []
    let mutable values = []
    let mutable nestedModules = []
    let mutable opens = []

    for decl in decls do
        match decl with
        | SynModuleSigDecl.Types(types = typeDefs) ->
            for td in typeDefs do
                types <- collectTypeDeclStubFromSig fileIndex td :: types

        | SynModuleSigDecl.Val(valSig = SynValSig(ident = SynIdent(ident, _); accessibility = access)) ->
            values <-
                { Name = ident
                  Accessibility = access.SingleAccess()
                  Range = ident.idRange
                  FileIndex = fileIndex }
                :: values

        | SynModuleSigDecl.NestedModule(moduleInfo = SynComponentInfo(attributes = attribs; longId = ids; accessibility = access); moduleDecls = nestedDecls; range = m) ->
            let name =
                match ids with
                | [ id ] -> id
                | _ -> List.last ids

            let qualName = parentPath @ [ name ]
            let innerTypes, innerValues, innerModules, innerOpens = collectSigDecls fileIndex qualName nestedDecls

            nestedModules <-
                { Name = name
                  QualifiedName = qualName
                  Accessibility = access
                  IsAutoOpen = isAutoOpen attribs
                  Kind = SynModuleOrNamespaceKind.NamedModule
                  Types = innerTypes
                  Values = innerValues
                  NestedModules = innerModules
                  Range = m
                  FileIndex = fileIndex }
                :: nestedModules

            opens <- innerOpens @ opens

        | SynModuleSigDecl.Open(target = target) ->
            match tryGetOpenPath target with
            | Some path -> opens <- path :: opens
            | None -> ()

        | SynModuleSigDecl.Exception(exnSig = SynExceptionSig(exnRepr = SynExceptionDefnRepr(caseName = SynUnionCase(ident = SynIdent(ident, _)); accessibility = access))) ->
            types <-
                { Name = ident
                  Kind = SynTypeDefnKind.Unspecified
                  TypeParamCount = 0
                  Accessibility = access
                  RecordFieldNames = []
                  UnionCaseNames = []
                  Range = ident.idRange
                  FileIndex = fileIndex }
                :: types

        | _ -> ()

    (List.rev types, List.rev values, List.rev nestedModules, List.rev opens)

/// Walk a parsed AST and extract top-level declarations.
let collectFileDeclarations (fileIndex: int) (fileName: string) (parsedInput: ParsedInput) : FileDeclarations =
    match parsedInput with
    | ParsedInput.ImplFile(ParsedImplFileInput(qualifiedNameOfFile = qualName; contents = contents)) ->
        let mutable allOpens = []

        let topLevelModules =
            contents
            |> List.map (fun (SynModuleOrNamespace(longId = longId; kind = kind; attribs = attribs; accessibility = access; decls = decls; range = m)) ->
                let name =
                    match longId with
                    | [ id ] -> id
                    | _ -> List.last longId

                let types, values, nestedModules, opens = collectImplDecls fileIndex longId decls
                allOpens <- opens @ allOpens

                { Name = name
                  QualifiedName = longId
                  Accessibility = access
                  IsAutoOpen = isAutoOpen attribs
                  Kind = kind
                  Types = types
                  Values = values
                  NestedModules = nestedModules
                  Range = m
                  FileIndex = fileIndex })

        { FileIndex = fileIndex
          FileName = fileName
          QualifiedName = qualName
          TopLevelModules = topLevelModules
          Opens = List.rev allOpens
          IdentifierRefs = [] } // IdentifierRefs populated by Track 02 enhanced dependency resolution

    | ParsedInput.SigFile(ParsedSigFileInput(qualifiedNameOfFile = qualName; contents = contents)) ->
        let topLevelModules =
            contents
            |> List.map (fun (SynModuleOrNamespaceSig(longId = longId; kind = kind; attribs = attribs; accessibility = access; decls = decls; range = m)) ->
                let name =
                    match longId with
                    | [ id ] -> id
                    | _ -> List.last longId

                let types, values, nestedModules, _opens = collectSigDecls fileIndex longId decls

                { Name = name
                  QualifiedName = longId
                  Accessibility = access
                  IsAutoOpen = isAutoOpen attribs
                  Kind = kind
                  Types = types
                  Values = values
                  NestedModules = nestedModules
                  Range = m
                  FileIndex = fileIndex })

        let allOpens =
            contents
            |> List.collect (fun (SynModuleOrNamespaceSig(decls = decls)) ->
                decls
                |> List.choose (fun d ->
                    match d with
                    | SynModuleSigDecl.Open(target = target) -> tryGetOpenPath target
                    | _ -> None))

        { FileIndex = fileIndex
          FileName = fileName
          QualifiedName = qualName
          TopLevelModules = topLevelModules
          Opens = allOpens
          IdentifierRefs = [] }

// ---------------------------------------------------------------
// Stub builder: buildFileStub
// ---------------------------------------------------------------

/// Build a ModuleOrNamespaceType stub containing Entity shells for all
/// type/module/value declarations in a file. Entity shells have names and
/// arities but no type representations (TNoRepr).
let buildFileStub (_g: TcGlobals) (fileDecls: FileDeclarations) : QualifiedNameOfFile * ModuleOrNamespaceType =
    /// Convert a SynModuleOrNamespaceKind to the TypedTree ModuleOrNamespaceKind
    let toModuleKind (kind: SynModuleOrNamespaceKind) =
        match kind with
        | SynModuleOrNamespaceKind.NamedModule -> ModuleOrNamespaceKind.ModuleOrType
        | SynModuleOrNamespaceKind.AnonModule -> ModuleOrNamespaceKind.ModuleOrType
        | SynModuleOrNamespaceKind.DeclaredNamespace -> ModuleOrNamespaceKind.Namespace(true)
        | SynModuleOrNamespaceKind.GlobalNamespace -> ModuleOrNamespaceKind.Namespace(false)

    /// Create a minimal Entity shell for a type declaration.
    /// The entity has a name, stamp, and arity but TNoRepr — the real
    /// representation is filled in during type checking.
    let mkTypeEntityStub (stub: TypeDeclStub) : Entity =
        Construct.NewTycon(
            None,
            stub.Name.idText,
            stub.Name.idRange,
            taccessPublic,
            taccessPublic,
            TyparKind.Type,
            LazyWithContext<Typars, range>.NotLazy [],
            XmlDoc.Empty,
            false,
            false,
            false,
            MaybeLazy.Strict(Construct.NewEmptyModuleOrNamespaceType ModuleOrNamespaceKind.ModuleOrType)
        )

    /// Create a minimal Entity shell for a nested module.
    let rec mkModuleEntityStub (stub: ModuleDeclStub) : Entity =
        let moduleKind = toModuleKind stub.Kind
        let moduleTy = mkModuleOrNamespaceTypeStub stub moduleKind
        Construct.NewModuleOrNamespace None taccessPublic stub.Name XmlDoc.Empty [] (MaybeLazy.Strict moduleTy)

    /// Build a ModuleOrNamespaceType from a ModuleDeclStub's contents
    and mkModuleOrNamespaceTypeStub (stub: ModuleDeclStub) (kind: ModuleOrNamespaceKind) : ModuleOrNamespaceType =
        let typeEntities = stub.Types |> List.map mkTypeEntityStub
        let moduleEntities = stub.NestedModules |> List.map mkModuleEntityStub
        let allEntities = typeEntities @ moduleEntities
        Construct.NewModuleOrNamespaceType kind allEntities []

    /// Build the top-level ModuleOrNamespaceType for the file, assembling
    /// all top-level modules/namespaces into a single type.
    let buildTopLevel () : ModuleOrNamespaceType =
        let allEntities =
            fileDecls.TopLevelModules
            |> List.collect (fun topMod ->
                match topMod.Kind with
                | SynModuleOrNamespaceKind.DeclaredNamespace
                | SynModuleOrNamespaceKind.GlobalNamespace ->
                    // For namespaces, types and nested modules go directly into the namespace
                    let typeEntities = topMod.Types |> List.map mkTypeEntityStub
                    let moduleEntities = topMod.NestedModules |> List.map mkModuleEntityStub
                    typeEntities @ moduleEntities

                | SynModuleOrNamespaceKind.NamedModule
                | SynModuleOrNamespaceKind.AnonModule ->
                    // For modules, create a module entity containing everything
                    [ mkModuleEntityStub topMod ])

        Construct.NewModuleOrNamespaceType ModuleOrNamespaceKind.ModuleOrType allEntities []

    (fileDecls.QualifiedName, buildTopLevel ())

// ---------------------------------------------------------------
// Enter phase orchestration
// ---------------------------------------------------------------

/// Run the full enter phase: collect declarations from all files, build stubs,
/// and fold them into the given TcEnv via AddLocalRootModuleOrNamespace.
let runEnterPhase
    (g: TcGlobals)
    (amap: ImportMap)
    (tcEnv: TcEnv)
    (parsedInputs: (string * ParsedInput) array)
    : TcEnv * FileDeclarations array =

    // Step 1: Collect declarations from all files (parallelizable)
    // Memory optimization: the full AST walk (FileContentMapping) produces millions
    // of PrefixedIdentifier entries for large files. For dependency resolution we
    // only need unique first-two-segment prefixes per file, not every occurrence.
    let fileDecls =
        parsedInputs
        |> Array.Parallel.mapi (fun idx (fileName, parsedInput) ->
            let fd = collectFileDeclarations idx fileName parsedInput
            let fileInProject : FileInProject = { Idx = idx; FileName = fileName; ParsedInput = parsedInput }
            let fileContentEntries = FileContentMapping.mkFileContent fileInProject

            // Deduplicate opens and identifier refs by string key to bound memory.
            // Keep only distinct first-two-segment prefixes for PrefixedIdentifier
            // (sufficient for matching against module/namespace export map).
            let opensSet = System.Collections.Generic.HashSet<string>()
            let refsSet = System.Collections.Generic.HashSet<string>()
            let extraOpens = ResizeArray<LongIdent>()
            let identRefs = ResizeArray<LongIdent>()

            let toIdents (parts: string list) = parts |> List.map (fun s -> Ident(s, range0))

            let rec collectRefs entry =
                match entry with
                | FileContentEntry.OpenStatement path ->
                    let key = String.concat "." path
                    if path.Length > 0 && opensSet.Add(key) then
                        extraOpens.Add(toIdents path)
                | FileContentEntry.PrefixedIdentifier path ->
                    // Keep full path but dedup by string key — saves memory vs raw list
                    // while preserving nested module paths like "FSharp.Compiler.AbstractIL.IL".
                    let key = String.concat "." path
                    if path.Length > 0 && refsSet.Add(key) then
                        identRefs.Add(toIdents path)
                | FileContentEntry.TopLevelNamespace(_, nested)
                | FileContentEntry.NestedModule(_, nested) ->
                    for n in nested do collectRefs n
                | _ -> ()

            for entry in fileContentEntries do
                collectRefs entry

            { fd with
                Opens = fd.Opens @ List.ofSeq extraOpens
                IdentifierRefs = List.ofSeq identRefs })

    // Step 2: Build stubs for each file
    let stubs =
        fileDecls
        |> Array.map (fun fd -> buildFileStub g fd)

    // Step 3: Fold all stubs into TcEnv
    let tcEnv =
        (tcEnv, stubs)
        ||> Array.fold (fun env (_qualName, moduleTy) ->
            AddLocalRootModuleOrNamespace g amap range0 env moduleTy)

    (tcEnv, fileDecls)

// ---------------------------------------------------------------
// Dependency graph and topological sort (Track 02)
// ---------------------------------------------------------------

/// Build an export map: for each module/type name, which file index UNIQUELY defines it.
/// Shared namespace prefixes (defined by multiple files) are tracked separately
/// to avoid false dependencies between files in the same namespace.
let private buildExportMap (fileDecls: FileDeclarations array) : Map<string, Set<int>> * Set<string> =
    let mutable exportMap = Map.empty<string, Set<int>>
    let mutable sharedPrefixes = Set.empty<string>

    // Sig/impl pairs are one logical contributor — a name registered by both
    // halves of a pair must NOT count as a shared prefix, otherwise consumers
    // would skip the dependency entirely. Compute the partner map inline (the
    // dedicated helpers live further down in the file).
    let pairPartner =
        let normalize (p: string) = p.Replace('\\', '/')
        let isSig (n: string) = n.EndsWith(".fsi")
        let sigByImplPath =
            fileDecls
            |> Array.choose (fun fd ->
                if isSig fd.FileName then
                    let implPath = normalize (fd.FileName.Substring(0, fd.FileName.Length - 1))
                    Some (implPath, fd.FileIndex)
                else None)
            |> Map.ofArray
        let mutable m = Map.empty<int, int>
        for fd in fileDecls do
            if not (isSig fd.FileName) then
                match Map.tryFind (normalize fd.FileName) sigByImplPath with
                | Some sigIdx ->
                    m <- Map.add fd.FileIndex sigIdx m
                    m <- Map.add sigIdx fd.FileIndex m
                | None -> ()
        m

    let addExport (name: string) (fileIdx: int) =
        let existing = exportMap |> Map.tryFind name |> Option.defaultValue Set.empty
        let updated = Set.add fileIdx existing
        // Distinct logical contributors: collapse sig/impl pairs to one entity.
        let distinctContributors =
            updated
            |> Set.fold (fun (acc: Set<int>) idx ->
                match Map.tryFind idx pairPartner with
                | Some partner when Set.contains partner acc -> acc
                | _ -> Set.add idx acc) Set.empty
        if distinctContributors.Count > 1 then
            sharedPrefixes <- Set.add name sharedPrefixes
        exportMap <- Map.add name updated exportMap

    for fd in fileDecls do
        for topMod in fd.TopLevelModules do
            // Register the full qualified module/namespace name
            let qualName = topMod.QualifiedName |> List.map (fun id -> id.idText) |> String.concat "."
            addExport qualName fd.FileIndex

            // Register each segment prefix for namespace resolution
            let segments = topMod.QualifiedName |> List.map (fun id -> id.idText)
            let mutable prefix = ""
            for seg in segments do
                prefix <- if prefix = "" then seg else prefix + "." + seg
                addExport prefix fd.FileIndex

            // Register type names qualified by module
            for ty in topMod.Types do
                let tyQualName = qualName + "." + ty.Name.idText
                addExport tyQualName fd.FileIndex

            // Register nested module names
            let rec registerNested (parentName: string) (m: ModuleDeclStub) =
                let nestedName = parentName + "." + m.Name.idText
                addExport nestedName fd.FileIndex
                for ty in m.Types do
                    addExport (nestedName + "." + ty.Name.idText) fd.FileIndex
                for nested in m.NestedModules do
                    registerNested nestedName nested

            for nested in topMod.NestedModules do
                registerNested qualName nested

    (exportMap, sharedPrefixes)

/// Add a dependency on a name, optionally skipping shared prefixes.
let private addDepFromExportMap
    (exportMap: Map<string, Set<int>>)
    (sharedPrefixes: Set<string>)
    (skipShared: bool)
    (selfIndex: int)
    (deps: byref<Set<int>>)
    (name: string) =
    if skipShared && Set.contains name sharedPrefixes then ()
    else
        match Map.tryFind name exportMap with
        | Some fileIndices ->
            for idx in fileIndices do
                if idx <> selfIndex then
                    deps <- Set.add idx deps
        | None -> ()

/// Resolve a path (list of idents) against the export map.
/// If prefixesToo is true, also tries all prefixes of the path.
let private resolvePathDeps
    (exportMap: Map<string, Set<int>>)
    (sharedPrefixes: Set<string>)
    (skipShared: bool)
    (prefixesToo: bool)
    (selfIndex: int)
    (deps: byref<Set<int>>)
    (path: LongIdent) =
    let fullPath = path |> List.map (fun id -> id.idText) |> String.concat "."
    addDepFromExportMap exportMap sharedPrefixes skipShared selfIndex &deps fullPath
    if prefixesToo then
        let segments = path |> List.map (fun id -> id.idText)
        let mutable prefix = ""
        for seg in segments do
            prefix <- if prefix = "" then seg else prefix + "." + seg
            addDepFromExportMap exportMap sharedPrefixes skipShared selfIndex &deps prefix

/// Get the namespace-prefix paths that should be prepended when resolving relative refs.
///
/// `module CycleTest.TreeMod` (NamedModule) returns
/// [["CycleTest"]; ["CycleTest"; "TreeMod"]] — a NamedModule's contents live
/// inside an implicit parent namespace, so siblings of that parent are
/// visible without qualification. `namespace FsCheck.Internals`
/// (DeclaredNamespace) returns only [["FsCheck"; "Internals"]] — F# does not
/// auto-import parent namespaces of a declared namespace, so trying parent
/// prefixes would conjure false dependency edges (e.g. a local `Result.isOk`
/// in TypeClass.fs would otherwise match `FsCheck.Result` defined in an
/// unrelated file via the parent prefix).
let private getEnclosingPrefixes (fd: FileDeclarations) : string list list =
    fd.TopLevelModules
    |> List.collect (fun topMod ->
        let segments = topMod.QualifiedName |> List.map (fun id -> id.idText)
        match topMod.Kind with
        | SynModuleOrNamespaceKind.NamedModule ->
            // NamedModule: emit each prefix length so parent-namespace siblings
            // are reachable.
            let mutable acc = []
            let mutable prefix = []
            for seg in segments do
                prefix <- prefix @ [seg]
                acc <- prefix :: acc
            List.rev acc
        | _ ->
            // DeclaredNamespace / GlobalNamespace / AnonModule: only the file's
            // own namespace is in scope — no implicit parent visibility.
            if segments.IsEmpty then [] else [ segments ])
    |> List.distinct

/// Resolve a path against the export map, also trying the path with each
/// enclosing-namespace prefix prepended (for namespace-relative references).
let private resolvePathDepsWithPrefixes
    (exportMap: Map<string, Set<int>>)
    (sharedPrefixes: Set<string>)
    (skipShared: bool)
    (prefixesToo: bool)
    (selfIndex: int)
    (enclosingPrefixes: string list list)
    (deps: byref<Set<int>>)
    (path: LongIdent) =
    // First: literal path resolution
    resolvePathDeps exportMap sharedPrefixes skipShared prefixesToo selfIndex &deps path

    // Then: try with each enclosing namespace prefix prepended.
    // For a ref `ForestMod.X` from a file in `CycleTest.TreeMod`, also try
    // `CycleTest.ForestMod.X` and `CycleTest.TreeMod.ForestMod.X`.
    let pathStrs = path |> List.map (fun id -> id.idText)
    for nsPrefix in enclosingPrefixes do
        let prefixed = nsPrefix @ pathStrs
        let prefixedPath = prefixed |> List.map (fun s -> Ident(s, range0))
        resolvePathDeps exportMap sharedPrefixes skipShared prefixesToo selfIndex &deps prefixedPath

/// Resolve a file's imports against the export map to find dependencies.
/// Opens always create dependencies (they're explicit imports).
/// IdentifierRefs skip shared namespace prefixes to avoid false cycles.
/// When includeIdentRefs is false, only Opens are used (fallback for cycle-prone projects).
let private resolveFileDependencies
    (exportMap: Map<string, Set<int>>)
    (sharedPrefixes: Set<string>)
    (includeIdentRefs: bool)
    (fd: FileDeclarations)
    : Set<int> =

    let mutable deps = Set.empty<int>
    let enclosingPrefixes = getEnclosingPrefixes fd

    // Opens: match full path only (no prefix expansion), never skip shared.
    // Also try with enclosing namespace prefixes (relative opens are valid F#).
    for openPath in fd.Opens do
        resolvePathDepsWithPrefixes exportMap sharedPrefixes false false fd.FileIndex enclosingPrefixes &deps openPath

    if includeIdentRefs then
        // Identifier refs: try prefixes, skip shared prefixes to avoid false cycles.
        // Also try with enclosing namespace prefixes for relative refs.
        for identRef in fd.IdentifierRefs do
            resolvePathDepsWithPrefixes exportMap sharedPrefixes true true fd.FileIndex enclosingPrefixes &deps identRef

    deps

/// Compute strongly connected components using Tarjan's algorithm.
/// Returns SCCs in reverse topological order: SCCs with no dependencies come LAST.
/// Each SCC is a list of file indices that mutually depend on each other.
/// Single-file SCCs represent DAG nodes; multi-file SCCs represent cycle groups.
let private computeSCCs (fileCount: int) (deps: Map<int, Set<int>>) : int list list =
    // Build adjacency: for each file, the set of files it depends on (edges out)
    let adj = Array.create fileCount Set.empty<int>
    for KeyValue(fileIdx, fileDeps) in deps do
        adj.[fileIdx] <- fileDeps

    let index = Array.create fileCount -1
    let lowlink = Array.create fileCount 0
    let onStack = Array.create fileCount false
    let stack = System.Collections.Generic.Stack<int>()
    let sccs = ResizeArray<int list>()
    let mutable nextIndex = 0

    // Iterative Tarjan to avoid stack overflow on large graphs
    let strongconnect (start: int) =
        // Each stack frame: (node, deps enumerator, child state)
        let callStack = System.Collections.Generic.Stack<int * System.Collections.Generic.IEnumerator<int>>()

        let visitNode v =
            index.[v] <- nextIndex
            lowlink.[v] <- nextIndex
            nextIndex <- nextIndex + 1
            stack.Push(v)
            onStack.[v] <- true
            callStack.Push((v, (adj.[v] :> seq<int>).GetEnumerator()))

        visitNode start

        while callStack.Count > 0 do
            let v, enumerator = callStack.Peek()
            let mutable advanced = false
            while not advanced && enumerator.MoveNext() do
                let w = enumerator.Current
                if index.[w] = -1 then
                    // Recurse into w
                    advanced <- true
                    visitNode w
                elif onStack.[w] then
                    lowlink.[v] <- min lowlink.[v] index.[w]
            if not advanced then
                // Done processing v's children — finalize
                callStack.Pop() |> ignore
                // If parent exists, propagate v's lowlink
                if callStack.Count > 0 then
                    let parent, _ = callStack.Peek()
                    lowlink.[parent] <- min lowlink.[parent] lowlink.[v]
                // If v is a root (lowlink == index), emit SCC
                if lowlink.[v] = index.[v] then
                    let scc = ResizeArray<int>()
                    let mutable w = -1
                    while w <> v do
                        w <- stack.Pop()
                        onStack.[w] <- false
                        scc.Add(w)
                    sccs.Add(List.ofSeq scc)

    for v in 0 .. fileCount - 1 do
        if index.[v] = -1 then
            strongconnect v

    // Tarjan's algorithm emits SCCs in topological order (dependencies first):
    // when DFS finishes processing a node, its dependencies have already finished
    // and been emitted. So no reversal needed.
    List.ofSeq sccs

/// Topological sort using Kahn's algorithm with deterministic tie-breaking.
/// Returns file indices in dependency order (dependencies first).
/// Raises an error string if cycles are detected.
let private topologicalSort (fileCount: int) (deps: Map<int, Set<int>>) : Result<int list, string> =
    // Compute in-degree for each node
    let inDegree = Array.create fileCount 0
    let adjacency = Array.init fileCount (fun _ -> ResizeArray<int>())

    for KeyValue(fileIdx, fileDeps) in deps do
        for dep in fileDeps do
            adjacency.[dep].Add(fileIdx) // dep -> fileIdx (dep must come before fileIdx)
            inDegree.[fileIdx] <- inDegree.[fileIdx] + 1

    // Start with nodes that have no dependencies (in-degree 0)
    // Use a sorted set for deterministic ordering (by file index, which is stable)
    let queue = System.Collections.Generic.SortedSet<int>()
    for i in 0 .. fileCount - 1 do
        if inDegree.[i] = 0 then
            queue.Add(i) |> ignore

    let result = ResizeArray<int>(fileCount)

    while queue.Count > 0 do
        let node = Seq.head queue
        queue.Remove(node) |> ignore
        result.Add(node)

        for dependent in adjacency.[node] do
            inDegree.[dependent] <- inDegree.[dependent] - 1
            if inDegree.[dependent] = 0 then
                queue.Add(dependent) |> ignore

    if result.Count < fileCount then
        // Cycle detected — find the nodes involved
        let cycleNodes =
            [| for i in 0 .. fileCount - 1 do
                if inDegree.[i] > 0 then yield i |]
        let cycleDesc =
            cycleNodes
            |> Array.map string
            |> String.concat ", "
        Error (sprintf "Circular file dependencies detected among file indices: %s" cycleDesc)
    else
        Ok (result |> Seq.toList)

/// Check if a file is auto-generated (from obj/ directory, AssemblyInfo, AssemblyAttributes, etc.)
/// Auto-generated files should have no dependencies and be placed first.
let private isAutoGeneratedFile (fd: FileDeclarations) =
    let fn = fd.FileName
    fn.Contains("/obj/") || fn.Contains("\\obj\\") ||
    fn.Contains("AssemblyInfo") || fn.Contains("AssemblyAttributes") ||
    fn.Contains("buildproperties")

/// Check if a filename is a signature file (.fsi)
let private isSigFile (fileName: string) =
    fileName.EndsWith(".fsi")

/// Normalize a file path for comparison (forward slashes, lowercase on Windows)
let private normalizePath (p: string) =
    p.Replace('\\', '/')

/// Find the .fsi/.fs pairs and build a map: impl file index → sig file index
let private buildSigImplPairs (fileDecls: FileDeclarations array) : Map<int, int> =
    // Build map: normalized .fs path (from .fsi with 'i' stripped) → sig file index
    let sigFiles =
        fileDecls
        |> Array.filter (fun fd -> isSigFile fd.FileName)
        |> Array.map (fun fd ->
            let fsPath = normalizePath (fd.FileName.Substring(0, fd.FileName.Length - 1))
            (fsPath, fd.FileIndex))
        |> Map.ofArray

    fileDecls
    |> Array.choose (fun fd ->
        if not (isSigFile fd.FileName) then
            let normalized = normalizePath fd.FileName
            match Map.tryFind normalized sigFiles with
            | Some sigIdx -> Some (fd.FileIndex, sigIdx)
            | None -> None
        else
            None)
    |> Map.ofArray

/// Enforce .fsi before .fs ordering in the final result.
/// For each .fsi/.fs pair, ensure the .fsi immediately precedes its .fs.
let private enforceSigBeforeImpl (fileDecls: FileDeclarations array) (order: int list) : int list =
    let sigImplPairs = buildSigImplPairs fileDecls
    // Reverse map: sig file index → impl file index
    let implForSig = sigImplPairs |> Map.toSeq |> Seq.map (fun (impl, sig') -> (sig', impl)) |> Map.ofSeq

    // Remove sig files from the order — we'll re-insert them before their impls
    let sigIndices = sigImplPairs |> Map.toSeq |> Seq.map snd |> Set.ofSeq
    let orderWithoutSigs = order |> List.filter (fun idx -> not (Set.contains idx sigIndices))

    // Re-insert each sig file immediately before its impl file
    orderWithoutSigs
    |> List.collect (fun idx ->
        match Map.tryFind idx implForSig |> Option.bind (fun _ -> None) with
        | _ ->
            // Check if this impl has a sig file
            match Map.tryFind idx sigImplPairs with
            | Some sigIdx -> [ sigIdx; idx ]  // sig before impl
            | None -> [ idx ])

/// Compute the dependency-ordered file indices from FileDeclarations.
/// Returns file indices in topological order (dependencies before dependents).
let computeDependencyOrder (fileDecls: FileDeclarations array) : int array =
    // Optional debug logging to file when FSHARP_FILE_ORDER_AUTO_DEBUG is set.
    let debugPathOpt =
        match System.Environment.GetEnvironmentVariable "FSHARP_FILE_ORDER_AUTO_DEBUG" with
        | null -> None
        | "" -> None
        | v -> Some v
    let logFile =
        match debugPathOpt with
        | Some p -> Some (System.IO.File.AppendText(p))
        | None -> None
    let log (msg: string) =
        match logFile with
        | Some w -> w.WriteLine(msg); w.Flush()
        | None -> ()

    let exportMap, sharedPrefixes = buildExportMap fileDecls

    let buildDeps (includeIdentRefs: bool) =
        fileDecls
        |> Array.map (fun fd ->
            if isAutoGeneratedFile fd then
                (fd.FileIndex, Set.empty)
            elif isSigFile fd.FileName then
                (fd.FileIndex, Set.empty)
            else
                (fd.FileIndex, resolveFileDependencies exportMap sharedPrefixes includeIdentRefs fd))
        |> Map.ofArray

    // Two-level retry: full refs, then opens-only. If both cycle, fall back to original order.
    // Large tightly-coupled codebases (like the F# compiler itself) have real cycles in
    // their opens — these require Level B (cycle groups) to resolve, which is future work.
    let deps =
        match topologicalSort fileDecls.Length (buildDeps true) with
        | Ok _ -> buildDeps true
        | Error _ ->
            log "Cycles detected with identifier refs — retrying with opens-only"
            buildDeps false

    match topologicalSort fileDecls.Length deps with
    | Ok order ->
        // Partition: auto-generated files first, then user files in dependency order
        let autoGen, userFiles =
            order |> List.partition (fun idx -> isAutoGeneratedFile fileDecls.[idx])
        // Enforce .fsi before .fs pairing in user files
        let userFilesWithSigs = enforceSigBeforeImpl fileDecls userFiles
        let result = (autoGen @ userFilesWithSigs) |> List.toArray
        if debug then
            log (sprintf "Export map size: %d, shared prefixes: %d" (Map.count exportMap) (Set.count sharedPrefixes))
            log (sprintf "Computed order has %d files" result.Length)
            for idx, fileIdx in Array.indexed result do
                let fn = fileDecls.[fileIdx].FileName
                if fn.Contains("EraseClosures") || fn.EndsWith("il.fs") || fn.EndsWith("il.fsi") ||
                   fn.Contains("ILX/Types") || fn.Contains("Morphs") then
                    log (sprintf "  pos %d: %s" idx fn)
            match logFile with Some w -> w.Close() | None -> ()
        result
    | Error msg ->
        log (sprintf "Cycle detected: %s. Falling back to original order." msg)
        match logFile with Some w -> w.Close() | None -> ()
        eprintfn "warning: %s. Falling back to original file order." msg
        [| 0 .. fileDecls.Length - 1 |]

/// A compilation unit: either a single file (DAG node) or a cycle group (SCC with >1 file).
type CompilationUnit =
    | SingleFile of FileIndex: int
    | CycleGroup of FileIndices: int list

/// Compute the dependency-ordered sequence of compilation units from FileDeclarations.
/// Unlike computeDependencyOrder, this preserves cycles as CycleGroup units for Level B processing.
/// Units are returned in dependency order: units with no dependencies come first.
/// Auto-generated files (AssemblyInfo etc.) are placed first regardless.
let computeCompilationUnits (fileDecls: FileDeclarations array) : CompilationUnit array =
    let exportMap, sharedPrefixes = buildExportMap fileDecls

    let deps =
        fileDecls
        |> Array.map (fun fd ->
            if isAutoGeneratedFile fd then
                (fd.FileIndex, Set.empty)
            elif isSigFile fd.FileName then
                (fd.FileIndex, Set.empty)
            else
                (fd.FileIndex, resolveFileDependencies exportMap sharedPrefixes true fd))
        |> Map.ofArray

    let sccs = computeSCCs fileDecls.Length deps

    // Build sig/impl pairing maps
    let sigImplPairs = buildSigImplPairs fileDecls  // impl idx -> sig idx
    let sigIndicesSet = sigImplPairs |> Map.toSeq |> Seq.map snd |> Set.ofSeq

    // Convert SCCs to compilation units, expanding any cycle group to include
    // sig files paired with the impls in that group (so sig+impl stay together).
    let units =
        sccs
        |> List.map (fun scc ->
            match scc with
            | [ single ] -> SingleFile single
            | many ->
                // Pull in any .fsi pairs for impls in this cycle group
                let withSigs =
                    many
                    |> List.collect (fun idx ->
                        match Map.tryFind idx sigImplPairs with
                        | Some sigIdx -> [ sigIdx; idx ]
                        | None -> [ idx ])
                CycleGroup (withSigs |> List.distinct |> List.sort))

    // Track which sig indices are now claimed by a cycle group; they must NOT
    // appear as separate units.
    let sigsInCycleGroups =
        units
        |> List.collect (fun u ->
            match u with
            | CycleGroup ixs -> ixs |> List.filter (fun i -> Set.contains i sigIndicesSet)
            | SingleFile _ -> [])
        |> Set.ofList

    // Partition: auto-generated files first, then user units in dependency order
    let isAutoGenUnit u =
        match u with
        | SingleFile idx -> isAutoGeneratedFile fileDecls.[idx]
        | CycleGroup _ -> false
    let autoGen, userUnits = units |> List.partition isAutoGenUnit

    let withSigsRepositioned =
        userUnits
        |> List.collect (fun u ->
            match u with
            | SingleFile idx when Set.contains idx sigsInCycleGroups ->
                // This sig file is now part of a cycle group; drop the duplicate single-file entry
                []
            | SingleFile idx when Set.contains idx sigIndicesSet ->
                // Sig file with no cycle-group claim; defer to be inserted before its impl
                []
            | SingleFile idx ->
                match Map.tryFind idx sigImplPairs with
                | Some sigIdx when not (Set.contains sigIdx sigsInCycleGroups) ->
                    [ SingleFile sigIdx; SingleFile idx ]  // sig before impl
                | _ -> [ u ]
            | CycleGroup _ -> [ u ])

    (autoGen @ withSigsRepositioned) |> List.toArray
