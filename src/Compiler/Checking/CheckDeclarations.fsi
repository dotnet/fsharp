// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckDeclarations

open System
open FSharp.Compiler.Diagnostics
open Internal.Utilities.Library
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Import
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

val AddLocalRootModuleOrNamespace: TcGlobals -> ImportMap -> range -> TcEnv -> ModuleOrNamespaceType -> TcEnv

val CreateInitialTcEnv:
    TcGlobals * ImportMap * range * assemblyName: string * (CcuThunk * string list * string list) list ->
        OpenDeclaration list * TcEnv

val AddCcuToTcEnv:
    TcGlobals *
    ImportMap *
    range *
    TcEnv *
    assemblyName: string *
    ccu: CcuThunk *
    autoOpens: string list *
    internalsVisibleToAttributes: string list ->
        OpenDeclaration list * TcEnv

type TopAttribs =
    { mainMethodAttrs: Attribs
      netModuleAttrs: Attribs
      assemblyAttrs: Attribs }

type ConditionalDefines = string list

val EmptyTopAttrs: TopAttribs

val CombineTopAttrs: TopAttribs -> TopAttribs -> TopAttribs

val TcOpenModuleOrNamespaceDecl:
    TcResultsSink -> TcGlobals -> ImportMap -> range -> TcEnv -> LongIdent * range -> TcEnv * OpenDeclaration list

val AddLocalSubModule:
    g: TcGlobals -> amap: ImportMap -> m: range -> env: TcEnv -> moduleEntity: ModuleOrNamespace -> TcEnv

val CheckOneImplFile:
    TcGlobals *
    ImportMap *
    CcuThunk *
    OpenDeclaration list *
    (unit -> bool) *
    ConditionalDefines option *
    TcResultsSink *
    bool *
    TcEnv *
    ModuleOrNamespaceType option *
    ParsedImplFileInput *
    FSharpDiagnosticOptions ->
        Cancellable<TopAttribs * CheckedImplFile * TcEnv * bool * ModuleOrNamespaceType>

/// Checks one declaration of a module or namespace that is not part of a recursive group
val TcModuleOrNamespaceElementNonMutRec:
    cenv: TcFileState ->
    parent: ParentRef ->
    typeNames: Set<string> ->
    scopem: range ->
    env: TcEnv ->
    synDecl: SynModuleDecl ->
        Cancellable<
            (ModuleOrNamespaceContents list * (unit -> unit) list * (AttributeTargets * Attrib) list) * TcEnv * TcEnv
         >

val CreateImplFileCheckState:
    TcGlobals *
    ImportMap *
    CcuThunk *
    ConditionalDefines option *
    TcResultsSink *
    bool *
    TcEnv *
    ModuleOrNamespaceType option *
    ParsedImplFileInput *
    FSharpDiagnosticOptions ->
        TcFileState * TcEnv * ModuleOrNamespaceType ref

val CheckImplFileTail:
    cenv: TcFileState ->
    checkForErrors: (unit -> bool) ->
    env: TcEnv ->
    rootSigOpt: ModuleOrNamespaceType option ->
    synImplFile: ParsedImplFileInput ->
    moduleTyAcc: ModuleOrNamespaceType ref ->
    ModuleOrNamespaceContents * (AttributeTargets * Attrib) list * TcEnv ->
        TopAttribs * CheckedImplFile * TcEnv * bool * ModuleOrNamespaceType

/// The defaults CheckImplFileTail applies to the inference variables left unsolved in the contents and attributes.
val ApplyDefaults:
    cenv: TcFileState ->
    g: TcGlobals ->
    denvAtEnd: FSharp.Compiler.TypedTreeOps.MemberRepresentation.DisplayEnv ->
    m: range ->
    moduleContents: ModuleOrNamespaceContents ->
    extraAttribs: Attribs ->
        unit

