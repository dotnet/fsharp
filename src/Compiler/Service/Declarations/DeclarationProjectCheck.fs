// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CodeAnalysis.Declarations.DeclarationProjectCheck

open System
open System.Collections.Generic
open System.Security.Cryptography
open System.Text
open System.Threading

open Internal.Utilities.Library

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.EditorServices
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

#if NETSTANDARD2_0
let private getAllocatedBytesForCurrentThread: unit -> int64 =
    match typeof<GC>.GetMethod("GetAllocatedBytesForCurrentThread", Type.EmptyTypes) with
    | null -> fun () -> 0L
    | mi ->
        let d = Delegate.CreateDelegate(typeof<Func<int64>>, mi) :?> Func<int64>
        fun () -> d.Invoke()
#endif

let allocatedBytesForCurrentThread () : int64 =
#if NETSTANDARD2_0
    getAllocatedBytesForCurrentThread ()
#else
    GC.GetAllocatedBytesForCurrentThread()
#endif

let private toHexString (bytes: byte[]) : string =
#if NETSTANDARD2_0
    BitConverter.ToString(bytes).Replace("-", "")
#else
    Convert.ToHexString bytes
#endif

/// Loggers filtering by these options, whose #nowarn scopes may be a parse's copy.
let newLoggersWith (diagnosticsOptions: FSharpDiagnosticOptions) =
    let capturing = CompilationDiagnosticLogger("TypeCheckFile", diagnosticsOptions)
    capturing, RawDiagnostics(GetDiagnosticsLoggerFilteringByScopedNowarn(diagnosticsOptions, capturing))

let newLoggers (state: CompilerState) =
    newLoggersWith state.tcConfig.diagnosticsOptions

let diagnosticsWith (state: CompilerState) (diagnosticsOptions: FSharpDiagnosticOptions) fileName (capturing: CompilationDiagnosticLogger) =
    DiagnosticHelpers.CreateDiagnostics(
        diagnosticsOptions,
        false,
        fileName,
        capturing.GetDiagnostics(),
        suggestNamesForErrors,
        state.tcConfig.flatErrors,
        None
    )

let diagnosticsOf (state: CompilerState) fileName (capturing: CompilationDiagnosticLogger) =
    diagnosticsWith state state.tcConfig.diagnosticsOptions fileName capturing

/// The diagnostics in the project's source files, as a full check's ErrorsByFile keeps them (ruling R18). Rendering
/// keeps a diagnostic by the file of its range as reported, then applies #line directives to it: one in generated
/// code mapped to its source, such as pars.fs to pars.fsy, would otherwise be reported in that other file.
let projectDiagnostics (ctx: ProjectContext) (diagnostics: FSharpDiagnostic[]) =
    diagnostics
    |> Array.filter (fun diagnostic -> ctx.IsSourceFile diagnostic.FileName)

/// Diagnostics as reported, filtered by these options (a parse's copy, with its #nowarn scopes), not yet
/// rendered: their text shows the types as they are when diagnosticsWith renders them.
let refiltered diagnosticsOptions (raw: PhasedDiagnostic[]) =
    let capturing, logger = newLoggersWith diagnosticsOptions

    for diagnostic in raw do
        logger.DiagnosticSink diagnostic

    capturing

/// Raises unless `before` is a sound setup for the file: earlier files of the project in file order, either every
/// one of them, or at least those the latest FileDependencies found the file depends on. That list is used only
/// while it is current: it was computed from this parse of the file, and from the latest parse of each earlier
/// file, whose contents decide what the file depends on.
///
/// For a setup from part of the earlier files, gives the module-name dictionary the file is deduplicated against,
/// as checking every earlier file gives it: the files in `before` alone would miss the names of the others.
let private checkBefore
    (ctx: ProjectContext)
    (before: FileSignature list)
    (parse: FSharpParseFileResults)
    : (ModuleNamesDict * HashSet<string>) option =
    match ctx.IndexOf parse.FileName with
    | None -> None
    | Some idx ->
        let mutable last = -1

        for signature in before do
            match ctx.IndexOf signature.FileName with
            | Some position when position > last && position < idx -> last <- position
            | _ ->
                raise (
                    ArgumentException(
                        $"The files before %s{parse.FileName} must be earlier files of the project, in file order: %s{signature.FileName} is not.",
                        nameof before
                    )
                )

        if before.Length = idx then
            None
        else
            let isCurrent (snapshot: DependencySnapshot) =
                snapshot.Parses.Length > idx
                && obj.ReferenceEquals(snapshot.Parses[idx], parse)
                && Seq.forall
                    (fun earlier ->
                        match ctx.LatestParse snapshot.Parses[earlier].FileName with
                        | Some latest -> obj.ReferenceEquals(latest, snapshot.Parses[earlier])
                        | None -> false)
                    (seq { 0 .. idx - 1 })

            match ctx.Dependencies with
            | Some snapshot when isCurrent snapshot ->
                let present =
                    HashSet<string>(before |> Seq.map _.FileName, StringComparer.OrdinalIgnoreCase)

                match
                    snapshot.Dependencies[idx]
                    |> Array.tryFind (fun dep -> not (present.Contains snapshot.Parses[dep].FileName))
                with
                | Some missing ->
                    raise (
                        ArgumentException(
                            $"%s{parse.FileName} depends on %s{snapshot.Parses[missing].FileName}, which the files before it miss.",
                            nameof before
                        )
                    )
                | None -> Some(snapshot.ModuleNamesBefore[idx], snapshot.MatchedSignatures)
            | _ ->
                raise (
                    StaleDependenciesException
                        $"The files before %s{parse.FileName} are not every earlier file, and FileDependencies has not been given this parse with the latest parse of every earlier file, so its dependencies are unknown."
                )

