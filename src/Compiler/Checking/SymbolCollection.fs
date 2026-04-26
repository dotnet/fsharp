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
      /// Names of static and instance members defined directly on this type
      /// (including those declared via `member this.X = ...`,
      /// `static member X = ...`, and `member val X = ...`).
      MemberNames: Ident list
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

/// Extract a member name from a SynBinding's head pattern.
/// `static member X = ...` → headPat is `SynPat.LongIdent([X])` → returns X.
/// `member this.X = ...` → headPat is `SynPat.LongIdent([this; X])` → returns X.
/// Other forms return None.
let private tryGetMemberName (b: SynBinding) =
    let (SynBinding(headPat = pat)) = b
    let rec stripWrappers p =
        match p with
        | SynPat.Paren(pat = inner) | SynPat.Typed(pat = inner) | SynPat.Attrib(pat = inner) ->
            stripWrappers inner
        | _ -> p
    match stripWrappers pat with
    | SynPat.LongIdent(longDotId = SynLongIdent(id = ids)) ->
        match ids with
        | [] -> None
        | _ -> Some(List.last ids)
    | SynPat.Named(ident = SynIdent(ident, _)) -> Some ident
    | _ -> None

/// Extract member names from a list of SynMemberDefns. Static members,
/// instance members, abstract slots, auto-properties, and let-bound class
/// values all contribute names callable as `Type.X`.
let private collectMemberNamesFromDefns (members: SynMemberDefns) : Ident list =
    let acc = ResizeArray<Ident>()
    let rec walk (m: SynMemberDefn) =
        match m with
        | SynMemberDefn.Member(memberDefn = b) ->
            match tryGetMemberName b with Some i -> acc.Add(i) | None -> ()
        | SynMemberDefn.GetSetMember(memberDefnForGet = bgOpt; memberDefnForSet = bsOpt) ->
            (match bgOpt with
             | Some b -> match tryGetMemberName b with Some i -> acc.Add(i) | None -> ()
             | None -> ())
            (match bsOpt with
             | Some b -> match tryGetMemberName b with Some i -> acc.Add(i) | None -> ()
             | None -> ())
        | SynMemberDefn.LetBindings(bindings = bs) ->
            for b in bs do
                let (SynBinding(headPat = p)) = b
                match tryGetBindingName b with
                | Some stub -> acc.Add(stub.Name)
                | None -> ignore p
        | SynMemberDefn.AbstractSlot(slotSig = SynValSig(ident = SynIdent(ident, _))) ->
            acc.Add(ident)
        | SynMemberDefn.ValField(fieldInfo = SynField(idOpt = Some idF)) ->
            acc.Add(idF)
        | SynMemberDefn.AutoProperty(ident = ident) -> acc.Add(ident)
        | _ -> ()
    for m in members do walk m
    List.ofSeq acc

/// Extract member names from a list of SynMemberSigs (for signature files).
let private collectMemberNamesFromSigs (members: SynMemberSig list) : Ident list =
    let acc = ResizeArray<Ident>()
    for m in members do
        match m with
        | SynMemberSig.Member(memberSig = SynValSig(ident = SynIdent(ident, _))) -> acc.Add(ident)
        | SynMemberSig.ValField(field = SynField(idOpt = Some idF)) -> acc.Add(idF)
        | _ -> ()
    List.ofSeq acc

/// Extract type declaration stubs from a SynTypeDefn
let private collectTypeDeclStub (fileIndex: int) (synTypeDefn: SynTypeDefn) : TypeDeclStub =
    let (SynTypeDefn(typeInfo = SynComponentInfo(typeParams = typarDecls; longId = ids; accessibility = access); typeRepr = repr; members = extraMembers; implicitConstructor = ctorOpt)) =
        synTypeDefn

    let name =
        match ids with
        | [] -> Ident("", range0)
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

    let objectModelMembers =
        match repr with
        | SynTypeDefnRepr.ObjectModel(members = ms) -> collectMemberNamesFromDefns ms
        | _ -> []

    let memberNames =
        let extra = collectMemberNamesFromDefns extraMembers
        let ctor =
            match ctorOpt with
            | Some m -> collectMemberNamesFromDefns [ m ]
            | None -> []
        objectModelMembers @ extra @ ctor

    { Name = name
      Kind = kind
      TypeParamCount = typeParamCount
      Accessibility = access
      RecordFieldNames = recordFields
      UnionCaseNames = unionCases
      MemberNames = memberNames
      Range = name.idRange
      FileIndex = fileIndex }

