// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis.Declarations

open System
open System.Collections.Generic
open System.IO
open System.Runtime.CompilerServices
open System.Threading

open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.EditorServices
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

[<RequireQualifiedAccess>]
type DeclarationKind =
    | Value
    | Type
    | Exception
    | Module
    | Open
    | ModuleAbbrev
    | Attribute
    | Do
    | RecGroup
    | Augmentation

type DeclarationKey =
    {
        /// The file's module or namespace, then the nested modules the declaration is in
        ModulePath: string list
        Name: string
        Kind: DeclarationKind
        /// The number of earlier declarations of the file with the same module path, name and kind
        Ordinal: int
    }

/// A declaration of the project that a typed use resolves to: the declaration with Key in File. Key is None for what a
/// signature file declares, which is checked whole: a use of it depends on that file's text.
type DeclarationRef =
    {
        File: string
        Key: DeclarationKey option
    }

/// The names a declaration defines and uses, from its syntax. A use can resolve to a definition through any of the
/// lists: Defines holds every kind of name, so a dependency on a name is a dependency on every declaration defining it.
type DeclarationContent =
    {
        /// Values, types, union cases, record fields, members, custom operation names and modules, including those
        /// inside a nested module or namespace fragment the declaration holds whole
        Defines: string[]
        /// Names resolved in the environment: unqualified identifiers, every segment of qualified ones, types, attributes
        /// (with and without the Attribute suffix) and opened modules. Locally bound names are included.
        Uses: string[]
        /// Names after a dot and members constructs call implicitly (computation expression builders, enumerators,
        /// indexers, trait calls): each may be a member or extension of any type
        MemberUses: string[]
        /// Record labels and union cases, including names after a dot, which can be record fields
        FieldOrCaseUses: string[]
        /// Something resolves to a name the syntax does not show, such as the `?` operator: the declaration depends on
        /// every earlier declaration
        HasUnresolvable: bool
        /// Every later declaration depends on this one: open, an [&lt;AutoOpen&gt;] module, a module abbreviation,
        /// a module-level do, an attribute declaration, a hash directive, or a custom operation whose name is unknown.
        /// #nowarn and #warnon are not declarations: lexing turns them into warning scopes by position, and FinishFile
        /// filters every declaration's diagnostics with the scopes of the parse it is given, so they need no dependency.
        AffectsAllLater: bool
        /// The names, as written, of the types the declaration augments (`type T with ...`; a group `type A with ... and
        /// B with ...` augments both), sorted; empty for anything else. Checking an intrinsic augmentation adds its
        /// members to T's entity, which T's result holds, so T's declaration and its augmentations re-check together,
        /// T first, whenever any of them re-checks: re-checking T gives a new entity, which only the augmentations
        /// checked after it extend.
        Augments: string[]
    }

/// A top-level declaration of an implementation file, or a declaration of a non-recursive nested module, or the
/// header of such a module (its attributes and name, which the module's declarations are checked inside).
[<Sealed>]
type ParsedDeclaration
    internal
    (
        key: DeclarationKey,
        decl: SynModuleDecl,
        range: range,
        scopem: range,
        text: string,
        headerInputs: string,
        within: DeclarationKey option,
        siblings: SynModuleDecl list,
        depth: int,
        isModuleHeader: bool,
        source: ISourceText
    ) =
    member _.Key = key
    /// The declaration's range; for a module header, from its start to the '=' after the module's name.
    member _.Range = range
    /// A hash of the declaration's source text. Unequal hashes mean the text changed; equal ones do not prove
    /// it is the same, so compare Text for that.
    member _.SyntaxHash = hash text
    /// The declaration's source text, from the start of its range to its end. Text alone does not decide a module
    /// header is unchanged: use IsUnchangedFrom, which also compares HeaderInputs.
    member _.Text = text
    /// For a module, what checking it reads beyond its text: whether a type of the same name is among the
    /// declarations around it (which gives the module the Module suffix), and for a module header the type names of
    /// its body (which its declarations, and the modules among them, are checked with). Empty for other declarations.
    member _.HeaderInputs = headerInputs
    /// The key of the module header this declaration is directly inside, if any. A re-checked header gives the module
    /// a new entity, the parent of its declarations' values, so every declaration under it must be checked again:
    /// an edge from the header to them with no fingerprint cutoff.
    member _.Within = within

    /// Whether checking this declaration reads the same syntax as checking the other: same Text and HeaderInputs.
    member _.IsUnchangedFrom(other: ParsedDeclaration) =
        String.Equals(text, other.Text, StringComparison.Ordinal)
        && String.Equals(headerInputs, other.HeaderInputs, StringComparison.Ordinal)

    /// The whole declaration; for a module header, the whole module.
    member internal _.Syntax = decl
    /// The range the declaration's definitions are in scope for, as TcModuleOrNamespaceElementsNonMutRec gives it.
    member internal _.ScopeRange = scopem
    /// How many nested modules the declaration is in, below the file's scope
    member internal _.Depth = depth
    member internal _.IsModuleHeader = isModuleHeader
    /// The declarations of the enclosing module or namespace, this one included: a module's suffix reads their types
    member internal _.Siblings = siblings
    /// The source text of the parse this declaration was split from.
    member internal _.Source = source

/// A scoped `before` was refused because the dependency snapshot it would be checked against is not current for the
/// file: FileDependencies has not been given this parse with the latest parse of every earlier file. Checking again
/// with current dependencies, or every earlier file, is accepted.
type StaleDependenciesException(message: string) =
    inherit ArgumentException(message, "before")

/// A check was refused because the process's #line directives of the file are not its parse's: a newer parse of
/// another text of the file replaced them. The check is right once they are its parse's again.
type SupersededLineDirectivesException(message: string) =
    inherit InvalidOperationException(message)

type internal CompilerState =
    {
        tcConfig: TcConfig
        tcGlobals: TcGlobals
        tcImports: TcImports
        tcInitialState: TcState
        projectOptions: FSharpProjectOptions
    }