/// The file-level fingerprint of what later files see of a file.
let private signatureFingerprint (mty: ModuleOrNamespaceType) (topAttrs: TopAttribs) =
    match SignatureFingerprint.compute mty topAttrs with
    | Some fingerprint -> fingerprint
    | None -> "?" + Guid.NewGuid().ToString("N")

/// An implementation file's: its signature file's when it has one, as CheckSignatureFile gives it.
let implFingerprint (rootSigOpt: ModuleOrNamespaceType option) (implFileSig: ModuleOrNamespaceType) (topAttrs: TopAttribs) =
    match rootSigOpt with
    | Some sigFileType -> signatureFingerprint sigFileType EmptyTopAttrs
    | None -> signatureFingerprint implFileSig topAttrs

/// The checker state after `before`, for checking `fileName`. A signature file in `before` whose implementation
/// file comes before `fileName` but is not in `before` is also added to the implementation environment, as graph
/// mode's ArtificialImplFile node adds it, at the implementation file's place in file order. So a file depending
/// on a signature file need not wait for the implementation file's check, and it still sees what a check of every
/// earlier file would. Only for the pairs `matched` holds (FileDependencies keeps the implementation file on the
/// lists of the others); a signature file whose implementation comes later is not added either: such a check
/// would not see it.
let private stateBefore (checks: ProjectChecks) (before: FileSignature list) (matched: HashSet<string> option) (fileName: string) =
    let present =
        HashSet<string>(before |> Seq.map (fun signature -> signature.FileName), StringComparer.OrdinalIgnoreCase)

    let implementationOf (signatureFile: string) =
        signatureFile.Substring(0, signatureFile.Length - 1)

    let artificial =
        [
            for signature in before do
                match signature.AsImplementation, matched with
                | Some asImplementation, Some matched when
                    matched.Contains signature.FileName
                    && not (present.Contains(implementationOf signature.FileName))
                    && checks.IsBefore(implementationOf signature.FileName, fileName)
                    ->
                    yield checks.IndexOf(implementationOf signature.FileName), signature.ArtificialKey, asImplementation
                | _ -> ()
        ]

    // In file order, each step where its file is: the environments layer what they hold in the order a check of
    // every earlier file adds it (AutoOpen modules and extension members shadow by it)
    let steps =
        [
            for signature in before -> checks.IndexOf signature.FileName, (signature :> obj), (fun tcState -> signature.Apply tcState)
        ]
        @ artificial
        |> List.sortBy (fun (index, _, _) -> index)

    // From the states the setups before reached: one setup's steps are mostly a prefix of another's
    let tcState =
        (checks.SetupRoot, steps)
        ||> List.fold (fun (node: SetupNode) (_, key, step) -> node.After(key, step))
        |> _.State

    let moduleNamesDict =
        lazy
            ((Map.empty, before)
             ||> List.fold (fun (acc: ModuleNamesDict) signature ->
                 (acc, signature.ModuleNamesDict)
                 ||> Map.fold (fun acc qualifiedName files ->
                     let known = acc |> Map.tryFind qualifiedName |> Option.defaultValue Map.empty
                     acc |> Map.add qualifiedName (Map.foldBack Map.add files known))))

    tcState, moduleNamesDict

/// The checker state after `before` and the implementation file to check in it, as BeginFile and CheckFile set
/// them up.
let private implFileState
    (checks: ProjectChecks)
    (before: FileSignature list)
    (scoped: (ModuleNamesDict * HashSet<string>) option)
    (parse: FSharpParseFileResults)
    =
    let tcState, chainModuleNames =
        stateBefore checks before (Option.map snd scoped) parse.FileName

    let moduleNamesDict =
        scoped |> Option.map fst |> Option.defaultWith chainModuleNames.Force

    let input, moduleNamesDict =
        DeduplicateParsedInputModuleName moduleNamesDict parse.ParseTree

    match input with
    | ParsedInput.SigFile _ -> invalidArg (nameof parse) "Expected an implementation file."
    | ParsedInput.ImplFile file -> tcState, moduleNamesDict, input, file