/// Extract type declaration stubs from a SynTypeDefnSig (signature file)
let private collectTypeDeclStubFromSig (fileIndex: int) (synTypeDefnSig: SynTypeDefnSig) : TypeDeclStub =
    let (SynTypeDefnSig(typeInfo = SynComponentInfo(typeParams = typarDecls; longId = ids; accessibility = access); typeRepr = repr; members = extraMemberSigs)) =
        synTypeDefnSig

    let name =
        match ids with
        | [] -> Ident("", range0)
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

    let memberNames =
        let objectModelMembers =
            match repr with
            | SynTypeDefnSigRepr.ObjectModel(memberSigs = ms) -> collectMemberNamesFromSigs ms
            | _ -> []
        objectModelMembers @ collectMemberNamesFromSigs extraMemberSigs

    { Name = name
      Kind = kind
      TypeParamCount = typeParamCount
      Accessibility = access
      RecordFieldNames = recordFields
      UnionCaseNames = unionCases
      MemberNames = memberNames
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
                | [] -> Ident("", range0)
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
                  MemberNames = []
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
                | [] -> Ident("", range0)
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
                  MemberNames = []
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
                    | [] -> Ident("", range0)
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
                    | [] -> Ident("", range0)
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
    ///
    /// Synthesises rigid type parameters matching the count from the AST so
    /// references like `MyType<'A>` or `Foo<int, string>` from another file
    /// can be type-checked against the stub before the real implementation
    /// runs (otherwise FS0033 "non-generic type does not expect type
    /// arguments" surfaces for any cross-file generic-type ref).
    let mkTypeEntityStub (stub: TypeDeclStub) : Entity =
        let typars : Typars =
            [ for i in 0 .. stub.TypeParamCount - 1 ->
                let nm = sprintf "T%d" i
                Construct.NewRigidTypar nm stub.Name.idRange ]
        Construct.NewTycon(
            None,
            stub.Name.idText,
            stub.Name.idRange,
            taccessPublic,
            taccessPublic,
            TyparKind.Type,
            LazyWithContext<Typars, range>.NotLazy typars,
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

    /// Build a ModuleOrNamespaceType from a ModuleDeclStub's contents.
    /// Filters out private/internal child modules and types: other files
    /// can't reference them anyway, and stubbing them would conflict with
    /// the real `module private X = ...` declaration when its file is
    /// type-checked (FS0245 "X is not a concrete module or type").
    and mkModuleOrNamespaceTypeStub (stub: ModuleDeclStub) (kind: ModuleOrNamespaceKind) : ModuleOrNamespaceType =
        let isPublic acc =
            match acc with
            | None -> true
            | Some(SynAccess.Public _) -> true
            | _ -> false
        let typeEntities =
            stub.Types
            |> List.filter (fun t -> isPublic t.Accessibility)
            |> List.map mkTypeEntityStub
        let moduleEntities =
            stub.NestedModules
            |> List.filter (fun m -> isPublic m.Accessibility)
            |> List.map mkModuleEntityStub
        let allEntities = typeEntities @ moduleEntities
        Construct.NewModuleOrNamespaceType kind allEntities []

    /// Build the top-level ModuleOrNamespaceType for the file, assembling
    /// top-level types into a single namespace-shaped container.
    ///
    /// We deliberately do NOT stub modules (top-level NamedModule files or
    /// nested modules inside a namespace). F# treats a stub module as an
    /// already-declared entity, and the real `module X = ...` declaration
    /// then fails with FS0245 "X is not a concrete module or type". Type
    /// stubs are fine because F# lets a type be forward-declared.
    let buildTopLevel () : ModuleOrNamespaceType =
        let isPublic acc =
            match acc with
            | None -> true
            | Some(SynAccess.Public _) -> true
            | _ -> false
        let allEntities =
            fileDecls.TopLevelModules
            |> List.collect (fun topMod ->
                match topMod.Kind with
                | SynModuleOrNamespaceKind.DeclaredNamespace
                | SynModuleOrNamespaceKind.GlobalNamespace ->
                    // For namespaces, only types go in (modules are skipped).
                    topMod.Types
                    |> List.filter (fun t -> isPublic t.Accessibility)
                    |> List.map mkTypeEntityStub
                | SynModuleOrNamespaceKind.NamedModule
                | SynModuleOrNamespaceKind.AnonModule ->
                    // Top-level module files: skip the module stub entirely.
                    [])
        Construct.NewModuleOrNamespaceType ModuleOrNamespaceKind.ModuleOrType allEntities []

    (fileDecls.QualifiedName, buildTopLevel ())

// ---------------------------------------------------------------
// Full-path identifier reference walker
// ---------------------------------------------------------------

/// Walk a parsed input and collect every long-identifier reference with its
/// FULL path preserved. Distinct from FileContentMapping.PrefixedIdentifier,
/// which truncates the trailing segment via `skipLast=true`.
///
/// Why we need full paths: with truncated paths, `Random.CreateWithSeedAndGamma`
/// (a real cross-file static call to a project type) and `Result.isOk` (a
/// FSharp.Core call) both reduce to a single-segment `["Random"]` /
/// `["Result"]` and are indistinguishable. With full paths plus type-member
/// registration, the analyser can match `Random.CreateWithSeedAndGamma`
/// against a registered member and reject `Result.isOk` (no such member).
///
/// Hand-rolled walker — SyntaxNode/ParsedInput.fold live in the Service
/// layer above this one. We dispatch on every SynExpr/SynPat/SynType
/// constructor present in this fork's AST. Unknown variants fall through
/// to no-op to keep the walker resilient to AST changes.
let private collectFullPathRefs (parsedInput: ParsedInput) : LongIdent list =
    let refs = ResizeArray<LongIdent>()
    let seen = System.Collections.Generic.HashSet<string>()
    let addIds (lid: LongIdent) =
        if not (List.isEmpty lid) then
            let key = lid |> List.map (fun (i: Ident) -> i.idText) |> String.concat "."
            if seen.Add(key) then refs.Add(lid)

    let rec walkExpr (e: SynExpr) =
        match e with
        | SynExpr.Paren(expr = e1) -> walkExpr e1
        | SynExpr.Quote(operator = op; quotedExpr = q) -> walkExpr op; walkExpr q
        | SynExpr.Const _ -> ()
        | SynExpr.Typed(expr = e1; targetType = ty) -> walkExpr e1; walkType ty
        | SynExpr.Tuple(exprs = es) -> for x in es do walkExpr x
        | SynExpr.AnonRecd(copyInfo = copyInfo; recordFields = fields) ->
            (match copyInfo with Some(e1, _) -> walkExpr e1 | None -> ())
            for (SynLongIdent(id = ids), _, e1) in fields do
                addIds ids
                walkExpr e1
        | SynExpr.ArrayOrList(exprs = es) -> for x in es do walkExpr x
        | SynExpr.Record(baseInfo = baseInfo; copyInfo = copyInfo; recordFields = fields) ->
            (match baseInfo with
             | Some(ty, e1, _, _, _) -> walkType ty; walkExpr e1
             | None -> ())
            (match copyInfo with Some(e1, _) -> walkExpr e1 | None -> ())
            for SynExprRecordField(fieldName = (SynLongIdent(id = ids), _); expr = eOpt) in fields do
                addIds ids
                (match eOpt with Some e1 -> walkExpr e1 | None -> ())
        | SynExpr.New(targetType = ty; expr = e1) -> walkType ty; walkExpr e1
        | SynExpr.ObjExpr(objType = ty; argOptions = argOpt; bindings = bs; members = ms; extraImpls = extras) ->
            walkType ty
            (match argOpt with Some(e1, _) -> walkExpr e1 | None -> ())
            for b in bs do walkBinding b
            for m in ms do walkMember m
            for SynInterfaceImpl(interfaceTy = ty2; bindings = bs2; members = ms2) in extras do
                walkType ty2
                for b in bs2 do walkBinding b
                for m in ms2 do walkMember m
        | SynExpr.While(whileExpr = e1; doExpr = e2) -> walkExpr e1; walkExpr e2
        | SynExpr.For(identBody = e1; toBody = e2; doBody = e3) ->
            walkExpr e1; walkExpr e2; walkExpr e3
        | SynExpr.ForEach(pat = pat; enumExpr = e1; bodyExpr = e2) ->
            walkPat pat; walkExpr e1; walkExpr e2
        | SynExpr.ArrayOrListComputed(expr = e1) -> walkExpr e1
        | SynExpr.IndexRange(expr1 = e1Opt; expr2 = e2Opt) ->
            (match e1Opt with Some e1 -> walkExpr e1 | None -> ())
            (match e2Opt with Some e2 -> walkExpr e2 | None -> ())
        | SynExpr.IndexFromEnd(expr = e1) -> walkExpr e1
        | SynExpr.ComputationExpr(expr = e1) -> walkExpr e1
        | SynExpr.Lambda(args = sps; body = e1) -> walkSimplePats sps; walkExpr e1
        | SynExpr.MatchLambda(matchClauses = cs) -> for c in cs do walkMatchClause c
        | SynExpr.Match(expr = e1; clauses = cs) ->
            walkExpr e1; for c in cs do walkMatchClause c
        | SynExpr.Do(expr = e1) -> walkExpr e1
        | SynExpr.Assert(expr = e1) -> walkExpr e1
        | SynExpr.App(funcExpr = e1; argExpr = e2) ->
            // Special-case `f arg` where `f` is a single Ident: capture it
            // as a 1-segment ref. Most local-bindings/parameters won't
            // match anything in the export map; the few that DO match are
            // exactly the cross-file deps we want to detect (e.g.
            // `transferStream conn stream` from a file with `open Suave.Sockets`
            // where transferStream is in `[<AutoOpen>] module AsyncSocket`).
            (match e1 with
             | SynExpr.Ident ident -> addIds [ ident ]
             | _ -> ())
            walkExpr e1; walkExpr e2
        | SynExpr.TypeApp(expr = e1; typeArgs = tys) ->
            walkExpr e1; for ty in tys do walkType ty
        | SynExpr.TryWith(tryExpr = e1; withCases = cs) ->
            walkExpr e1; for c in cs do walkMatchClause c
        | SynExpr.TryFinally(tryExpr = e1; finallyExpr = e2) -> walkExpr e1; walkExpr e2
        | SynExpr.Lazy(expr = e1) -> walkExpr e1
        | SynExpr.Sequential(expr1 = e1; expr2 = e2) -> walkExpr e1; walkExpr e2
        | SynExpr.IfThenElse(ifExpr = e1; thenExpr = e2; elseExpr = e3Opt) ->
            walkExpr e1; walkExpr e2
            (match e3Opt with Some e3 -> walkExpr e3 | None -> ())
        | SynExpr.Typar _ -> ()
        | SynExpr.Ident _ -> ()
        | SynExpr.LongIdent(longDotId = SynLongIdent(id = ids)) -> addIds ids
        | SynExpr.LongIdentSet(longDotId = SynLongIdent(id = ids); expr = e1) ->
            addIds ids; walkExpr e1
        | SynExpr.DotGet(expr = e1) ->
            // Postfix on a dynamic expression — recurse into the expression
            // but skip the trailing long-ident segments (they're field/method
            // names on whatever the expression evaluates to).
            walkExpr e1
        | SynExpr.DotLambda(expr = e1) -> walkExpr e1
        | SynExpr.DotSet(targetExpr = e1; rhsExpr = e2) -> walkExpr e1; walkExpr e2
        | SynExpr.Set(targetExpr = e1; rhsExpr = e2) -> walkExpr e1; walkExpr e2
        | SynExpr.DotIndexedGet(objectExpr = e1; indexArgs = e2) ->
            walkExpr e1; walkExpr e2
        | SynExpr.DotIndexedSet(objectExpr = e1; indexArgs = e2; valueExpr = e3) ->
            walkExpr e1; walkExpr e2; walkExpr e3
        | SynExpr.NamedIndexedPropertySet(longDotId = SynLongIdent(id = ids); expr1 = e1; expr2 = e2) ->
            addIds ids; walkExpr e1; walkExpr e2
        | SynExpr.DotNamedIndexedPropertySet(targetExpr = e1; argExpr = e2; rhsExpr = e3) ->
            walkExpr e1; walkExpr e2; walkExpr e3
        | SynExpr.TypeTest(expr = e1; targetType = ty) -> walkExpr e1; walkType ty
        | SynExpr.Upcast(expr = e1; targetType = ty) -> walkExpr e1; walkType ty
        | SynExpr.Downcast(expr = e1; targetType = ty) -> walkExpr e1; walkType ty
        | SynExpr.InferredUpcast(expr = e1) -> walkExpr e1
        | SynExpr.InferredDowncast(expr = e1) -> walkExpr e1
        | SynExpr.Null _ -> ()
        | SynExpr.AddressOf(expr = e1) -> walkExpr e1
        | SynExpr.TraitCall(supportTys = supTy; argExpr = e1) ->
            walkType supTy; walkExpr e1
        | SynExpr.JoinIn(lhsExpr = e1; rhsExpr = e2) -> walkExpr e1; walkExpr e2
        | SynExpr.ImplicitZero _ -> ()
        | SynExpr.SequentialOrImplicitYield(expr1 = e1; expr2 = e2; ifNotStmt = e3) ->
            walkExpr e1; walkExpr e2; walkExpr e3
        | SynExpr.YieldOrReturn(expr = e1) -> walkExpr e1
        | SynExpr.YieldOrReturnFrom(expr = e1) -> walkExpr e1
        | SynExpr.LetOrUse letOrUse ->
            for b in letOrUse.Bindings do walkBinding b
            walkExpr letOrUse.Body
        | SynExpr.MatchBang(expr = e1; clauses = cs) ->
            walkExpr e1; for c in cs do walkMatchClause c
        | SynExpr.DoBang(expr = e1) -> walkExpr e1
        | SynExpr.WhileBang(whileExpr = e1; doExpr = e2) -> walkExpr e1; walkExpr e2
        | SynExpr.LibraryOnlyILAssembly(typeArgs = tys; args = es; retTy = retTys) ->
            for ty in tys do walkType ty
            for e1 in es do walkExpr e1
            for ty in retTys do walkType ty
        | SynExpr.LibraryOnlyStaticOptimization(expr = e1; optimizedExpr = e2) ->
            walkExpr e1; walkExpr e2
        | SynExpr.LibraryOnlyUnionCaseFieldGet(expr = e1) -> walkExpr e1
        | SynExpr.LibraryOnlyUnionCaseFieldSet(expr = e1; rhsExpr = e2) ->
            walkExpr e1; walkExpr e2
        | SynExpr.ArbitraryAfterError _ -> ()
        | SynExpr.FromParseError(expr = e1) -> walkExpr e1
        | SynExpr.DiscardAfterMissingQualificationAfterDot(expr = e1) -> walkExpr e1
        | SynExpr.Fixed(expr = e1) -> walkExpr e1
        | SynExpr.InterpolatedString(contents = parts) ->
            for part in parts do
                match part with
                | SynInterpolatedStringPart.FillExpr(fillExpr = e1) -> walkExpr e1
                | SynInterpolatedStringPart.String _ -> ()
        | SynExpr.DebugPoint(innerExpr = e1) -> walkExpr e1
        | SynExpr.Dynamic(funcExpr = e1; argExpr = e2) -> walkExpr e1; walkExpr e2

    and walkType (t: SynType) =
        match t with
        | SynType.LongIdent(SynLongIdent(id = ids)) -> addIds ids
        | SynType.App(typeName = ty; typeArgs = tys) ->
            walkType ty
            for ti in tys do walkType ti
        | SynType.LongIdentApp(typeName = ty; longDotId = SynLongIdent(id = ids); typeArgs = tys) ->
            walkType ty
            addIds ids
            for ti in tys do walkType ti
        | SynType.Tuple(path = segs) ->
            for seg in segs do
                match seg with
                | SynTupleTypeSegment.Type ty -> walkType ty
                | _ -> ()
        | SynType.AnonRecd(fields = fs) ->
            for (_, ty) in fs do walkType ty
        | SynType.Array(elementType = ty) -> walkType ty
        | SynType.Fun(argType = a; returnType = r) -> walkType a; walkType r
        | SynType.Var _ -> ()
        | SynType.Anon _ -> ()
        | SynType.WithGlobalConstraints(typeName = ty) -> walkType ty
        | SynType.HashConstraint(innerType = ty) -> walkType ty
        | SynType.MeasurePower(baseMeasure = ty) -> walkType ty
        | SynType.StaticConstant _ -> ()
        | SynType.StaticConstantNull _ -> ()
        | SynType.StaticConstantExpr(expr = e1) -> walkExpr e1
        | SynType.StaticConstantNamed(ident = a; value = b) -> walkType a; walkType b
        | SynType.WithNull(innerType = ty) -> walkType ty
        | SynType.Paren(innerType = ty) -> walkType ty
        | SynType.SignatureParameter(usedType = ty; attributes = attrs) ->
            walkAttribs attrs
            walkType ty
        | SynType.Or(lhsType = a; rhsType = b) -> walkType a; walkType b
        | SynType.FromParseError _ -> ()
        | SynType.Intersection(types = tys) -> for ty in tys do walkType ty

    and walkPat (p: SynPat) =
        match p with
        | SynPat.Const _ -> ()
        | SynPat.Wild _ -> ()
        | SynPat.Named _ -> ()
        | SynPat.Typed(pat = sp; targetType = ty) -> walkPat sp; walkType ty
        | SynPat.Attrib(pat = sp; attributes = attrs) -> walkAttribs attrs; walkPat sp
        | SynPat.Or(lhsPat = a; rhsPat = b) -> walkPat a; walkPat b
        | SynPat.ListCons(lhsPat = a; rhsPat = b) -> walkPat a; walkPat b
        | SynPat.Ands(pats = ps) -> for sp in ps do walkPat sp
        | SynPat.As(lhsPat = a; rhsPat = b) -> walkPat a; walkPat b
        | SynPat.LongIdent(longDotId = SynLongIdent(id = ids); argPats = argPats) ->
            addIds ids
            walkArgPats argPats
        | SynPat.Tuple(elementPats = ps) -> for sp in ps do walkPat sp
        | SynPat.Paren(pat = sp) -> walkPat sp
        | SynPat.ArrayOrList(elementPats = ps) -> for sp in ps do walkPat sp
        | SynPat.Record(fieldPats = fps) ->
            for fp in fps do
                let (NamePatPairField(fieldName = SynLongIdent(id = ids); pat = sp)) = fp
                addIds ids
                walkPat sp
        | SynPat.Null _ -> ()
        | SynPat.OptionalVal _ -> ()
        | SynPat.IsInst(pat = ty) -> walkType ty
        | SynPat.QuoteExpr(expr = e1) -> walkExpr e1
        | SynPat.InstanceMember _ -> ()
        | SynPat.FromParseError(pat = sp) -> walkPat sp

    and walkArgPats (a: SynArgPats) =
        match a with
        | SynArgPats.Pats pats -> for sp in pats do walkPat sp
        | SynArgPats.NamePatPairs(pats = nps) ->
            for np in nps do
                let (NamePatPairField(fieldName = SynLongIdent(id = ids); pat = sp)) = np
                addIds ids
                walkPat sp

    and walkSimplePat (sp: SynSimplePat) =
        match sp with
        | SynSimplePat.Id _ -> ()
        | SynSimplePat.Typed(pat = inner; targetType = ty) -> walkSimplePat inner; walkType ty
        | SynSimplePat.Attrib(pat = inner; attributes = attrs) ->
            walkAttribs attrs; walkSimplePat inner

    and walkSimplePats (sps: SynSimplePats) =
        match sps with
        | SynSimplePats.SimplePats(pats = pats) -> for sp in pats do walkSimplePat sp

    and walkMatchClause (SynMatchClause(pat = p; whenExpr = wOpt; resultExpr = e)) =
        walkPat p
        (match wOpt with Some w -> walkExpr w | None -> ())
        walkExpr e

    and walkBinding (SynBinding(headPat = p; returnInfo = retOpt; expr = e; attributes = attrs)) =
        walkAttribs attrs
        walkPat p
        (match retOpt with
         | Some(SynBindingReturnInfo(typeName = ty; attributes = retAttrs)) ->
             walkAttribs retAttrs; walkType ty
         | None -> ())
        walkExpr e

    and walkMember (m: SynMemberDefn) =
        match m with
        | SynMemberDefn.Open _ -> ()
        | SynMemberDefn.Member(memberDefn = b) -> walkBinding b
        | SynMemberDefn.GetSetMember(memberDefnForGet = bgOpt; memberDefnForSet = bsOpt) ->
            (match bgOpt with Some b -> walkBinding b | None -> ())
            (match bsOpt with Some b -> walkBinding b | None -> ())
        | SynMemberDefn.ImplicitCtor(attributes = attrs; ctorArgs = pat) ->
            walkAttribs attrs; walkPat pat
        | SynMemberDefn.ImplicitInherit(inheritType = ty; inheritArgs = e1) ->
            walkType ty; walkExpr e1
        | SynMemberDefn.LetBindings(bindings = bs) -> for b in bs do walkBinding b
        | SynMemberDefn.AbstractSlot(slotSig = SynValSig(synType = ty; attributes = attrs)) ->
            walkAttribs attrs; walkType ty
        | SynMemberDefn.Interface(interfaceType = ty; members = msOpt) ->
            walkType ty
            match msOpt with
            | Some xs -> for x in xs do walkMember x
            | None -> ()
        | SynMemberDefn.Inherit(baseType = tyOpt) ->
            (match tyOpt with Some ty -> walkType ty | None -> ())
        | SynMemberDefn.ValField(fieldInfo = SynField(fieldType = ty; attributes = attrs)) ->
            walkAttribs attrs; walkType ty
        | SynMemberDefn.NestedType(typeDefn = td) -> walkTypeDefn td
        | SynMemberDefn.AutoProperty(attributes = attrs; typeOpt = tyOpt; synExpr = e1) ->
            walkAttribs attrs
            (match tyOpt with Some ty -> walkType ty | None -> ())
            walkExpr e1

    and walkAttribs (xs: SynAttributes) =
        for al in xs do
            for a in al.Attributes do
                addIds a.TypeName.LongIdent
                walkExpr a.ArgExpr

    and walkTypeDefn (SynTypeDefn(typeInfo = info; typeRepr = repr; members = ms; implicitConstructor = ctorOpt)) =
        let (SynComponentInfo(attributes = attrs)) = info
        walkAttribs attrs
        walkTypeDefnRepr repr
        (match ctorOpt with Some c -> walkMember c | None -> ())
        for m in ms do walkMember m

    and walkTypeDefnRepr (r: SynTypeDefnRepr) =
        match r with
        | SynTypeDefnRepr.ObjectModel(members = ms) -> for m in ms do walkMember m
        | SynTypeDefnRepr.Simple(simpleRepr = simple) -> walkSimpleRepr simple
        | SynTypeDefnRepr.Exception(exnRepr = SynExceptionDefnRepr(caseName = uc; attributes = attrs)) ->
            walkAttribs attrs; walkUnionCase uc

    and walkSimpleRepr (r: SynTypeDefnSimpleRepr) =
        match r with
        | SynTypeDefnSimpleRepr.Union(unionCases = cases) ->
            for uc in cases do walkUnionCase uc
        | SynTypeDefnSimpleRepr.Enum(cases = cases) ->
            for SynEnumCase(valueExpr = e1; attributes = attrs) in cases do
                walkAttribs attrs; walkExpr e1
        | SynTypeDefnSimpleRepr.Record(recordFields = fields) ->
            for f in fields do walkField f
        | SynTypeDefnSimpleRepr.General(inherits = inhs; slotsigs = ss; fields = fields) ->
            for (ty, _, _) in inhs do walkType ty
            for (SynValSig(synType = ty), _) in ss do walkType ty
            for f in fields do walkField f
        | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> ()
        | SynTypeDefnSimpleRepr.TypeAbbrev(rhsType = ty) -> walkType ty
        | SynTypeDefnSimpleRepr.None _ -> ()
        | SynTypeDefnSimpleRepr.Exception(exnRepr = SynExceptionDefnRepr(caseName = uc; attributes = attrs)) ->
            walkAttribs attrs; walkUnionCase uc

    and walkUnionCase (SynUnionCase(attributes = attrs; caseType = ck)) =
        walkAttribs attrs
        match ck with
        | SynUnionCaseKind.Fields cases -> for f in cases do walkField f
        | SynUnionCaseKind.FullType(fullType = ty) -> walkType ty

    and walkField (SynField(attributes = attrs; fieldType = ty)) =
        walkAttribs attrs; walkType ty

    let rec walkDecl (d: SynModuleDecl) =
        match d with
        | SynModuleDecl.ModuleAbbrev(longId = ids) -> addIds ids
        | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(attributes = attrs); decls = inner) ->
            walkAttribs attrs
            for d in inner do walkDecl d
        | SynModuleDecl.Let(bindings = bs) -> for b in bs do walkBinding b
        | SynModuleDecl.Expr(expr = e) -> walkExpr e
        | SynModuleDecl.Types(typeDefns = tds) -> for td in tds do walkTypeDefn td
        | SynModuleDecl.Exception(exnDefn = SynExceptionDefn(exnRepr = SynExceptionDefnRepr(caseName = uc; attributes = attrs); members = ms)) ->
            walkAttribs attrs
            walkUnionCase uc
            for m in ms do walkMember m
        | SynModuleDecl.Open _ -> ()
        | SynModuleDecl.Attributes(attributes = attrs) -> walkAttribs attrs
        | SynModuleDecl.HashDirective _ -> ()
        | SynModuleDecl.NamespaceFragment _ -> ()

    match parsedInput with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = contents)) ->
        for SynModuleOrNamespace(decls = decls; attribs = attrs) in contents do
            walkAttribs attrs
            for d in decls do walkDecl d
    | ParsedInput.SigFile _ ->
        // Sig files contribute opens/exports separately via collectSigDecls.
        // We skip walking sig-file bodies here — the declarations they expose
        // are already in fd.Opens / fd.TopLevelModules. Sig files aren't users
        // of cross-file deps in the same sense impls are.
        ()

    List.ofSeq refs

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

    // Step 1: Collect declarations from all files (parallelizable).
    //
    // Identifier refs come from the custom `collectFullPathRefs` walker so we
    // get full paths for `Random.CreateWithSeedAndGamma` etc., not the
    // truncated qualifier-only paths that FileContentMapping emits.
    //
    // Opens still come from the FileContentMapping walk because it picks up
    // nested-module opens that `collectFileDeclarations` doesn't see at the
    // top level.
    let fileDecls =
        parsedInputs
        |> Array.Parallel.mapi (fun idx (fileName, parsedInput) ->
            let fd = collectFileDeclarations idx fileName parsedInput
            let fileInProject : FileInProject = { Idx = idx; FileName = fileName; ParsedInput = parsedInput }
            let fileContentEntries = FileContentMapping.mkFileContent fileInProject

            let opensSet = System.Collections.Generic.HashSet<string>()
            let extraOpens = ResizeArray<LongIdent>()
            let toIdents (parts: string list) = parts |> List.map (fun s -> Ident(s, range0))

            let rec collectOpens entry =
                match entry with
                | FileContentEntry.OpenStatement path ->
                    let key = String.concat "." path
                    if path.Length > 0 && opensSet.Add(key) then
                        extraOpens.Add(toIdents path)
                | FileContentEntry.TopLevelNamespace(_, nested)
                | FileContentEntry.NestedModule(_, nested) ->
                    for n in nested do collectOpens n
                | _ -> ()

            for entry in fileContentEntries do
                collectOpens entry

            { fd with
                Opens = fd.Opens @ List.ofSeq extraOpens
                IdentifierRefs = collectFullPathRefs parsedInput })

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
/// Classify each entry in the export map by what kind of declaration produced
/// it. Used by the matching policy: when prefix-iterating a multi-segment
/// reference to find a cross-file dep, a `Module` prefix counts as a module
/// reference (legitimate dep), but a bare `Type` prefix is rejected unless
/// the trailing path resolves to a registered Member of that type.
type private ExportKind =
    | Module
    | Type
    | Value
    | Member