[<AutoOpen>]
module internal CompilerStateInit =

    let userOpName = "Unknown"
    let suggestNamesForErrors = true
    let captureIdentifiersWhenParsing = false

    let initializeCompilerStateWith
        (projectReferences: IProjectReference list)
        (projectOptions: FSharpProjectOptions)
        (reset: unit -> unit)
        =
        async {
            let tcConfig =
                let tcConfigB =
                    TcConfigBuilder.CreateNew(
                        SimulatedMSBuildReferenceResolver.getResolver (),
                        defaultFSharpBinariesDir = FSharpCheckerResultsSettings.defaultFSharpBinariesDir,
                        reduceMemoryUsage = ReduceMemoryFlag.Yes,
                        implicitIncludeDir = !!Path.GetDirectoryName(projectOptions.ProjectFileName),
                        isInteractive = false,
                        isInvalidationSupported = true,
                        defaultCopyFSharpCore = CopyFSharpCoreFlag.No,
                        tryGetMetadataSnapshot = (fun _ -> None),
                        sdkDirOverride = None,
                        rangeForErrors = range0
                    )
                // Before the -r: options are applied: each one takes the project reference of its path
                tcConfigB.projectReferences <- projectReferences
                let sourceFiles = projectOptions.SourceFiles |> Array.toList
                let argv = projectOptions.OtherOptions |> Array.toList
                let _sourceFiles = ApplyCommandLineArgs(tcConfigB, sourceFiles, argv)
                TcConfig.Create(tcConfigB, validate = false)

            let tcConfigP = TcConfigProvider.Constant(tcConfig)

            let dependencyProvider = new DependencyProvider()
            let! tcGlobals, tcImports = TcImports.BuildTcImports(tcConfigP, dependencyProvider)

#if !NO_TYPEPROVIDERS
            // Handle type provider invalidation by resetting compiler state
            tcImports.GetCcusExcludingBase()
            |> Seq.iter (fun ccu -> ccu.Deref.InvalidateEvent.Add(fun _ -> reset ()))
#else
            ignore reset
#endif

            // The name the build gives the assembly (<AssemblyName>, by -o:): InternalsVisibleTo reads it, and references resolve by it
            let assemblyName =
                match tcConfig.outputFile with
                | Some(output: string) -> !!Path.GetFileNameWithoutExtension(output)
                | None -> !!Path.GetFileNameWithoutExtension(projectOptions.ProjectFileName)

            let tcInitial, openDecls0 =
                GetInitialTcEnv(assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)

            let tcInitialState =
                GetInitialTcState(rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, tcInitial, openDecls0)

            return
                {
                    tcConfig = tcConfig
                    tcGlobals = tcGlobals
                    tcImports = tcImports
                    tcInitialState = tcInitialState
                    projectOptions = projectOptions
                }
        }

    let initializeCompilerState (projectOptions: FSharpProjectOptions) reset =
        initializeCompilerStateWith [] projectOptions reset

    /// Resets the compiler-generated names of a file from zero again, before checking it, so a file checked again in
    /// this process, as in watch mode, gets the names a fresh compilation gives instead of continuing from its last check.
    let ResetGeneratedNames (input: ParsedInput, compilerState: CompilerState) =
        compilerState.tcGlobals.CompilerGlobalState
        |> Option.iter (fun state -> state.ResetCompilerGeneratedNamesOfFile input.QualifiedName.Range.FileIndex)

/// What FileDependencies computed from one parse of every file of the project.
[<NoEquality; NoComparison>]
type internal DependencySnapshot =
    {
        Parses: FSharpParseFileResults[]
        Dependencies: int[][]
        /// The module-name dictionary each file is deduplicated against: every earlier file's entries, as checking the
        /// whole project gives them
        ModuleNamesBefore: ModuleNamesDict[]
        /// The signature files whose implementation file has the same qualified name, both deduplicated: checking the
        /// implementation file gives it that signature, so a setup can stand the signature in for it
        MatchedSignatures: HashSet<string>
    }

/// A checker state some setup reached: the initial state, with the steps on the path to it applied in order. The state
/// after one more step is kept while the step's key (the FileSignature it applies) is alive.
[<Sealed>]
type internal SetupNode(state: TcState) =
    let after = ConditionalWeakTable<obj, SetupNode>()

    member _.State = state

    member _.After(key: obj, step: TcState -> TcState) : SetupNode =
        after.GetValue(key, ConditionalWeakTable<obj, SetupNode>.CreateValueCallback(fun _ -> SetupNode(step state)))

/// What the checks of a project share that does not reach its parses, so a lazily built result can hold it: the
/// compiler state, a lock per file, the setups reached, and the test counter.
[<Sealed>]
type internal ProjectChecks(state: CompilerState) =
    let fileLocks =
        System.Collections.Concurrent.ConcurrentDictionary<string, obj>(StringComparer.OrdinalIgnoreCase)

    let latestParses =
        System.Collections.Concurrent.ConcurrentDictionary<string, FSharpParseFileResults>(StringComparer.OrdinalIgnoreCase)

    let setupRoot = SetupNode state.tcInitialState
    let mutable checkResultsBuilt = 0

    let fileIndexes =
        let indexes = Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)

        state.projectOptions.SourceFiles
        |> Array.iteri (fun idx fileName -> indexes[fileName] <- idx)

        indexes

    member _.State = state

    /// The initial checker state, from which each setup's steps, in file order, lead to the states setups reached
    member _.SetupRoot = setupRoot

    /// The project file's index, or -1.
    member _.IndexOf(fileName: string) =
        match fileIndexes.TryGetValue fileName with
        | true, idx -> idx
        | _ -> -1

    /// Whether `fileName` is a project file that comes before `other`.
    member _.IsBefore(fileName: string, other: string) =
        match fileIndexes.TryGetValue fileName, fileIndexes.TryGetValue other with
        | (true, a), (true, b) -> a < b
        | _ -> false

    /// Held from resetting a file's compiler-generated-name counters through the check that generates its names:
    /// the counters are the project's, by file, so two checks of one file must not interleave there. Checks of
    /// different files do not wait for each other.
    member _.FileLock(fileName: string) =
        fileLocks.GetOrAdd(fileName, fun _ -> obj ())

    /// The latest parse Parse made of each file, set while the file's parse lock is held.
    member _.SetLatestParse(parse: FSharpParseFileResults) = latestParses[parse.FileName] <- parse

    member _.LatestParse(fileName: string) =
        match latestParses.TryGetValue fileName with
        | true, parse -> Some parse
        | _ -> None

    member _.CheckResultsBuilt = Volatile.Read &checkResultsBuilt

    member _.CountCheckResultsBuilt() =
        Interlocked.Increment &checkResultsBuilt |> ignore

