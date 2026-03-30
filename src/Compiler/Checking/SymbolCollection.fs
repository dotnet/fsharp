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
    let fileDecls =
        parsedInputs
        |> Array.Parallel.mapi (fun idx (fileName, parsedInput) ->
            let fd = collectFileDeclarations idx fileName parsedInput
            // Enrich with identifier references from the full AST walk
            // using the existing GraphChecking.FileContentMapping infrastructure
            let fileInProject : FileInProject = { Idx = idx; FileName = fileName; ParsedInput = parsedInput }
            let fileContentEntries = FileContentMapping.mkFileContent fileInProject
            let identRefs =
                fileContentEntries
                |> List.collect (fun entry ->
                    let rec collectRefs entry =
                        match entry with
                        | FileContentEntry.PrefixedIdentifier path ->
                            // Convert string list to Ident list for storage
                            [ path |> List.map (fun s -> Ident(s, range0)) ]
                        | FileContentEntry.OpenStatement path ->
                            [ path |> List.map (fun s -> Ident(s, range0)) ]
                        | FileContentEntry.TopLevelNamespace(_, nested)
                        | FileContentEntry.NestedModule(_, nested) ->
                            nested |> List.collect collectRefs
                        | _ -> []
                    collectRefs entry)
            { fd with
                Opens = fd.Opens @ (identRefs |> List.filter (fun ids -> ids.Length > 0))
                IdentifierRefs = identRefs })

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

/// Build an export map: for each module/namespace name, which file index defines it.
/// A name may be defined by multiple files (e.g., namespaces spanning files).
let private buildExportMap (fileDecls: FileDeclarations array) : Map<string, Set<int>> =
    let mutable exportMap = Map.empty<string, Set<int>>

    let addExport (name: string) (fileIdx: int) =
        let existing = exportMap |> Map.tryFind name |> Option.defaultValue Set.empty
        exportMap <- Map.add name (Set.add fileIdx existing) exportMap

    for fd in fileDecls do
        for topMod in fd.TopLevelModules do
            // Register the module/namespace name
            let qualName = topMod.QualifiedName |> List.map (fun id -> id.idText) |> String.concat "."
            addExport qualName fd.FileIndex

            // Also register just the last segment (the module name without namespace prefix)
            addExport topMod.Name.idText fd.FileIndex

            // Register each segment prefix for namespace resolution
            // e.g., "A.B.C" registers "A", "A.B", "A.B.C"
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
                addExport m.Name.idText fd.FileIndex
                for ty in m.Types do
                    addExport (nestedName + "." + ty.Name.idText) fd.FileIndex
                for nested in m.NestedModules do
                    registerNested nestedName nested

            for nested in topMod.NestedModules do
                registerNested qualName nested

    exportMap

/// Resolve a file's imports (Opens) against the export map to find dependencies.
let private resolveFileDependencies
    (exportMap: Map<string, Set<int>>)
    (fd: FileDeclarations)
    : Set<int> =

    let mutable deps = Set.empty<int>

    for openPath in fd.Opens do
        // Try the full open path
        let fullPath = openPath |> List.map (fun id -> id.idText) |> String.concat "."
        match Map.tryFind fullPath exportMap with
        | Some fileIndices ->
            for idx in fileIndices do
                if idx <> fd.FileIndex then
                    deps <- Set.add idx deps
        | None -> ()

        // Also try each prefix of the open path
        let segments = openPath |> List.map (fun id -> id.idText)
        let mutable prefix = ""
        for seg in segments do
            prefix <- if prefix = "" then seg else prefix + "." + seg
            match Map.tryFind prefix exportMap with
            | Some fileIndices ->
                for idx in fileIndices do
                    if idx <> fd.FileIndex then
                        deps <- Set.add idx deps
            | None -> ()

    deps

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

/// Compute the dependency-ordered file indices from FileDeclarations.
/// Returns file indices in topological order (dependencies before dependents).
let computeDependencyOrder (fileDecls: FileDeclarations array) : int array =
    let exportMap = buildExportMap fileDecls

    // Build dependency map: fileIndex -> set of file indices it depends on
    let deps =
        fileDecls
        |> Array.map (fun fd -> (fd.FileIndex, resolveFileDependencies exportMap fd))
        |> Map.ofArray

    match topologicalSort fileDecls.Length deps with
    | Ok order -> order |> List.toArray
    | Error msg -> failwith msg