let private buildExportMap (fileDecls: FileDeclarations array) : Map<string, Set<int>> * Set<string> * Map<string, ExportKind> * Map<string, Set<int>> =
    let mutable exportMap = Map.empty<string, Set<int>>
    let mutable sharedPrefixes = Set.empty<string>
    let mutable kinds = Map.empty<string, ExportKind>
    // aliasMap is a SEPARATE resolution shortcut for [<AutoOpen>] modules.
    // It's never used for sharedPrefixes/kinds/Module-tracking — only as a
    // fallback when an ident ref doesn't resolve via exportMap. This keeps
    // the "what's actually declared" map clean from "what's reachable via
    // AutoOpen" so aliases can't create false sharedPrefix collisions or
    // change prefix-iteration behaviour.
    let mutable aliasMap = Map.empty<string, Set<int>>

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

    let addExportWithKind (name: string) (fileIdx: int) (kind: ExportKind) =
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
        // Kind: if a name is registered as multiple kinds, prefer Module over
        // others (a module can shadow nested type/value names in scope).
        match Map.tryFind name kinds, kind with
        | Some Module, _ -> ()
        | _, k -> kinds <- Map.add name k kinds

    let addAlias (name: string) (fileIdx: int) =
        let existing = aliasMap |> Map.tryFind name |> Option.defaultValue Set.empty
        aliasMap <- Map.add name (Set.add fileIdx existing) aliasMap

    for fd in fileDecls do
        for topMod in fd.TopLevelModules do
            // Register the full qualified module/namespace name
            let qualName = topMod.QualifiedName |> List.map (fun id -> id.idText) |> String.concat "."
            addExportWithKind qualName fd.FileIndex Module

            // Register each segment prefix for namespace resolution
            let segments = topMod.QualifiedName |> List.map (fun id -> id.idText)
            let mutable prefix = ""
            for seg in segments do
                prefix <- if prefix = "" then seg else prefix + "." + seg
                addExportWithKind prefix fd.FileIndex Module

            // For [<AutoOpen>] top-level NamedModule (e.g.
            // `[<AutoOpen>] module Suave.Sockets.AsyncSocket`), content is
            // reachable via the PARENT namespace path. So `transferStream`
            // (a let binding inside AsyncSocket) is visible after
            // `open Suave.Sockets` without further qualification.
            //
            // Compute the parent path (everything except the last segment).
            // If the topMod is at namespace root (e.g. `module Foo`), there's
            // no parent and AutoOpen has no effect on resolution.
            let topAlias =
                if topMod.IsAutoOpen
                   && topMod.Kind = SynModuleOrNamespaceKind.NamedModule
                   && segments.Length > 1 then
                    Some (segments |> List.take (segments.Length - 1) |> String.concat ".")
                else None

            // Register type names + their members, qualified by module
            for ty in topMod.Types do
                let tyQualName = qualName + "." + ty.Name.idText
                addExportWithKind tyQualName fd.FileIndex Type
                for memberName in ty.MemberNames do
                    addExportWithKind (tyQualName + "." + memberName.idText) fd.FileIndex Member
                match topAlias with
                | Some a ->
                    addAlias (a + "." + ty.Name.idText) fd.FileIndex
                    for memberName in ty.MemberNames do
                        addAlias (a + "." + ty.Name.idText + "." + memberName.idText) fd.FileIndex
                | None -> ()

            // Register module-level let-binding values
            for v in topMod.Values do
                addExportWithKind (qualName + "." + v.Name.idText) fd.FileIndex Value
                match topAlias with
                | Some a -> addAlias (a + "." + v.Name.idText) fd.FileIndex
                | None -> ()

            // Register nested module names + their content.
            //
            // [<AutoOpen>] handling: when a nested module has the AutoOpen
            // attribute, its content (Types, Values, Members, Module names)
            // is reachable through the parent without an explicit `open`. We
            // record this in `aliasMap` (separate from exportMap) so that
            // refs to `SocketBinding` from a file with `open Suave` can
            // fall back to `Suave.SocketBinding` resolution without the
            // alias polluting sharedPrefixes/kinds calculations.
            //
            // `aliasParent` is the namespace under which the contents of
            // this AutoOpen chain are reachable (always the topmost
            // non-AutoOpen ancestor, or `None` if no AutoOpen ancestor).
            let rec registerNested (parentName: string) (aliasParent: string option) (m: ModuleDeclStub) =
                let nestedName = parentName + "." + m.Name.idText
                addExportWithKind nestedName fd.FileIndex Module
                let registerWithAlias (suffix: string) (kind: ExportKind) =
                    addExportWithKind (nestedName + "." + suffix) fd.FileIndex kind
                    match aliasParent with
                    | Some a -> addAlias (a + "." + suffix) fd.FileIndex
                    | None -> ()
                for ty in m.Types do
                    registerWithAlias ty.Name.idText Type
                    for memberName in ty.MemberNames do
                        registerWithAlias (ty.Name.idText + "." + memberName.idText) Member
                for v in m.Values do
                    registerWithAlias v.Name.idText Value
                // Module names get aliased too (under aliasMap only). Lets
                // `Common.foo` resolve when there's `[<AutoOpen>] module
                // Suave.Utils { module Common = ... }` and the consumer has
                // `open Suave.Utils`.
                match aliasParent with
                | Some a -> addAlias (a + "." + m.Name.idText) fd.FileIndex
                | None -> ()
                let childAlias =
                    if m.IsAutoOpen then
                        match aliasParent with
                        | Some _ -> aliasParent
                        | None -> Some parentName
                    else None
                for nested in m.NestedModules do
                    registerNested nestedName childAlias nested

            for nested in topMod.NestedModules do
                let alias = if nested.IsAutoOpen then Some qualName else None
                registerNested qualName alias nested

    (exportMap, sharedPrefixes, kinds, aliasMap)