/// A project's options, references and initial checking state.
[<Sealed>]
type ProjectContext internal (state: CompilerState) =
    let parseLocks =
        System.Collections.Concurrent.ConcurrentDictionary<string, obj>(StringComparer.OrdinalIgnoreCase)
    // Exactly as ErrorsByFile matches a diagnostic's file against the project's files
    let sourceFiles =
        HashSet<string>(state.projectOptions.SourceFiles, StringComparer.Ordinal)

    let fileIndexes =
        let indexes = Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)

        state.projectOptions.SourceFiles
        |> Array.iteri (fun idx fileName -> indexes[fileName] <- idx)

        indexes

    let mutable dependencies: DependencySnapshot option = None
    let checks = ProjectChecks state
    member internal _.State = state
    member internal _.Checks = checks

    /// For tests: how many times CheckFile's results on this context built a name-resolution sink and check results.
    member _.CheckResultsBuilt = checks.CheckResultsBuilt

    /// Runs `f` holding the file's check lock, as a declaration check does. For tests.
    member _.HoldingCheckLock(fileName: string, f: unit -> 'T) : 'T = lock (checks.FileLock fileName) f

    /// For tests: when set, DeclarationChecker.Shift refuses every declaration of this context, so its callers check
    /// them again.
    member val RefuseShifts = false with get, set

    /// Runs with the reason each time DeclarationChecker.Shift refuses a declaration of this context whose text is
    /// the same at another line. For tests and diagnosis.
    member val ShiftRefusedHook: string -> unit = ignore with get, set

    /// How many checks the finish ran for shifted results of any context in the process reported at a range of their
    /// file outside their declaration (see Leftovers.Shifted). For tests, which require 0.
    static member ShiftFinishMisses = DeclarationShift.FinishMisses()

    /// Held while a file is parsed and its #nowarn scopes, which the parse records in the project's options, are
    /// copied: parses of one file serialize, parses of different files do not.
    member internal _.ParseLock(fileName: string) =
        parseLocks.GetOrAdd(fileName, fun _ -> obj ())

    member internal _.IsSourceFile(fileName: string) = sourceFiles.Contains fileName

    member internal _.IndexOf(fileName: string) =
        match fileIndexes.TryGetValue fileName with
        | true, idx -> Some idx
        | _ -> None

    member internal _.SourceFiles = state.projectOptions.SourceFiles

    /// The latest parse Parse made of each file, set while the file's parse lock is held.
    member internal _.SetLatestParse(parse: FSharpParseFileResults) = checks.SetLatestParse parse

    member internal _.LatestParse(fileName: string) = checks.LatestParse fileName

    /// What the latest FileDependencies computed. A reference, so reads and writes are atomic.
    member internal _.Dependencies
        with get () = dependencies
        and set (snapshot: DependencySnapshot option) = dependencies <- snapshot

/// What no print of a signature shows: whether its types link to its own binders and entities.
module internal SignatureScope =

    /// A rigid type variable used outside the binder that scopes it, and an entity of the file that is not the
    /// signature's own, each as a line. An inference variable error recovery left free is not one.
    let outOfScope (fileName: string) (mty: ModuleOrNamespaceType) : string[] =
        let own = HashSet<Stamp>()

        let rec addOwn (m: ModuleOrNamespaceType) =
            for e in m.AllEntities do
                own.Add e.Stamp |> ignore

                if e.IsModuleOrNamespace then
                    addOwn e.ModuleOrNamespaceType

        addOwn mty
        let problems = ResizeArray<string>()

        let rec ty (where: string) (scope: Typar list) (t: TType) =
            match stripTyparEqns t with
            | TType_var(tp, _) -> typar where scope tp
            | TType_app(tcref, args, _) ->
                entity where tcref
                List.iter (ty where scope) args
            | TType_fun(a, b, _) ->
                ty where scope a
                ty where scope b
            | TType_tuple(_, args)
            | TType_anon(_, args) -> List.iter (ty where scope) args
            | TType_forall(tps, body) ->
                let inner = tps @ scope
                binders where inner tps
                ty where inner body
            | TType_measure m -> measure where scope m
            | TType_ucase(ucref, args) ->
                entity where ucref.TyconRef
                List.iter (ty where scope) args

        and measure where scope (m: Measure) =
            match stripUnitEqns m with
            | Measure.Var tp -> typar where scope tp
            | Measure.Const(tcref, _) -> entity where tcref
            | Measure.Prod(a, b, _) ->
                measure where scope a
                measure where scope b
            | Measure.Inv a
            | Measure.RationalPower(a, _) -> measure where scope a
            | Measure.One _ -> ()

        and typar where scope (tp: Typar) =
            if
                tp.Rigidity = TyparRigidity.Rigid
                && not (scope |> List.exists (fun s -> obj.ReferenceEquals(s, tp)))
            then
                problems.Add $"%s{where}: %s{tp.DisplayName}#%d{tp.Stamp} is out of scope"

        and entity where (tcref: TyconRef) =
            if
                tcref.IsLocalRef
                && String.Equals(tcref.Deref.Range.FileName, fileName, StringComparison.OrdinalIgnoreCase)
                && not (own.Contains tcref.Deref.Stamp)
            then
                problems.Add $"%s{where}: %s{tcref.DisplayName}#%d{tcref.Deref.Stamp} is not the signature's own"

        and binders where scope (tps: Typar list) =
            for tp in tps do
                for c in tp.Constraints do
                    let inConstraint = $"%s{where}, a constraint of %s{tp.DisplayName}"

                    match c with
                    | TyparConstraint.CoercesTo(t, _)
                    | TyparConstraint.DefaultsTo(_, t, _)
                    | TyparConstraint.IsEnum(t, _) -> ty inConstraint scope t
                    | TyparConstraint.IsDelegate(a, b, _) ->
                        ty inConstraint scope a
                        ty inConstraint scope b
                    | TyparConstraint.SimpleChoice(ts, _) -> List.iter (ty inConstraint scope) ts
                    | TyparConstraint.MayResolveMember(TTrait(supportTys, _, _, argTys, returnTy, _, _, _), _) ->
                        List.iter (ty inConstraint scope) supportTys
                        List.iter (ty inConstraint scope) argTys
                        Option.iter (ty inConstraint scope) returnTy
                    | _ -> ()

        let rec walk (m: ModuleOrNamespaceType) =
            for v in m.AllValsAndMembers do
                let where = $"val %s{v.LogicalName}"
                ty where [] v.Type

                let scope =
                    match stripTyparEqns v.Type with
                    | TType_forall(tps, _) -> tps
                    | _ -> []

                for TSlotSig(_, _, ctps, mtps, paramTys, returnTy) in v.ImplementedSlotSigs do
                    let inner = ctps @ mtps @ scope
                    let inSlot = where + ", a slot it implements"
                    binders inSlot inner (ctps @ mtps)

                    for group in paramTys do
                        for TSlotParam(_, paramTy, _, _, _, _) in group do
                            ty inSlot inner paramTy

                    Option.iter (ty inSlot inner) returnTy

            for e in m.AllEntities do
                let where = $"type %s{e.LogicalName}"
                let scope = e.Typars
                binders where scope scope

                if e.IsModuleOrNamespace then
                    walk e.ModuleOrNamespaceType
                else
                    for f in e.AllFieldsArray do
                        ty $"%s{where}, field %s{f.LogicalName}" scope f.FormalType

                    if e.IsUnionTycon then
                        for case in e.UnionCasesArray do
                            for f in case.RecdFieldsArray do
                                ty $"%s{where}, case %s{case.DisplayName}" scope f.FormalType

                    Option.iter (ty $"%s{where}, its abbreviation" scope) e.TypeAbbrev

        walk mty
        problems.ToArray()

    /// The type parameters of every value, in the order the signature holds them
    let typarNames (mty: ModuleOrNamespaceType) : string[] =
        let rec walk (m: ModuleOrNamespaceType) =
            [
                for v in m.AllValsAndMembers do
                    for tp in v.Typars do
                        yield $"%s{v.LogicalName}: %s{tp.DisplayName}"

                for e in m.AllEntities do
                    if e.IsModuleOrNamespace then
                        yield! walk e.ModuleOrNamespaceType
            ]

        Array.ofList (walk mty)

/// What a file adds to the state later files are checked in.
[<Sealed>]
type FileSignature
    internal
    (
        fileName: string,
        apply: TcState -> TcState,
        moduleNamesDict: ModuleNamesDict,
        definitions: Map<Stamp, DeclarationRef>,
        fingerprint: Lazy<string>,
        moduleType: ModuleOrNamespaceType,
        asImplementation: (TcState -> TcState) option,
        failed: bool,
        uses: (struct (Item * ItemOccurrence * range))[],
        semanticClassification: SemanticClassificationItem[]
    ) =
    /// What the files after this one see of it, as the file-level SignatureFingerprint describes it: for an
    /// implementation file with a signature file, the signature file's. Unequal for anything not describable. A check
    /// told nothing reads it takes it on first read.
    member _.Fingerprint = fingerprint.Value
    /// Whether the fingerprint has been taken: comparing it otherwise takes it.
    member _.HasFingerprint = fingerprint.IsValueCreated
    member internal _.FileName = fileName
    member internal _.Apply tcState = apply tcState
    /// The module-name dictionary DeduplicateParsedInputModuleName gave for this file: every earlier file's entries
    /// and its own. BeginFile merges these over the earlier files.
    member internal _.ModuleNamesDict = moduleNamesDict
    /// The declaration defining each entity and value later files see of this file, by stamp
    member internal _.Definitions = definitions
    /// What a signature file's check resolved, as a sink held it: nothing else of the sink is kept
    member internal _.Uses = uses
    /// Semantic classification items captured during signature checking; empty for an implementation file's signature.
    member _.SemanticClassification: SemanticClassificationItem[] = semanticClassification
    /// The module type the file adds to the checker state, for tests
    member internal _.ModuleType = moduleType

    /// For tests: every type variable or entity the signature's types use that it does not itself hold. Empty for a
    /// sound signature, as no print can tell.
    member _.OutOfScope() =
        SignatureScope.outOfScope fileName moduleType

    /// For tests: the names of the type parameters of the signature's values.
    member _.TyparNames() = SignatureScope.typarNames moduleType
    /// For a signature file: what graph-mode checking's ArtificialImplFile node does, adding the signature's contents
    /// to the environment implementation files are checked in. A file set up from a signature file without the
    /// implementation file behind it sees the signature there, as in graph mode.
    member internal _.AsImplementation = asImplementation
    /// Whether the implementation file's check failed with an error it recovered from (as CheckOneInput's
    /// PartialResultOnError): the file then adds nothing to the files after it, its signature file's contents
    /// included, so a file set up from that signature file sees it only once the implementation file is in its setup.
    member _.Failed = failed
    /// Stands for the AsImplementation step among a setup's steps, as the FileSignature itself stands for Apply
    member val internal ArtificialKey = obj ()

type internal CheckedDeclaration = ModuleOrNamespaceContents list * (unit -> unit) list * (AttributeTargets * Attrib) list

/// Keeps every diagnostic as reported, before the warning options and #nowarn scopes filter it, and passes it on.
/// The #nowarn scopes come from lexing the file: FinishFile filters what was kept again with those of the parse it
/// is given.
type internal RawDiagnostics(next: DiagnosticsLogger) =
    inherit DiagnosticsLogger("RawDiagnostics")
    let raw = ResizeArray<PhasedDiagnostic>()

    override _.DiagnosticSink diagnostic =
        raw.Add diagnostic
        next.DiagnosticSink diagnostic

    override _.ErrorCount = next.ErrorCount
    override _.CheckForRealErrorsIgnoringWarnings = next.CheckForRealErrorsIgnoringWarnings
    member _.Diagnostics = raw.ToArray()

/// A non-recursive nested module whose declarations are checked one at a time.
[<NoEquality; NoComparison>]
type internal ModuleFrame =
    {
        /// None when setting up the module failed, which skips its declarations
        Entity: ModuleOrNamespace option
        /// The module declaration's scope in its parent, and its range
        ScopeRange: range
        Range: range
        /// The environment, parent and type names the module's header was checked in
        OuterEnv: TcEnv
        OuterParent: ParentRef
        OuterTypeNames: Set<string>
        /// What the module's declarations checked so far left, latest first
        Checked: CheckedDeclaration list
    }

/// The environment a declaration is checked in.
[<Sealed>]
type DeclarationEnvironment
    internal (env: TcEnv, parent: ParentRef, typeNames: Set<string>, frames: ModuleFrame list, definitions: Map<Stamp, DeclarationRef>) =
    member internal _.Env = env
    member internal _.Parent = parent
    member internal _.TypeNames = typeNames
    /// The nested modules the environment is inside, innermost first
    member internal _.Frames = frames
    /// The declaration defining each entity and value of the project the environment holds, by stamp: those of
    /// earlier files, and of every declaration the environment was built from, including earlier versions of them
    member internal _.Definitions = definitions

/// Where a result goes when the file is built from the results.
[<NoEquality; NoComparison>]
type internal Nesting =
    /// A declaration, in as many nested modules
    | InModules of depth: int
    /// The header of a nested module that is in as many nested modules
    | ModuleHeader of depth: int * frame: ModuleFrame

    member x.Depth =
        match x with
        | InModules depth
        | ModuleHeader(depth, _) -> depth

/// How to undo what the latest finish of a file changed in its declarations' typed trees, or null once undone.
[<Sealed>]
type internal FinishUndo() =
    member val Log: ResizeArray<unit -> unit> | null = null with get, set

/// What checking left in cenv that CheckImplFileTail reads, to replay into the finish's own cenv.
[<Sealed>]
type internal Leftovers
    (
        preDefaults: (unit -> unit)[],
        final: (unit -> unit)[],
        extraCxs: (Stamp * (TraitConstraintInfo * range)) list,
        unionsWithDeferredAttributes: Set<Stamp>,
        deferredUnionNullnessChecks: (TType * range) list,
        createsGeneratedProvidedTypes: bool,
        namedDebugPoints: KeyValuePair<NamedDebugPointKey, range>[],
        /// How far the checks' own ranges are from where the check left them: the file, the declaration's range and
        /// the end of its scope as the check left them, and the lines moved over all shifts. The closures stay the
        /// check's, so a result shifted many times runs each with one wrapper.
        moved: struct (int * range * pos * int) voption
    ) =
    /// The check run with what it reports, and what it raises, moved.
    static member private MovedCheck(struct (fileIndex, range, scopeEnd, delta), check: unit -> unit) =
        fun () ->
            let s = DeclarationShift.Shifter.Create(fileIndex, range, scopeEnd, delta)

            try
                try
                    use _ =
                        UseTransformedDiagnosticsLogger(fun next -> DeclarationShift.ShiftingLogger(next, s.Diagnostic))

                    check ()
                with e ->
                    match s.Exn e with
                    | same when obj.ReferenceEquals(same, e) -> reraise ()
                    | movedExn -> raise movedExn
            finally
                if s.Refused.IsSome then
                    DeclarationShift.CountFinishMiss()

    static member Of(cenv: TcFileState) =
        let css = cenv.css

        Leftovers(
            css.PostInferenceChecksPreDefaults.ToArray(),
            css.PostInferenceChecksFinal.ToArray(),
            // Oldest first for each type variable, so adding them again keeps their order
            css.ExtraCxs.Fold (fun stamp cx acc -> (stamp, cx) :: acc) [],
            css.UnionsWithDeferredAttributes,
            css.DeferredUnionNullnessChecks,
            cenv.createsGeneratedProvidedTypes,
            Array.ofSeq cenv.namedDebugPointsForInlinedCode,
            ValueNone
        )

    member _.Apply(into: TcFileState) =
        let run (check: unit -> unit) =
            match moved with
            | ValueSome offset -> Leftovers.MovedCheck(offset, check)
            | ValueNone -> check

        into.css.PostInferenceChecksPreDefaults.AddRange(Array.map run preDefaults)
        into.css.PostInferenceChecksFinal.AddRange(Array.map run final)

        for stamp, cx in extraCxs do
            into.css.ExtraCxs.Add(stamp, cx)

        into.css.UnionsWithDeferredAttributes <- Set.union into.css.UnionsWithDeferredAttributes unionsWithDeferredAttributes
        into.css.DeferredUnionNullnessChecks <- deferredUnionNullnessChecks @ into.css.DeferredUnionNullnessChecks

        if createsGeneratedProvidedTypes then
            into.createsGeneratedProvidedTypes <- true

        for KeyValue(key, m) in namedDebugPoints do
            into.namedDebugPointsForInlinedCode[key] <- m

    /// Deferred union nullness checks, which compare a range they keep to themselves with one in the solver's state:
    /// a shift could move only one of them.
    member _.HasDeferredChecks = not deferredUnionNullnessChecks.IsEmpty

    /// The checks the finish runs (post-inference checks: a member constraint retried, a type's nullness, an enum's
    /// underlying type) keep the ranges they report at to themselves: each is run with what it reports, and what it
    /// raises, moved. Those ranges are the ones its check's constraint solver was given (ConstraintSolver.fs: the
    /// `csenv` and `m2` of PostponeOnFailedMemberConstraintResolution, SolveTypeIsEnum and AddCxTypeCanCarryNullnessInfo),
    /// all of the declaration's own syntax, so a shift moves all of them. A range elsewhere in the file would be a miss
    /// the finish can no longer refuse: it is counted (DeclarationShift.FinishMisses), which the tests require to be 0.
    member _.Shifted(s: DeclarationShift.Shifter) =
        // Numbers and ranges only: nothing here keeps a shifter alive, and the first shift's range is the check's
        let fileIndex, oldRange, scopeEnd, delta = s.Parameters

        let moved =
            match moved with
            | ValueSome(struct (fileIndex, range, scopeEnd, earlier)) -> struct (fileIndex, range, scopeEnd, earlier + delta)
            | ValueNone -> struct (fileIndex, oldRange, scopeEnd, delta)

        Leftovers(
            preDefaults,
            final,
            extraCxs |> List.map (fun (stamp, (info, m)) -> stamp, (info, s.Own m)),
            unionsWithDeferredAttributes,
            deferredUnionNullnessChecks,
            createsGeneratedProvidedTypes,
            namedDebugPoints |> Array.map (fun kv -> KeyValuePair(kv.Key, s.Own kv.Value)),
            ValueSome moved
        )

/// Tracks members added in place by later declarations without pinning superseded Tycons across FileStarts.
[<Sealed>]
type internal AugmentedTypes() =
    let table = ConditionalWeakTable<Tycon, ResizeArray<ValRef>>()
#if NETSTANDARD2_0
    let keys = ResizeArray<WeakReference<Tycon>>()
#endif

    member _.Add(target: Tycon, memberRef: ValRef) =
#if NETSTANDARD2_0
        match table.TryGetValue target with
        | true, members -> members.Add memberRef
        | false, _ ->
            let members = ResizeArray()
            members.Add memberRef
            table.Add(target, members)
            keys.Add(WeakReference<Tycon>(target))
#else
        table.GetOrCreateValue(target).Add(memberRef)
#endif

    member _.Iter(action: Tycon -> ResizeArray<ValRef> -> unit) =
#if NETSTANDARD2_0
        let mutable write = 0

        for read in 0 .. keys.Count - 1 do
            let wr = keys[read]

            match wr.TryGetTarget() with
            | true, tycon ->
                keys[write] <- wr
                write <- write + 1

                match table.TryGetValue tycon with
                | true, members -> action tycon members
                | false, _ -> ()
            | false, _ -> ()

        if write < keys.Count then
            keys.RemoveRange(write, keys.Count - write)
#else
        for KeyValue(tycon, members) in table do
            action tycon members
#endif

/// An implementation file set up for checking its declarations one at a time.
[<Sealed>]
type FileStart
    internal
    (
        ctx: ProjectContext,
        parse: FSharpParseFileResults,
        file: ParsedImplFileInput,
        moduleNamesDict: ModuleNamesDict,
        tcState: TcState,
        rootSigOpt: ModuleOrNamespaceType option,
        scope: ImplFileDeclarationScope,
        scopeContents: ModuleOrNamespaceType,
        sink: TcResultsSinkImpl,
        leftovers: Leftovers,
        rawDiagnostics: PhasedDiagnostic[]
    ) =
    let sync = obj ()
    let finishUndo = FinishUndo()
    let mutable lastShares: obj | null = null
    let augmented = AugmentedTypes()

    member internal _.Context = ctx
    /// Each type a later declaration's check gave members in place, with those members. Under Sync.
    member internal _.Augmented = augmented
    /// The parse the file was set up from.
    member _.Parse = parse
    member internal _.File = file
    member internal _.ModuleNamesDict = moduleNamesDict
    member internal _.TcState = tcState
    member internal _.RootSigOpt = rootSigOpt
    member internal _.Scope = scope
    /// What the scope's accumulator held before the declarations: FinishFile merges their deltas onto it.
    member internal _.ScopeContents = scopeContents
    member internal _.Sink = sink
    member internal _.Leftovers = leftovers
    /// What setting up the file reported, before filtering: FinishFile filters it with its parse's #nowarn scopes
    member internal _.RawDiagnostics = rawDiagnostics
    /// Held while finishing the file, and while a replay hides the members later declarations add (Augmented).
    member internal _.Sync = sync
    /// The environment BeginFile gave: what the end environment is folded from
    member val internal EnvAtStart: DeclarationEnvironment = Unchecked.defaultof<_> with get, set

    /// Undoes what the latest finish changed in the declarations' typed trees, once.
    member internal _.UndoLastFinish() =
        lock sync (fun () ->
            match finishUndo.Log with
            | null -> ()
            | log ->
                // Cleared only after every action ran: they restore saved values, so after an exception part-way the
                // next call can run them all again.
                MutationLog.Undo log
                finishUndo.Log <- null)

    member internal _.SetLastFinish(log: ResizeArray<unit -> unit>) = finishUndo.Log <- log

    /// Under Sync.
    member internal _.FinishUndo = finishUndo

    /// What the latest finish computed that the next one may reuse (a FinishShares), or null. Under Sync.
    member internal _.LastShares
        with get () = lastShares
        and set (shares: obj | null) = lastShares <- shares

[<Sealed>]
type DeclarationResult
    internal
    (
        start: FileStart,
        declaration: ParsedDeclaration,
        fingerprint: string,
        atCheck: string voption,
        diagnostics: FSharpDiagnostic[],
        rawDiagnostics: PhasedDiagnostic[],
        leavesFreeTypars: bool,
        dependsOnLater: bool,
        openTypars: Typar[],
        leavesUnsolved: bool,
        slotsAtCheck: IReadOnlyDictionary<Val, SlotSig list * bool>,
        envAfter: DeclarationEnvironment,
        envAtEnd: TcEnv,
        checkedDecl: CheckedDeclaration,
        delta: ModuleOrNamespaceType,
        deltaCombines: bool,
        sink: TcResultsSinkImpl,
        leftovers: Leftovers,
        nesting: Nesting,
        typedUses: DeclarationRef[],
        heldDefinitions: (Stamp * DeclarationRef)[],
        references: (DeclarationRef * string list * string)[],
        shareKey: obj,
        capturedScope: range
    ) =
    let mutable finishTypedUses: DeclarationRef[] = [||]
    let mutable shiftedTo: DeclarationResult | null = null

    member _.Key = declaration.Key
    /// What FinishFile keeps its computations for this result by: the check's, which shifting it keeps (see Shift)
    member internal _.ShareKey = shareKey
    /// The scope the check was given for the declaration (its ScopeRange then), moved by each shift of the result: the
    /// scope its environments and opens hold. The current parse's scope can end elsewhere, when a declaration was added
    /// or edited below it since; ScopesNow maps this one to it.
    member _.CapturedScope = capturedScope
    /// Made by CheckDeclarationAdHoc: its types are its own, and no finish holds it
    member val internal IsAdHoc = false with get, set
    /// The fingerprint as the check left the declaration. Finishing the file can still change it, by defaults or by
    /// later declarations solving its inference variables, and only then: FileResult.Fingerprints has the final one,
    /// which differs from this one only when DependsOnLaterDeclarations.
    member _.Fingerprint = fingerprint
    /// When FinishFile changes more of the declaration than the declarations after it see (see
    /// DeclarationStep.whileFinished), the fingerprint of what they see, as the check left it: Fingerprint and the final
    /// fingerprint are this and the description of what the finish leaves.
    member internal _.AtCheck = atCheck
    /// Provisional: the declaration's diagnostics filtered with the project's live options when it was checked,
    /// whose #nowarn scopes are those of whichever version of the file was parsed last. FileResult.Diagnostics is
    /// authoritative: FinishFile filters what the check reported with the scopes of the parse it is given, and
    /// renders it after closing the inference scope, so its types show what defaults solved.
    member _.Diagnostics = diagnostics

    /// Whether the environment after the declaration holds inference variables nothing has solved yet
    /// (ComputeUngeneralizableTypars of it holds one that is not rigid), as `let r = ref []` leaves. A later declaration's check can
    /// solve them in place, and nothing undoes that, so the result depends on every later declaration of its file:
    /// see DependsOnLaterDeclarations.
    member _.LeavesFreeTypars = leavesFreeTypars

    /// Whether later checks change this result in place, so that (ruling R12) it depends on every later declaration
    /// of its file: when one changes, is added or is removed, discard this result and re-check the declaration from
    /// its environment before it, and everything after it. That is when it LeavesFreeTypars, or when what it publishes
    /// or an inline body of it holds an inference variable nothing has solved: a member's can be solved by a later use
    /// of the member, and one only in an inline body by the finish's defaults (this over-approximates that case: no
    /// later declaration reaches such a variable, but the finish changes the fingerprint). Checks also mark the values
    /// they use referenced, which the unused-value warning (FS1182) reads; FinishFile recomputes those marks from the
    /// current results (ruling R15), so they need no dependency.
    member _.DependsOnLaterDeclarations = dependsOnLater

    /// Whether a later result holding what the declaration publishes must hold its latest check's: another check gives new
    /// entities, which entity references compare by object, so a result holding the old ones is not one to check against
    /// the new ones, whatever their fingerprints say. A type abbreviation is looked through, so it counts only when its
    /// target reaches another declaration of its file: the file's inferred signature remaps that file's entities, and an
    /// old abbreviation would keep pointing at the unmapped one.
    member _.DefinesEntities =
        delta.AllEntities
        |> Seq.exists (fun entity ->
            not entity.IsTypeAbbrev
            || typedUses
               |> Array.exists (fun r -> r.Key <> Some declaration.Key && r.File = start.Parse.FileName))

    /// The inference variables that made this result DependsOnLaterDeclarations, as the check left them: every one that
    /// is open is one of these, and it is empty exactly when the result does not depend on later declarations.
    member internal _.OpenTypars = openTypars

    /// `OpenTypars`, each with where it comes from. For diagnosing why a result depends on later declarations.
    member _.DescribeOpenTypars() =
        openTypars
        |> Array.map (fun tp ->
            $"%s{tp.DisplayName}#%d{tp.Stamp} at %s{tp.Range.FileName}(%d{tp.Range.StartLine},%d{tp.Range.StartColumn}) solved=%b{tp.IsSolved} rigidity=%A{tp.Rigidity}")

    /// Whether the check left an inference variable nothing has solved anywhere in what it checked, bodies included.
    /// The finish's defaults solve such a variable, or not when an earlier default in the file fails, so what the
    /// finish renders and takes for this result depends on the other declarations of the file.
    member internal _.LeavesUnsolved = leavesUnsolved

    member _.EnvAfter = envAfter

    /// The declarations of the project this declaration depends on through types, which its names may not show:
    /// those defining every entity, value and trait solution in its checked contents, in the types of what it binds
    /// and in what its checker reported resolving (so a tree damaged by error recovery is still covered), and then,
    /// for each type of the project among them, the types it is made of (fields, union cases, base type, interfaces,
    /// abbreviation, type parameter constraints), transitively. Assemblies the project references are left out: they
    /// do not change within a session. Taken at the check, and again at the latest FinishFile given this result, which
    /// can solve more; this is the union of both.
    member _.TypedUses =
        match finishTypedUses with
        | [||] -> typedUses
        | atFinish -> Array.append typedUses atFinish |> Array.distinct |> Array.sort

    member internal _.SetFinishTypedUses(uses: DeclarationRef[]) = finishTypedUses <- uses
    member internal _.FinishTypedUses = finishTypedUses

    /// The result the latest Shift of this one gave, which owns the items it moved; null while it was not shifted.
    /// Under the FileStart's lock.
    member internal _.ShiftedTo
        with get () = shiftedTo
        and set (r: DeclarationResult | null) = shiftedTo <- r

    member internal _.CheckTypedUses = typedUses

    /// Each entity and value of the project the check's TypedUses went through, with the declaration defining it. A kept
    /// result can hold one its declaration's newer result no longer has (a type abbreviation re-checked with the same
    /// fingerprint keeps its users), so an environment holding this result must still know where it came from: a check
    /// that reaches it through this result depends on that declaration.
    member internal _.HeldDefinitions = heldDefinitions
    /// The module values of the project the check marked referenced: the declaration defining each, the path of the
    /// module it is in, and its name
    member internal _.References = references
    member internal _.Start = start
    member internal _.Declaration = declaration

    member internal _.Ref =
        {
            File = start.Parse.FileName
            Key = Some declaration.Key
        }

    /// What the check reported, before filtering
    member internal _.RawDiagnostics = rawDiagnostics
    /// The environment at the end of the enclosing scope if this is its last declaration.
    member internal _.EnvAtEnd = envAtEnd
    member internal _.Checked = checkedDecl
    /// What the declaration publishes into the module or namespace it is in; for a module header, the module without
    /// contents, which FinishFile does not merge (it finishes the module from its declarations).
    member internal _.Delta: ModuleOrNamespaceType = delta
    /// Whether the delta is combined into the file's contents, as a namespace fragment's is, rather than added.
    member internal _.DeltaCombines = deltaCombines

    member internal _.Sink = sink
    member internal _.Leftovers = leftovers
    member internal _.Nesting = nesting
    /// See DeclarationStep.slotsAtCheck
    member internal _.SlotsAtCheck = slotsAtCheck

    /// Whether the two results contribute the same Val and Entity objects, by reference and in the same order.
    /// For tests.
    member _.IsSameDelta(other: DeclarationResult) =
        let same (mine: seq<'T>) (theirs: seq<'T>) =
            Seq.length mine = Seq.length theirs
            && Seq.forall2 (fun a b -> obj.ReferenceEquals(a, b)) mine theirs

        same delta.AllValsAndMembers other.Delta.AllValsAndMembers
        && same delta.AllEntities other.Delta.AllEntities

[<Sealed>]
type FileResult
    internal
    (
        diagnostics: FSharpDiagnostic[],
        fingerprints: IReadOnlyDictionary<DeclarationKey, string>,
        signature: FileSignature,
        checkResults: Lazy<FSharpCheckFileResults>,
        phaseTicks: IReadOnlyDictionary<string, int64>,
        phaseAllocated: IReadOnlyDictionary<string, int64>,
        reused: int,
        topAttrs: TopAttribs,
        beforeRead: unit -> unit,
        buildAgain: (unit -> FSharpCheckFileResults) voption,
        classify: (range option -> SemanticClassificationItem[]) voption
    ) =
    member _.Diagnostics = diagnostics
    /// The assembly-level attributes the file's tail gave: its signature's fingerprint leaves them out when the file
    /// has a signature file.
    member internal _.TopAttrs = topAttrs
    /// Each declaration's fingerprint after the file's inference scope is closed.
    member _.Fingerprints = fingerprints
    /// FinishFile's is the previous finish's object, on the same FileStart, when every final fingerprint and the
    /// file's own fingerprint are unchanged from it.
    member _.Signature = signature

    /// Built on first read, holding the file's check lock. CheckFile's are built by checking the file again with a
    /// sink in the setup it was checked in; that build raises InvalidOperationException, then and on every later
    /// read, when the file's line directives are no longer those of the parse it checked: a newer parse of another
    /// text of the file replaced them. FinishFile's are built from what the finish captured, with the checker state
    /// the finish gave; never while a later finish or declaration check on the same FileStart undoes the finish. For a
    /// file with a signature file, building them applies the conformance check to the implementation again (FinishFile
    /// undid it), under the FileStart's lock and then the file's check lock, as a declaration check takes them. Their
    /// queries read the declarations' typed trees as they are when asked: after a later finish or declaration check on
    /// the same FileStart, as it left them, as for every result checked by declaration, so a superseded FileResult's
    /// queries show the implementation unconformed (without the signature's argument names and xml docs) until the
    /// next finish's results are built. Query the latest FileResult.
    member _.CheckResults =
        // Outside the lazy, which would keep the exception for every later read
        if not checkResults.IsValueCreated then
            beforeRead ()

        checkResults.Value

    /// As CheckResults, for a reader that copies out what it needs: CheckFile's are checked again for this read and not
    /// kept, so this result goes on holding only its inputs, unless CheckResults was read first. FinishFile's are CheckResults.
    member this.CheckResultsNotKept =
        match buildAgain with
        | ValueSome build when not checkResults.IsValueCreated -> build ()
        | _ -> this.CheckResults

    /// Semantic classifications for the checked file, filtered to `range` when given. FinishFile's merge its sinks
    /// without building or retaining CheckResults; CheckFile's use CheckResultsNotKept unless CheckResults was already built.
    member this.GetSemanticClassification(?range: range) : SemanticClassificationItem[] =
        if checkResults.IsValueCreated then
            checkResults.Value.GetSemanticClassification(range)
        else
            match classify with
            | ValueSome f ->
                beforeRead ()
                f range
            | ValueNone -> this.CheckResultsNotKept.GetSemanticClassification(range)

    /// Stopwatch ticks of each part of the finish, in source order: wait (the locks and undoing the last finish),
    /// refilter, leftovers, envAtEnd, assemble, replay, tail, typedUses, render, fingerprints, signature, sinks. CheckFile's:
    /// setup, tail (the check), definitions, fingerprint, render.
    member _.PhaseTicks = phaseTicks
    /// The bytes the checking thread allocated in each part, as PhaseTicks names them.
    member _.PhaseAllocated = phaseAllocated
    /// How many results' final fingerprint, finish TypedUses and rendered diagnostics came from the previous finish
    /// of the same FileStart. For tests; 0 for CheckFile.
    member _.Reused = reused

/// A project's assembly as a referencing project sees it, built from the project's checked files without compiling.
[<Sealed>]
type ProjectAssembly internal (fingerprint: string, data: Lazy<ProjectAssemblyDataResult>) =
    /// Equal only when a referencing project would check the same against either: the file signatures' fingerprints
    /// and the assembly-level attributes (InternalsVisibleTo, AutoOpen). Unequal for anything not describable.
    member _.Fingerprint = fingerprint
    /// Pickled when first read: a referencing project reads it only when the fingerprint has changed.
    member internal _.Data = data.Value

    /// Whether a referencing project reads the file at the path instead: a project with type provider generated types.
    /// Pickles the assembly. Such a project is as of its last build.
    member _.ReadsFileOnDisk =
        match data.Value with
        | ProjectAssemblyDataResult.Available _ -> false
        | ProjectAssemblyDataResult.Unavailable _ -> true

/// What a finish rendered and fingerprinted for one result.
[<NoEquality; NoComparison>]
type internal FinishShare =
    {
        Fingerprint: string
        Diagnostics: FSharpDiagnostic[]
        ErrorCount: int
        /// The line the declaration started on: a shifted result reuses the diagnostics only when it has none
        Line: int
    }

/// What the latest finish of a FileStart computed that the next finish of it may reuse: per result given to it, by
/// its ShareKey (a shifted result has its check's), for the options and line directives it rendered with; and its
/// signature, for the keys and fingerprints it had.
[<Sealed>]
type internal FinishShares
    (
        options: FSharpDiagnosticOptions,
        directives: (int * (int * int)) list,
        shares: Dictionary<obj, FinishShare>,
        keys: DeclarationKey[],
        fingerprints: string[],
        signature: FileSignature option,
        envs: (obj * DeclarationEnvironment)[],
        exits: TcResultsSinkImpl[]
    ) =
    member _.Options = options
    member _.Directives = directives
    member _.Shares = shares
    member _.Keys = keys
    member _.Fingerprints = fingerprints
    /// None when the finish's tail failed
    member _.Signature = signature
    /// The environment after each declaration as the finish assembled it, with the result's ShareKey
    member _.Envs = envs
    /// What leaving modules reported: at i, before the declaration at i, and last, after every declaration
    member _.Exits = exits

/// Scope ranges as the current text gives them, by the sink that captured them. `rows` holds each declaration's sink, range
/// and scope range when it was checked; `current` gives each declaration's range and scope range in the current text.
type internal ScopesNow
    (setup: TcResultsSinkImpl, setupEnd: pos, rows: (TcResultsSinkImpl * range * range)[], fileEnd: pos, current: unit -> (range * range)[])
    =
    let moved =
        lazy
            (let d =
                Dictionary<TcResultsSinkImpl, struct (range * range * range * range)>(HashIdentity.Reference)

             let now = current ()

             if now.Length = rows.Length then
                 for i in 0 .. rows.Length - 1 do
                     let sink, oldRange, oldScope = rows[i]
                     let range, scope = now[i]
                     d[sink] <- struct (oldRange, oldScope, range, scope)

             d)

    member _.Of(sink: TcResultsSinkImpl, m: range) : range =
        if obj.ReferenceEquals(sink, setup) then
            if FSharp.Compiler.Text.Position.posEq m.End setupEnd then
                mkFileIndexRange m.FileIndex m.Start fileEnd
            else
                m
        else
            match moved.Value.TryGetValue sink with
            | true, struct (oldRange, oldScope, rangeNow, scopeNow) when FSharp.Compiler.Text.Position.posEq m.End oldScope.End ->
                // The declaration's scope as its check was given it, or from the declaration's end to the scope's end
                if FSharp.Compiler.Text.Position.posEq m.Start oldScope.Start then
                    scopeNow
                elif FSharp.Compiler.Text.Position.posEq m.Start oldRange.End then
                    mkFileIndexRange m.FileIndex rangeNow.End scopeNow.End
                else
                    m
            | true, _ -> m
            | _ ->
                // What leaving a module reported, at the scope its header's check was given
                moved.Value.Values
                |> Seq.tryPick (fun (struct (_, oldScope, _, scopeNow)) -> if Range.equals oldScope m then Some scopeNow else None)
                |> Option.defaultValue m