/// What CheckOneInput does before checking an implementation file, reporting to the ambient logger: the file's
/// signature, if any.
let private enterImplFile (state: CompilerState) (tcState: TcState) (input: ParsedInput) (file: ParsedImplFileInput) =
    ResetGeneratedNames(input, state)
    let qualNameOfFile = file.QualifiedName

    if tcState.HasRootImpl qualNameOfFile then
        errorR (Error(FSComp.SR.buildImplementationAlreadyGiven (RichText.mkModule qualNameOfFile.Text), input.Range))

    tcState.TryFindRootSig qualNameOfFile

let private conditionalDefines (state: CompilerState) =
    if state.tcConfig.noConditionalErasure then
        None
    else
        Some state.tcConfig.conditionalDefines

/// Runs a check of an implementation file whose signature conformance would write the other ranges of its
/// signature file's items (rootSigOpt): the implementation's range on each, which go to implementation reads in
/// every file set up from the signature. Only a check of the file's latest parse writes them; one of an older
/// text leaves them, so they are the latest text's once it has been checked, whatever other checks of the file
/// run and in whatever order. Run under the file's check lock.
let leavingSignatureRanges
    (checks: ProjectChecks)
    (parse: FSharpParseFileResults)
    (rootSigOpt: ModuleOrNamespaceType option)
    (check: unit -> 'T)
    : 'T =
    let latest =
        match checks.LatestParse parse.FileName with
        | Some latest -> obj.ReferenceEquals(latest, parse)
        | None -> false

    if rootSigOpt.IsNone || latest then
        check ()
    else
        let outer = SignatureConformance.SignatureRanges.Keep
        SignatureConformance.SignatureRanges.Keep <- true

        try
            check ()
        finally
            SignatureConformance.SignatureRanges.Keep <- outer

/// Gives every value the resolutions name, and every value the checked file binds, its compiled name now, under the
/// file's check lock: a compiled name some values get is generated on first read, from the file's counters, which
/// a check of the file resets. Queries on the results, the implementation file's included, then read given names.
let compiledNamesOf (state: CompilerState) (resolutions: TcResolutions) (implFileOpt: CheckedImplFile option) =
    let globalState = state.tcGlobals.CompilerGlobalState

    let read (vref: ValRef) =
        if vref.IsLocalRef then
            vref.Deref.CompiledName globalState |> ignore

    match implFileOpt with
    | Some implFile ->
        let folder =
            { ExprFolder0 with
                valBindingSiteIntercept = fun () (_, v: Val) -> v.CompiledName globalState |> ignore
            }

        FoldImplFile folder () implFile
    | None -> ()

    for resolution in resolutions.CapturedNameResolutions do
        match resolution.Item with
        | Item.Value vref
        | Item.CustomBuilder(_, vref) -> read vref
        | Item.ActivePatternCase apref -> read apref.ActivePatternVal
        | Item.MethodGroup(methods = minfos)
        | Item.CtorGroup(_, minfos) -> minfos |> List.iter (fun minfo -> Option.iter read minfo.ArbitraryValRef)
        | Item.Property(info = pinfos) -> pinfos |> List.iter (fun pinfo -> Option.iter read pinfo.ArbitraryValRef)
        | Item.Event einfo -> Option.iter read einfo.ArbitraryValRef
        | _ -> ()

/// CheckOneImplFile on an implementation file set up by implFileState and enterImplFile, recovering from an error as
/// CheckOneInput does, under the ambient logger.
let private checkImplFile
    (state: CompilerState)
    (tcState: TcState)
    rootSigOpt
    file
    (parse: FSharpParseFileResults)
    (logger: DiagnosticsLogger)
    sink
    =
    let checkForErrors () =
        parse.ParseHadErrors || logger.ErrorCount > 0

    try
        CheckOneImplFile(
            state.tcGlobals,
            state.tcImports.GetImportMap(),
            tcState.Ccu,
            tcState.ImplicitOpenDeclarations,
            checkForErrors,
            conditionalDefines state,
            sink,
            state.tcConfig.internalTestSpanStackReferring,
            tcState.TcEnvFromImpls,
            rootSigOpt,
            file,
            state.tcConfig.diagnosticsOptions
        )
        |> Cancellable.runWithoutCancellation
        |> Some
    with RecoverableException exn ->
        errorRecovery exn range0
        None

/// The check results of a file CheckFile checked without a sink: it is checked again, with one, in the same
/// setup. Like CheckFile, the build refuses (refuseUnlessCurrentDirectives) before and after the check. Its
/// diagnostics are those of the first check, already rendered; this check's are dropped.
let private buildCheckResults (checks: ProjectChecks) before moduleNamesBefore (parse: FSharpParseFileResults) diagnostics =
    let state = checks.State
    DeclarationSyntax.refuseUnlessCurrentDirectives parse
    let tcState, _, input, file = implFileState checks before moduleNamesBefore parse
    let _, logger = newLoggersWith (DeclarationSyntax.warnOptionsOf parse)
    use _ = new CompilationGlobalsScope(logger, BuildPhase.TypeCheck)
    use _ = Cancellable.UsingToken CancellationToken.None

    let sink =
        TcResultsSinkImpl(state.tcGlobals, ?sourceText = DeclarationSyntax.sourceOf parse)

    checks.CountCheckResultsBuilt()

    let rootSigOpt, checkedFile =
        lock (checks.FileLock parse.FileName) (fun () ->
            let rootSigOpt = enterImplFile state tcState input file

            let checkedFile =
                leavingSignatureRanges checks parse rootSigOpt (fun () ->
                    checkImplFile state tcState rootSigOpt file parse logger (TcResultsSink.WithSink sink))

            compiledNamesOf state (sink.GetResolutions()) (checkedFile |> Option.map (fun (_, implFile, _, _, _) -> implFile))
            rootSigOpt, checkedFile)

    DeclarationSyntax.refuseUnlessCurrentDirectives parse

    let ccuSigForFile, ownSigForFile, tcEnvAtEnd, implFileOpt =
        match checkedFile with
        | Some(_, implFile, tcEnvAtEnd, _, ownSigForFile) ->
            let ccuSigForFile, _ =
                AddCheckResultsToTcState
                    (state.tcGlobals,
                     state.tcImports.GetImportMap(),
                     rootSigOpt.IsSome,
                     None,
                     TcResultsSink.NoSink,
                     tcState.TcEnvFromImpls,
                     file.QualifiedName,
                     implFile.Signature)
                    tcState

            ccuSigForFile, ownSigForFile, tcEnvAtEnd, Some implFile
        | None ->
            tcState.CcuSig, Construct.NewEmptyModuleOrNamespaceType(ModuleOrNamespaceKind.Namespace true), tcState.TcEnvFromSignatures, None

    let scope =
        TypeCheckInfo(
            state.tcConfig,
            state.tcGlobals,
            ccuSigForFile,
            ownSigForFile,
            tcState.Ccu,
            state.tcImports,
            tcEnvAtEnd.AccessRights,
            state.projectOptions.ProjectFileName,
            parse.FileName,
            Some state.projectOptions,
            sink.GetResolutions(),
            sink.GetSymbolUses(),
            tcEnvAtEnd.NameEnv,
            None,
            implFileOpt,
            sink.GetOpenDeclarations()
        )

    FSharpCheckFileResults(parse.FileName, diagnostics, Some scope, parse.DependencyFiles, None, true)

/// A checker state of its own (solver, caches, sink), with a new initial environment and accumulator for the file.
let newCheckerState (state: CompilerState) (tcState: TcState) rootSigOpt file (sink: TcResultsSinkImpl) =
    CreateImplFileCheckState(
        state.tcGlobals,
        state.tcImports.GetImportMap(),
        tcState.Ccu,
        conditionalDefines state,
        TcResultsSink.WithSink sink,
        state.tcConfig.internalTestSpanStackReferring,
        tcState.TcEnvFromImpls,
        rootSigOpt,
        file,
        state.tcConfig.diagnosticsOptions
    )

/// The stamps of every entity and value in a module type, including those inside its modules.
let rec stampsOf (mty: ModuleOrNamespaceType) =
    seq {
        for entity in mty.AllEntities do
            yield entity.Stamp

            if entity.IsModuleOrNamespace then
                yield! stampsOf entity.ModuleOrNamespaceType

        for v in mty.AllValsAndMembers do
            yield v.Stamp
    }

/// Records the declaration as the definition of what its delta publishes.
let addDefinitions (definitions: Map<Stamp, DeclarationRef>) (declaration: DeclarationRef) (delta: ModuleOrNamespaceType) =
    (definitions, stampsOf delta)
    ||> Seq.fold (fun acc stamp -> Map.add stamp declaration acc)

let private definitionsBefore (before: FileSignature list) =
    (Map.empty, before)
    ||> List.fold (fun acc signature -> Map.foldBack Map.add signature.Definitions acc)

/// What a signature file declares: its declarations are not checked one at a time, so the file stands for them.
let private signatureDefinitions fileName (sigType: ModuleOrNamespaceType) =
    addDefinitions Map.empty { File = fileName; Key = None } sigType

/// The definitions of a file's inferred signature, a copy of the file's contents with new stamps
/// (copyModuleOrNamespaceType keeps every entity and value, in order): each copied entity and value is matched to
/// the original at the same position. A copy of another shape would lose definitions, so it raises.
let rec copiedDefinitions (own: Map<Stamp, DeclarationRef>) (contents: ModuleOrNamespaceType) (copy: ModuleOrNamespaceType) acc =
    let pairs (originals: seq<'T>) (copies: seq<'T>) (name: 'T -> string) =
        let originals, copies = Array.ofSeq originals, Array.ofSeq copies

        let names (items: 'T[]) =
            items |> Array.map name |> String.concat ", "

        if
            originals.Length <> copies.Length
            || Array.exists2 (fun o c -> name o <> name c) originals copies
        then
            invalidOp $"The inferred signature is not a copy of the file's contents: [%s{names originals}] against [%s{names copies}]."

        Array.zip originals copies

    let add acc (original: Stamp) (copied: Stamp) =
        match own.TryFind original with
        | Some declaration -> Map.add copied declaration acc
        | None -> acc

    let acc =
        (acc, pairs contents.AllEntities copy.AllEntities _.LogicalName)
        ||> Array.fold (fun acc (o, c) -> copiedDefinitions own o.ModuleOrNamespaceType c.ModuleOrNamespaceType (add acc o.Stamp c.Stamp))

    (acc, pairs contents.AllValsAndMembers copy.AllValsAndMembers _.LogicalName)
    ||> Array.fold (fun acc (o, c) -> add acc o.Stamp c.Stamp)

let implFileSignature
    (state: CompilerState)
    fileName
    qualNameOfFile
    (implFileSig: ModuleOrNamespaceType)
    createsGeneratedProvidedTypes
    moduleNamesDict
    definitions
    (fingerprint: Lazy<string>)
    =
    let amap = state.tcImports.GetImportMap()

    let apply (tcState: TcState) =
        let hadSig = (tcState.TryFindRootSig qualNameOfFile).IsSome

        let _, tcState =
            AddCheckResultsToTcState
                (state.tcGlobals, amap, hadSig, None, TcResultsSink.NoSink, tcState.TcEnvFromImpls, qualNameOfFile, implFileSig)
                tcState

        AddCreatesGeneratedProvidedTypes createsGeneratedProvidedTypes tcState

    FileSignature(fileName, apply, moduleNamesDict, definitions, fingerprint, implFileSig, None, false, [||], [||])

/// What CheckOneInput's PartialResultOnError gives later files: the checker state as it was before the file.
let failedFileSignature fileName moduleNamesDict =
    FileSignature(
        fileName,
        id,
        moduleNamesDict,
        Map.empty,
        // Every failed check of the file adds nothing, the same to every later file
        Lazy<string>.CreateFromValue("!failed"),
        Construct.NewEmptyModuleOrNamespaceType(ModuleOrNamespaceKind.Namespace true),
        None,
        true,
        [||],
        [||]
    )

let createProject (options: FSharpProjectOptions) : Async<ProjectContext> =
    async {
        let! state = initializeCompilerState options ignore
        return ProjectContext state
    }

/// A project context in which each `-r:` whose path is a key of `references` is that project's in-memory assembly,
/// not the file at the path. Everything else is as CreateProject.
let createProjectWithReferences
    (options: FSharpProjectOptions)
    (references: IReadOnlyDictionary<string, ProjectAssembly>)
    : Async<ProjectContext> =
    async {
        // Here, where the caller sees a failure: the imports turn one into a missing reference, in no logger
        for assembly in references.Values do
            assembly.Data |> ignore

        let projectReferences =
            [
                for KeyValue(path, assembly) in references ->
                    { new IProjectReference with
                        member _.FileName = path
                        member _.EvaluateRawContents() = async { return assembly.Data }
                        member _.TryGetLogicalTimeStamp _ = Some DateTime.UtcNow
                    }
            ]

        let! state = initializeCompilerStateWith projectReferences options ignore
        return ProjectContext state
    }

/// For tests: an assembly whose pickle raises.
let failingProjectAssembly (message: string) : ProjectAssembly =
    ProjectAssembly("!failing", Lazy<ProjectAssemblyDataResult>(fun () -> failwith message))

/// The project's assembly from its files' results and signatures, as an incremental build gives one: `files` are
/// the implementation files' results and `signatures` every file's, in file order. Each signature must be what the
/// files after it were checked against: one holding an entity of another version of an earlier file would pickle
/// a reference to nothing. A project with type provider generated types has none: the file at the path is read
/// instead.
let projectAssembly (ctx: ProjectContext) (files: FileResult list) (signatures: FileSignature list) : ProjectAssembly =
    let state = ctx.State
    let checks = ctx.Checks

    let topAttrs =
        List.foldBack (fun (file: FileResult) acc -> CombineTopAttrs file.TopAttrs acc) files EmptyTopAttrs

    let fingerprint =
        use hash = IncrementalHash.CreateHash HashAlgorithmName.SHA256

        let add (text: string) =
            hash.AppendData(Encoding.UTF8.GetBytes text)
            hash.AppendData [| 0uy |]

        for signature in signatures do
            add signature.FileName
            // A file nothing depended on takes its fingerprint here, reading compiled names given on first read
            add (lock (checks.FileLock signature.FileName) (fun () -> signature.Fingerprint))

        add (signatureFingerprint (Construct.NewEmptyModuleOrNamespaceType(ModuleOrNamespaceKind.Namespace true)) topAttrs)
        toHexString (hash.GetHashAndReset())

    let build () =
        let _, logger = newLoggers state
        use _ = new CompilationGlobalsScope(logger, BuildPhase.TypeCheck)
        use _ = Cancellable.UsingToken CancellationToken.None

        let tcState =
            (state.tcInitialState, signatures)
            ||> List.fold (fun tcState signature -> signature.Apply tcState)

        let _, _, ccuContents = CheckClosedInputSetFinish([], tcState)
        let generatedCcu = tcState.Ccu.CloneWithFinalizedContents ccuContents
        let assemblyName = tcState.Ccu.AssemblyName
        let tcConfig = state.tcConfig
        let tcGlobals = state.tcGlobals
        let mutable hasTypeProviderAssemblyAttrib = false

        // As IncrementalBuild's finalize step
        let ilAssemRef =
            let publicKey =
                try
                    let signingInfo =
                        CreateILModule.ValidateKeySigningAttributes(tcConfig, tcGlobals, topAttrs)

                    match CreateILModule.GetStrongNameSigner signingInfo with
                    | None -> None
                    | Some s -> Some(PublicKey.KeyAsToken s.PublicKey)
                with exn ->
                    errorRecoveryNoRange exn
                    None

            let mutable locale = None
            let mutable ver = None

            for attr in topAttrs.assemblyAttrs do
                let flag = classifyAssemblyAttrib tcGlobals attr

                if hasFlag flag WellKnownAssemblyAttributes.AssemblyCultureAttribute then
                    match attr with
                    | Attrib(_, _, [ AttribStringArg s ], _, _, _, _) -> locale <- Some s
                    | _ -> ()
                elif hasFlag flag WellKnownAssemblyAttributes.AssemblyVersionAttribute then
                    match attr with
                    | Attrib(_, _, [ AttribStringArg s ], _, _, _, _) ->
                        ver <-
                            (try
                                Some(parseILVersion s)
                             with _ ->
                                 None)
                    | _ -> ()
                elif hasFlag flag WellKnownAssemblyAttributes.TypeProviderAssemblyAttribute then
                    hasTypeProviderAssemblyAttrib <- true

            let version =
                match ver with
                | None -> tcConfig.version.GetVersionInfo tcConfig.implicitIncludeDir
                | Some v -> v

            ILAssemblyRef.Create(assemblyName, None, publicKey, false, Some version, locale)

        if tcState.CreatesGeneratedProvidedTypes || hasTypeProviderAssemblyAttrib then
            ProjectAssemblyDataResult.Unavailable true
        else
            let outfile = tcConfig.outputFile |> Option.defaultValue (assemblyName + ".dll")

            // The pickle reads compiled names given on first read, so no check of a file runs meanwhile. No check
            // waits for a second file's lock while holding one.
            let rec holding (names: string list) : IRawFSharpAssemblyData =
                match names with
                | [] ->
                    RawFSharpAssemblyDataBackedByLanguageService(
                        tcConfig,
                        tcGlobals,
                        generatedCcu,
                        outfile,
                        topAttrs,
                        assemblyName,
                        ilAssemRef
                    )
                | name :: rest -> lock (checks.FileLock name) (fun () -> holding rest)

            ProjectAssemblyDataResult.Available(holding (signatures |> List.map _.FileName))

    ProjectAssembly(fingerprint, Lazy<ProjectAssemblyDataResult>(build, LazyThreadSafetyMode.ExecutionAndPublication))

let checkSignatureFile (ctx: ProjectContext) (before: FileSignature list) (parse: FSharpParseFileResults) : FileSignature =
    let state = ctx.State

    match parse.ParseTree with
    | ParsedInput.ImplFile _ -> invalidArg (nameof parse) "Expected a signature file."
    | ParsedInput.SigFile _ ->
        let moduleNamesBefore = checkBefore ctx before parse

        let tcState, chainModuleNames =
            stateBefore ctx.Checks before (Option.map snd moduleNamesBefore) parse.FileName

        let moduleNamesDict =
            moduleNamesBefore |> Option.map fst |> Option.defaultWith chainModuleNames.Force

        let input, moduleNamesDict =
            DeduplicateParsedInputModuleName moduleNamesDict parse.ParseTree

        match input with
        | ParsedInput.ImplFile _ -> invalidOp "unreachable"
        | ParsedInput.SigFile file ->
            let _, logger = newLoggers state
            use _ = new CompilationGlobalsScope(logger, BuildPhase.TypeCheck)

            let checkForErrors () =
                parse.ParseHadErrors || logger.ErrorCount > 0

            let amap = state.tcImports.GetImportMap()

            let sink =
                TcResultsSinkImpl(state.tcGlobals, ?sourceText = DeclarationSyntax.sourceOf parse)

            // The fingerprint reads the compiled names of the file's values, which are given on first read
            let sigFileType, createsGeneratedProvidedTypes, fingerprint, semanticClassification =
                lock (ctx.Checks.FileLock parse.FileName) (fun () ->
                    ResetGeneratedNames(input, state)

                    let _, sigFileType, createsGeneratedProvidedTypes =
                        CheckOneSigFile
                            (state.tcGlobals,
                             amap,
                             tcState.Ccu,
                             checkForErrors,
                             conditionalDefines state,
                             TcResultsSink.WithSink sink,
                             state.tcConfig.internalTestSpanStackReferring,
                             state.tcConfig.diagnosticsOptions)
                            tcState.TcEnvFromSignatures
                            file
                        |> Cancellable.runWithoutCancellation

                    let semanticClassification =
                        sink.GetResolutions().GetSemanticClassification(state.tcGlobals, amap, sink.GetFormatSpecifierLocations(), None)

                    sigFileType, createsGeneratedProvidedTypes, signatureFingerprint sigFileType EmptyTopAttrs, semanticClassification)

            let qualNameOfFile = file.QualifiedName
            let m = input.Range

            let apply =
                AddSignatureFileToTcState(state.tcGlobals, amap, m, qualNameOfFile, sigFileType, createsGeneratedProvidedTypes)

            // As AddSignatureResultToTcImplEnv, which graph mode runs for an ArtificialImplFile node
            let asImplementation (tcState: TcState) =
                match tcState.TryFindRootSig qualNameOfFile with
                | Some rootSig ->
                    AddCheckResultsToTcState
                        (state.tcGlobals, amap, true, None, TcResultsSink.NoSink, tcState.TcEnvFromImpls, qualNameOfFile, rootSig)
                        tcState
                    |> snd
                | None -> tcState

            FileSignature(
                parse.FileName,
                apply,
                moduleNamesDict,
                signatureDefinitions parse.FileName sigFileType,
                Lazy<string>.CreateFromValue fingerprint,
                sigFileType,
                Some asImplementation,
                false,
                [|
                    for chunk in sink.GetSymbolUses().AllUsesOfSymbols do
                        for u in chunk do
                            if u.ItemOccurrence <> ItemOccurrence.RelatedText then
                                struct (u.ItemWithInst.Item, u.ItemOccurrence, u.Range)
                |],
                semanticClassification
            )

/// For tests: each access path of the entities, values, fields and union cases in the signature's module type, as
/// the fingerprint writes it and as sprintf "%A" gives it.
let accessPathTexts (signature: FileSignature) : (string * string)[] =
    let paths = ResizeArray<CompilationPath>()

    let addAccess (TAccess compPaths) =
        for compPath in compPaths do
            paths.Add compPath

    let rec walk (mty: ModuleOrNamespaceType) =
        for v in mty.AllValsAndMembers do
            addAccess v.Accessibility

        for e in mty.AllEntities do
            addAccess e.Accessibility
            addAccess e.TypeReprAccessibility

            for f in e.AllFieldsArray do
                addAccess f.Accessibility

            for uc in e.UnionCasesArray do
                addAccess uc.Accessibility

            if e.IsModuleOrNamespace then
                walk e.ModuleOrNamespaceType

    walk signature.ModuleType

    [|
        for path in paths -> SignatureFingerprint.accessPathText path, sprintf "%A" path.AccessPath
    |]

let beginFile (ctx: ProjectContext) (before: FileSignature list) (parse: FSharpParseFileResults) : FileStart * DeclarationEnvironment =
    let state = ctx.State

    match parse.ParseTree with
    | ParsedInput.SigFile _ -> invalidArg (nameof parse) "Only implementation files are checked one declaration at a time."
    | ParsedInput.ImplFile _ ->
        let moduleNamesBefore = checkBefore ctx before parse

        let tcState, moduleNamesDict, input, file =
            implFileState ctx.Checks before moduleNamesBefore parse

        let _, logger = newLoggers state
        use _ = new CompilationGlobalsScope(logger, BuildPhase.TypeCheck)
        use _ = Cancellable.UsingToken CancellationToken.None

        let sink =
            TcResultsSinkImpl(state.tcGlobals, ?sourceText = DeclarationSyntax.sourceOf parse)

        let rootSigOpt, cenv, scope =
            lock (ctx.Checks.FileLock parse.FileName) (fun () ->
                let rootSigOpt = enterImplFile state tcState input file
                let cenv, envFile, _ = newCheckerState state tcState rootSigOpt file sink
                rootSigOpt, cenv, EnterImplFileDeclarations cenv envFile file (DeclarationSyntax.splitsFragment file))

        let start =
            FileStart(
                ctx,
                parse,
                file,
                moduleNamesDict,
                tcState,
                rootSigOpt,
                scope,
                scope.Env.eModuleOrNamespaceTypeAccumulator.Value,
                sink,
                Leftovers.Of cenv,
                logger.Diagnostics
            )

        let env0 =
            DeclarationEnvironment(scope.Env, scope.Parent, scope.TypeNames, [], definitionsBefore before)

        start.EnvAtStart <- env0
        start, env0

/// Checks an implementation file whole, as a full check does, after the files before it: every earlier file, or
/// those FileDependencies gives for this parse. It keeps no per-declaration state: the result has no
/// fingerprints. It checks with no name-resolution sink, as a project check does; the result's check results are
/// built when first read (FileResult.CheckResults).
///
/// The diagnostics come from `parse`, filtered with its #nowarn scopes, as FinishFile's do.
///
/// With `fingerprint` false, nothing is expected to read the signature's fingerprint (no later file depends on
/// the file): it is taken on first read instead. It describes names and types only, nothing the file's counters
/// give, so that read needs no lock.
let checkFile (ctx: ProjectContext) (before: FileSignature list) (parse: FSharpParseFileResults) (fingerprint: bool option) : FileResult =
    let state = ctx.State

    match parse.ParseTree with
    | ParsedInput.SigFile _ -> invalidArg (nameof parse) "Expected an implementation file."
    | ParsedInput.ImplFile _ ->
        let phaseTicks = Dictionary<string, int64>()
        let phaseAllocated = Dictionary<string, int64>()
        let mutable phaseStart = System.Diagnostics.Stopwatch.GetTimestamp()
        let mutable allocatedAtStart = allocatedBytesForCurrentThread ()

        let endPhase name =
            let now = System.Diagnostics.Stopwatch.GetTimestamp()
            let allocated = allocatedBytesForCurrentThread ()
            phaseTicks[name] <- now - phaseStart
            phaseAllocated[name] <- allocated - allocatedAtStart
            phaseStart <- now
            allocatedAtStart <- allocated

        let moduleNamesBefore = checkBefore ctx before parse
        DeclarationSyntax.refuseUnlessCurrentDirectives parse
        let options = DeclarationSyntax.warnOptionsOf parse

        let tcState, moduleNamesDict, input, file =
            implFileState ctx.Checks before moduleNamesBefore parse

        let capturing, logger = newLoggersWith options
        use _ = new CompilationGlobalsScope(logger, BuildPhase.TypeCheck)
        use _ = Cancellable.UsingToken CancellationToken.None

        // The fingerprint reads the compiled names of the file's values, which are given on first read
        let signature, topAttrs =
            lock (ctx.Checks.FileLock parse.FileName) (fun () ->
                let rootSigOpt = enterImplFile state tcState input file
                endPhase "setup"

                let checkedFile =
                    leavingSignatureRanges ctx.Checks parse rootSigOpt (fun () ->
                        checkImplFile state tcState rootSigOpt file parse logger TcResultsSink.NoSink)

                endPhase "tail"

                match checkedFile with
                | Some(topAttrs, implFile, _, createsGeneratedProvidedTypes, _) ->
                    // No declaration stands for what later files see of it, so the file does, as a signature file
                    let definitions =
                        match rootSigOpt with
                        | Some _ -> Map.empty
                        | None -> signatureDefinitions parse.FileName implFile.Signature

                    endPhase "definitions"

                    let describe () =
                        implFingerprint rootSigOpt implFile.Signature topAttrs

                    let fp =
                        if defaultArg fingerprint true then
                            Lazy<string>.CreateFromValue(describe ())
                        else
                            Lazy<string>(describe, LazyThreadSafetyMode.ExecutionAndPublication)

                    endPhase "fingerprint"

                    implFileSignature
                        state
                        parse.FileName
                        file.QualifiedName
                        implFile.Signature
                        createsGeneratedProvidedTypes
                        moduleNamesDict
                        definitions
                        fp,
                    topAttrs
                | None -> failedFileSignature parse.FileName moduleNamesDict, EmptyTopAttrs)

        let diagnostics =
            Array.append parse.Diagnostics (diagnosticsWith state options parse.FileName capturing)
            |> projectDiagnostics ctx

        endPhase "render"
        // The check and the rendering read the directives: neither may have seen a newer parse's
        DeclarationSyntax.refuseUnlessCurrentDirectives parse

        // Holds the setup's inputs, not the setup it built from them, so the check state is not kept alive, and not
        // the context, which reaches every parse of the project
        let checks = ctx.Checks

        let build () =
            buildCheckResults checks before moduleNamesBefore parse diagnostics

        let checkResults =
            Lazy<FSharpCheckFileResults>(build, LazyThreadSafetyMode.ExecutionAndPublication)

        FileResult(
            diagnostics,
            Dictionary<DeclarationKey, string>(HashIdentity.Structural),
            signature,
            checkResults,
            phaseTicks,
            phaseAllocated,
            0,
            topAttrs,
            ignore,
            ValueSome build,
            ValueNone
        )