/// Add a dependency on a name, optionally skipping shared prefixes.
/// Looks up `name` in the main exportMap; if not found, falls back to
/// aliasMap (AutoOpen resolution shortcuts).
let private addDepFromExportMap
    (exportMap: Map<string, Set<int>>)
    (sharedPrefixes: Set<string>)
    (aliasMap: Map<string, Set<int>>)
    (skipShared: bool)
    (selfIndex: int)
    (deps: byref<Set<int>>)
    (name: string) =
    if skipShared && Set.contains name sharedPrefixes then ()
    else
        let primary = Map.tryFind name exportMap
        match primary with
        | Some fileIndices ->
            for idx in fileIndices do
                if idx <> selfIndex then
                    deps <- Set.add idx deps
        | None ->
            match Map.tryFind name aliasMap with
            | Some fileIndices ->
                if not (isNull (System.Environment.GetEnvironmentVariable "FSHARP_FILE_ORDER_AUTO_TRACE")) then
                    eprintfn "[file-order-auto] alias hit: %s -> %A from self=%d" name (Set.toList fileIndices) selfIndex
                for idx in fileIndices do
                    if idx <> selfIndex then
                        deps <- Set.add idx deps
            | None -> ()

/// Resolve a path (list of idents) against the export map.
///
/// Strategy: walk the segments left-to-right looking for the LONGEST prefix
/// that resolves to a Module/Value/Member entry. A bare Type prefix (with
/// trailing segments unaccounted for) is skipped — that pattern usually
/// means a member call where the member isn't registered (e.g. a
/// FSharp.Core method whose qualifier collides with a project type name).
/// The legacy short-prefix-iteration mode (used by Opens, where the full
/// path is known to terminate at a module) keeps the older behaviour.
let private resolvePathDeps
    (exportMap: Map<string, Set<int>>)
    (sharedPrefixes: Set<string>)
    (kinds: Map<string, ExportKind>)
    (aliasMap: Map<string, Set<int>>)
    (skipShared: bool)
    (prefixesToo: bool)
    (selfIndex: int)
    (deps: byref<Set<int>>)
    (path: LongIdent) =
    let segments = path |> List.map (fun id -> id.idText)
    let fullPath = String.concat "." segments
    // Always try the literal full path first.
    addDepFromExportMap exportMap sharedPrefixes aliasMap skipShared selfIndex &deps fullPath
    if prefixesToo then
        // Walk prefixes from longest (full path = handled above) to shortest,
        // accepting Module/Value/Member matches but rejecting bare Type
        // prefixes whose trailing segments aren't registered as members.
        let mutable prefixes : (string * int) list = []
        let mutable acc = ""
        let mutable i = 0
        for seg in segments do
            acc <- if acc = "" then seg else acc + "." + seg
            i <- i + 1
            prefixes <- (acc, i) :: prefixes
        // prefixes is now longest-first
        for (prefix, _len) in prefixes do
            // Skip the full path — already tried.
            if prefix <> fullPath then
                let kind = Map.tryFind prefix kinds
                match kind with
                | Some Module
                | Some Value
                | Some Member ->
                    addDepFromExportMap exportMap sharedPrefixes aliasMap skipShared selfIndex &deps prefix
                | Some Type ->
                    // Bare-type prefix match: only add the dep if the trailing
                    // segments represent a member that's registered (i.e. the
                    // full path matched above). They didn't, so skip — this is
                    // the FsCheck.Result/FSharp.Core.Result collision case.
                    ()
                | None -> ()

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
    (kinds: Map<string, ExportKind>)
    (aliasMap: Map<string, Set<int>>)
    (skipShared: bool)
    (prefixesToo: bool)
    (selfIndex: int)
    (enclosingPrefixes: string list list)
    (deps: byref<Set<int>>)
    (path: LongIdent) =
    // First: literal path resolution
    resolvePathDeps exportMap sharedPrefixes kinds aliasMap skipShared prefixesToo selfIndex &deps path

    // Then: try with each enclosing namespace prefix prepended.
    // For a ref `ForestMod.X` from a file in `CycleTest.TreeMod`, also try
    // `CycleTest.ForestMod.X` and `CycleTest.TreeMod.ForestMod.X`.
    let pathStrs = path |> List.map (fun id -> id.idText)
    for nsPrefix in enclosingPrefixes do
        let prefixed = nsPrefix @ pathStrs
        let prefixedPath = prefixed |> List.map (fun s -> Ident(s, range0))
        resolvePathDeps exportMap sharedPrefixes kinds aliasMap skipShared prefixesToo selfIndex &deps prefixedPath