/// What CheckImplFileTail solves, after its defaults, of the inference variables left in the contents and attributes.
val SolveInternalUnknowns:
    g: TcGlobals ->
    cenv: TcFileState ->
    denvAtEnd: FSharp.Compiler.TypedTreeOps.MemberRepresentation.DisplayEnv ->
    moduleContents: ModuleOrNamespaceContents ->
    extraAttribs: Attribs ->
        unit

[<Sealed; NoEquality; NoComparison>]
type ImplFileScopeKind

[<NoEquality; NoComparison>]
type ImplFileDeclarationScope =
    { Parent: ParentRef
      Env: TcEnv
      TypeNames: Set<string>
      ChecksDeclarations: bool
      Kind: ImplFileScopeKind }

val EnterImplFileDeclarations:
    cenv: TcFileState ->
    env: TcEnv ->
    synImplFile: ParsedImplFileInput ->
    splitFragment: bool ->
        ImplFileDeclarationScope

val ExitImplFileDeclarations:
    cenv: TcFileState ->
    env: TcEnv ->
    openDecls0: OpenDeclaration list ->
    scope: ImplFileDeclarationScope ->
    scopeContents: ModuleOrNamespaceType ->
    compiledDefs: (ModuleOrNamespaceContents list * (unit -> unit) list * (AttributeTargets * Attrib) list) list ->
    envAtEnd: TcEnv ->
        ModuleOrNamespaceContents * (AttributeTargets * Attrib) list * TcEnv

/// The type names of a module's or namespace's declarations, which give a module of the same name among them the
/// Module suffix
val TypeNamesOfDeclarations: cenv: TcFileState -> env: TcEnv -> decls: SynModuleDecl list -> Set<string>

/// Sets up checking the declarations of a non-recursive 'module M = ...' one at a time: the module, and the
/// environment, parent and type names its declarations are checked in. None when that fails, after reporting it.
val EnterNestedModuleDeclarations:
    cenv: TcFileState ->
    parent: ParentRef ->
    typeNames: Set<string> ->
    env: TcEnv ->
    synDecl: SynModuleDecl ->
        (ModuleOrNamespace * TcEnv * ParentRef * Set<string>) option

/// Finishes a non-recursive 'module M = ...' whose declarations were checked one at a time: gives the module
/// moduleContents as its type, publishes it into env's accumulator and adds it to env.
val ExitNestedModuleDeclarations:
    cenv: TcFileState ->
    scopem: range ->
    env: TcEnv ->
    m: range ->
    moduleEntity: ModuleOrNamespace ->
    moduleContents: ModuleOrNamespaceType ->
    compiledDefs: (ModuleOrNamespaceContents list * (unit -> unit) list * (AttributeTargets * Attrib) list) list ->
    envAtEnd: TcEnv ->
        (ModuleOrNamespaceContents list * (unit -> unit) list * (AttributeTargets * Attrib) list) * TcEnv * TcEnv

/// Adds to env what checking synDecl in scope scopem adds to the environment after it, taking what it defines from
/// defs (the values and entities that check published) instead of checking it again. Opens and module abbreviations
/// are resolved again in env. For name resolution only: it reports nothing and publishes nothing. Not for the header
/// of a module whose declarations are checked one at a time.
val ExtendEnvWithDeclaration:
    cenv: TcFileState -> scopem: range -> env: TcEnv -> synDecl: SynModuleDecl -> defs: ModuleOrNamespaceType -> TcEnv

val CheckOneSigFile:
    TcGlobals *
    ImportMap *
    CcuThunk *
    (unit -> bool) *
    ConditionalDefines option *
    TcResultsSink *
    bool *
    FSharpDiagnosticOptions ->
        TcEnv ->
        ParsedSigFileInput ->
            Cancellable<TcEnv * ModuleOrNamespaceType * bool>

exception NotUpperCaseConstructor of range: range

exception NotUpperCaseConstructorWithoutRQA of range: range