/// Resolve a file's imports against the export map to find dependencies.
/// Opens always create dependencies (they're explicit imports).
/// IdentifierRefs skip shared namespace prefixes to avoid false cycles.
/// When includeIdentRefs is false, only Opens are used (fallback for cycle-prone projects).
let private resolveFileDependencies
    (exportMap: Map<string, Set<int>>)
    (sharedPrefixes: Set<string>)
    (kinds: Map<string, ExportKind>)
    (aliasMap: Map<string, Set<int>>)
    (includeIdentRefs: bool)
    (fd: FileDeclarations)
    : Set<int> =

    let mutable deps = Set.empty<int>
    let enclosingNs = getEnclosingPrefixes fd

    // Opens: match full path only (no prefix expansion), AND skip shared
    // prefixes. `open FsCheck` from a file already inside `namespace FsCheck`
    // would otherwise add every contributor as a dep — opens declare scope,
    // not specific deps. Identifier refs handle the actual cross-file links.
    for openPath in fd.Opens do
        resolvePathDepsWithPrefixes exportMap sharedPrefixes kinds aliasMap true false fd.FileIndex enclosingNs &deps openPath

    // For ident-ref resolution we also use `open` paths as additional
    // resolution prefixes — but ONLY for opens whose path itself names a
    // unique declaration (not a shared namespace). `open FsCheck.Internals`
    // where many files contribute would otherwise let any leftover ident
    // path-match against unrelated names from sibling files via the
    // shared parent. Limiting to non-shared opens means a ref like
    // `TypeClass.TypeClass` from a file with `open FsCheck.Internals`
    // can resolve via the [FsCheck; Internals] prefix without that prefix
    // being a wildcard for everything else.
    let openPrefixes =
        fd.Opens
        |> List.choose (fun lid ->
            let segs = lid |> List.map (fun (i: Ident) -> i.idText)
            let key = String.concat "." segs
            // Use opens that point at a known module/namespace as resolution
            // prefixes. This lets `TypeClass.TypeClass` from a file with
            // `open FsCheck.Internals` resolve to `FsCheck.Internals.TypeClass.TypeClass`.
            match Map.tryFind key kinds with
            | Some Module -> Some segs
            | _ -> None)

    // Collect names defined LOCALLY in this file (top-level + nested types and
    // modules). When an ident-ref's first segment matches a local name, we
    // suppress opens-as-prefix expansion for it: `Prop.safeForce` from inside
    // a file that has `open FsCheck.FSharp` AND a local `module Prop = ...`
    // refers to the local one, not the opened FsCheck.FSharp.Prop.
    let localNames =
        let acc = System.Collections.Generic.HashSet<string>()
        let rec visitMod (m: ModuleDeclStub) =
            acc.Add(m.Name.idText) |> ignore
            for ty in m.Types do acc.Add(ty.Name.idText) |> ignore
            for v in m.Values do acc.Add(v.Name.idText) |> ignore
            for nm in m.NestedModules do visitMod nm
        for tm in fd.TopLevelModules do visitMod tm
        acc

    if includeIdentRefs then
        let myName = (fd.FileName |> System.IO.Path.GetFileName |> string)
        let traceMe = myName = "Stream.fs" && not (isNull (System.Environment.GetEnvironmentVariable "FSHARP_FILE_ORDER_AUTO_TRACE"))
        for identRef in fd.IdentifierRefs do
            // Always try enclosing namespace prefixes.
            // Try opens-as-prefix only when the ref's first segment isn't
            // shadowed by a locally-defined name.
            let firstSeg =
                match identRef with
                | (i: Ident) :: _ -> i.idText
                | [] -> ""
            let isShadowed = firstSeg <> "" && localNames.Contains(firstSeg)
            let prefixes =
                if isShadowed then
                    enclosingNs
                else
                    (enclosingNs @ openPrefixes) |> List.distinct
            if traceMe && firstSeg = "transferStream" then
                eprintfn "[stream-trace] ref=%A shadowed=%b prefixes=%A" (identRef |> List.map (fun (i: Ident) -> i.idText)) isShadowed prefixes
            let before = deps
            resolvePathDepsWithPrefixes exportMap sharedPrefixes kinds aliasMap true true fd.FileIndex prefixes &deps identRef
            if traceMe && firstSeg = "transferStream" then
                let added = Set.difference deps before |> Set.toList
                eprintfn "[stream-trace] after resolve, added=%A" added

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

    let exportMap, sharedPrefixes, kinds, aliasMap = buildExportMap fileDecls

    let buildDeps (includeIdentRefs: bool) =
        fileDecls
        |> Array.map (fun fd ->
            if isAutoGeneratedFile fd then
                (fd.FileIndex, Set.empty)
            elif isSigFile fd.FileName then
                (fd.FileIndex, Set.empty)
            else
                (fd.FileIndex, resolveFileDependencies exportMap sharedPrefixes kinds aliasMap includeIdentRefs fd))
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
    let exportMap, sharedPrefixes, kinds, aliasMap = buildExportMap fileDecls

    // Build sig→impl redirect: when a file depends on a sig (e.g.
    // `Connection.fs` references `Suave.Runtime.SocketBinding` which is
    // declared in `Runtime.fsi`), we redirect that dep to the IMPL
    // (`Runtime.fs`). This way Tarjan places the impl at the right
    // topological position and the pair-rewriting step (which emits
    // `[sig; impl]` at the impl's position) preserves ordering for
    // consumers.
    let sigToImpl =
        let pairs = buildSigImplPairs fileDecls  // impl → sig
        pairs
        |> Map.toSeq
        |> Seq.map (fun (impl, sigIdx) -> sigIdx, impl)
        |> Map.ofSeq

    let redirectSig idx =
        match Map.tryFind idx sigToImpl with
        | Some implIdx -> implIdx
        | None -> idx

    let deps =
        fileDecls
        |> Array.map (fun fd ->
            if isAutoGeneratedFile fd then
                (fd.FileIndex, Set.empty)
            elif isSigFile fd.FileName then
                (fd.FileIndex, Set.empty)
            else
                let raw = resolveFileDependencies exportMap sharedPrefixes kinds aliasMap true fd
                let redirected =
                    raw
                    |> Set.map redirectSig
                    |> Set.filter (fun i -> i <> fd.FileIndex)
                (fd.FileIndex, redirected))
        |> Map.ofArray

    if not (isNull (System.Environment.GetEnvironmentVariable "FSHARP_FILE_ORDER_AUTO_TRACE")) then
        for fd in fileDecls do
            let nm = (fd.FileName |> System.IO.Path.GetFileName |> string)
            if nm = "Stream.fs" || nm = "Runtime.fs" || nm = "Connection.fs" then
                let d = Map.tryFind fd.FileIndex deps |> Option.defaultValue Set.empty
                let depNames = d |> Seq.map (fun i -> (fileDecls.[i].FileName |> System.IO.Path.GetFileName |> string)) |> String.concat ", "
                eprintfn "[file-order-auto] %s(idx=%d) deps: [%s]" nm fd.FileIndex depNames

    if not (isNull (System.Environment.GetEnvironmentVariable "FSHARP_FILE_ORDER_AUTO_TRACE")) then
        for fd in fileDecls do
            let nm = (fd.FileName |> System.IO.Path.GetFileName |> string)
            if nm = "Random.fs" || nm = "Testable.fs" then
                eprintfn "[file-order-auto] %s top-modules:" nm
                for tm in fd.TopLevelModules do
                    let qual = tm.QualifiedName |> List.map (fun (i: Ident) -> i.idText) |> String.concat "."
                    eprintfn "  Module %s (kind=%A)" qual tm.Kind
                    for ty in tm.Types do
                        let mems = ty.MemberNames |> List.map (fun (i: Ident) -> i.idText) |> String.concat ", "
                        eprintfn "    Type %s members=[%s]" ty.Name.idText mems
                    for v in tm.Values do
                        eprintfn "    Value %s" v.Name.idText
        eprintfn "[file-order-auto] FSharp.Gen.fs deps:"
        for KeyValue(idx, depSet) in deps do
            let nm = (fileDecls.[idx].FileName |> System.IO.Path.GetFileName |> string)
            if nm = "FSharp.Gen.fs" || nm = "Random.fs" then
                let depNames = depSet |> Seq.map (fun d -> (fileDecls.[d].FileName |> System.IO.Path.GetFileName |> string)) |> String.concat ", "
                eprintfn "  %s -> [%s]" nm depNames
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
                // Already pulled into a cycle group; drop the duplicate.
                []
            | SingleFile idx when Set.contains idx sigIndicesSet ->
                // Sig file alone (not in a cycle group). Skip — its impl
                // will pull it in at the impl's topo position. Consumers
                // that depend on the sig had their deps redirected to the
                // impl in the deps map (see computeCompilationUnits), so
                // their ordering against the pair is preserved.
                []
            | SingleFile idx ->
                match Map.tryFind idx sigImplPairs with
                | Some sigIdx when not (Set.contains sigIdx sigsInCycleGroups) ->
                    [ SingleFile sigIdx; SingleFile idx ]
                | _ -> [ u ]
            | CycleGroup _ -> [ u ])

    (autoGen @ withSigsRepositioned) |> List.toArray
