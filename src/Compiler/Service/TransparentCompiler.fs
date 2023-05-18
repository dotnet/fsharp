namespace FSharp.Compiler.CodeAnalysis.TransparentCompiler

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO

open Internal.Utilities.Collections
open Internal.Utilities.Library

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.BuildGraph
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.Symbols
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open System.Threading.Tasks
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.GraphChecking
open FSharp.Compiler.Syntax
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.NameResolution
open Internal.Utilities.Library.Extras
open FSharp.Compiler.TypedTree
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.EditorServices
open FSharp.Compiler.CodeAnalysis

type internal FSharpFile =
    {
        Range: range
        Source: FSharpFileSnapshot
        IsLastCompiland: bool
        IsExe: bool
    }

/// Things we need to start parsing and checking files for a given project snapshot
type internal BootstrapInfo =
    {
        TcConfig: TcConfig
        TcImports: TcImports
        TcGlobals: TcGlobals
        InitialTcInfo: TcInfo
        SourceFiles: FSharpFile list
        LoadClosure: LoadClosure option
    }

    member this.GetFile fileName =
        this.SourceFiles
        |> List.tryFind (fun f -> f.Source.FileName = fileName)
        |> Option.defaultWith (fun _ ->
            failwith (
                $"File {fileName} not found in project snapshot. Files in project: \n\n"
                + (this.SourceFiles |> Seq.map (fun f -> f.Source.FileName) |> String.concat " \n")
            ))

type internal TcIntermediateResult = TcInfo * TcResultsSinkImpl * CheckedImplFile option * string

/// Accumulated results of type checking. The minimum amount of state in order to continue type-checking following files.
[<NoEquality; NoComparison>]
type internal TcIntermediate =
    {
        finisher: Finisher<TcState, PartialResult>
        //tcEnvAtEndOfFile: TcEnv

        /// Disambiguation table for module names
        moduleNamesDict: ModuleNamesDict

        /// Accumulated diagnostics, last file first
        tcDiagnosticsRev: (PhasedDiagnostic * FSharpDiagnosticSeverity)[] list

        tcDependencyFiles: string list

        sink: TcResultsSinkImpl
    }

type internal TransparentCompiler
    (
        legacyReferenceResolver,
        projectCacheSize,
        keepAssemblyContents,
        keepAllBackgroundResolutions,
        tryGetMetadataSnapshot,
        suggestNamesForErrors,
        keepAllBackgroundSymbolUses,
        enableBackgroundItemKeyStoreAndSemanticClassification,
        enablePartialTypeChecking,
        parallelReferenceResolution,
        captureIdentifiersWhenParsing,
        getSource: (string -> Async<ISourceText option>) option,
        useChangeNotifications,
        useSyntaxTreeCache
    ) =

    // Is having just one of these ok?
    let lexResourceManager = Lexhelp.LexResourceManager()

    let cacheEvent = new Event<string * JobEventType * obj>()
    let triggerCacheEvent name (e, k) = cacheEvent.Trigger(name, e, k)

    let ParseFileCache = AsyncMemoize(triggerCacheEvent "ParseFile")

    let ParseAndCheckFileInProjectCache =
        AsyncMemoize(triggerCacheEvent "ParseAndCheckFileInProject")

    let FrameworkImportsCache = AsyncMemoize(triggerCacheEvent "FrameworkImports")
    let BootstrapInfoStaticCache = AsyncMemoize(triggerCacheEvent "BootstrapInfoStatic")
    let BootstrapInfoCache = AsyncMemoize(triggerCacheEvent "BootstrapInfo")
    let TcPriorCache = AsyncMemoize(triggerCacheEvent "TcPrior")
    let TcIntermediateCache = AsyncMemoize(triggerCacheEvent "TcIntermediate")

    let DependencyGraphForLastFileCache =
        AsyncMemoize(triggerCacheEvent "DependencyGraphForLastFile")

    let SemanticClassificationCache =
        AsyncMemoize(triggerCacheEvent "SemanticClassification")

    let ItemKeyStoreCache = AsyncMemoize(triggerCacheEvent "ItemKeyStore")

    // We currently share one global dependency provider for all scripts for the FSharpChecker.
    // For projects, one is used per project.
    //
    // Sharing one for all scripts is necessary for good performance from GetProjectOptionsFromScript,
    // which requires a dependency provider to process through the project options prior to working out
    // if the cached incremental builder can be used for the project.
    let dependencyProviderForScripts = new DependencyProvider()

    // use this to process not-yet-implemented tasks
    let backgroundCompiler =
        BackgroundCompiler(
            legacyReferenceResolver,
            projectCacheSize,
            keepAssemblyContents,
            keepAllBackgroundResolutions,
            tryGetMetadataSnapshot,
            suggestNamesForErrors,
            keepAllBackgroundSymbolUses,
            enableBackgroundItemKeyStoreAndSemanticClassification,
            enablePartialTypeChecking,
            parallelReferenceResolution,
            captureIdentifiersWhenParsing,
            getSource,
            useChangeNotifications,
            useSyntaxTreeCache
        )
        :> IBackgroundCompiler

    let getProjectReferences (project: FSharpProjectSnapshot) userOpName =
        [
            for r in project.ReferencedProjects do

                match r with
                | FSharpReferencedProjectSnapshot.FSharpReference (nm, opts) ->
                    // Don't use cross-project references for FSharp.Core, since various bits of code
                    // require a concrete FSharp.Core to exist on-disk. The only solutions that have
                    // these cross-project references to FSharp.Core are VisualFSharp.sln and FSharp.sln. The ramification
                    // of this is that you need to build FSharp.Core to get intellisense in those projects.

                    if
                        (try
                            Path.GetFileNameWithoutExtension(nm)
                         with _ ->
                             "")
                        <> GetFSharpCoreLibraryName()
                    then
                        { new IProjectReference with
                            member x.EvaluateRawContents() =
                                node {
                                    Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "GetAssemblyData", nm)

                                    return!
                                        backgroundCompiler.GetAssemblyData(
                                            opts.ToOptions(),
                                            userOpName + ".CheckReferencedProject(" + nm + ")"
                                        )
                                }

                            member x.TryGetLogicalTimeStamp(cache) =
                                // TODO:
                                None

                            member x.FileName = nm
                        }
        ]

    let ComputeFrameworkImports (tcConfig: TcConfig) frameworkDLLs nonFrameworkResolutions =
        let frameworkDLLsKey =
            frameworkDLLs
            |> List.map (fun ar -> ar.resolvedPath) // The cache key. Just the minimal data.
            |> List.sort // Sort to promote cache hits.

        // The data elements in this key are very important. There should be nothing else in the TcConfig that logically affects
        // the import of a set of framework DLLs into F# CCUs. That is, the F# CCUs that result from a set of DLLs (including
        // FSharp.Core.dll and mscorlib.dll) must be logically invariant of all the other compiler configuration parameters.
        let key =
            FrameworkImportsCacheKey(
                frameworkDLLsKey,
                tcConfig.primaryAssembly.Name,
                tcConfig.GetTargetFrameworkDirectories(),
                tcConfig.fsharpBinariesDir,
                tcConfig.langVersion.SpecifiedVersion
            )

        FrameworkImportsCache.Get(
            key,
            node {
                use _ = Activity.start "ComputeFrameworkImports" []
                let tcConfigP = TcConfigProvider.Constant tcConfig
                return! TcImports.BuildFrameworkTcImports(tcConfigP, frameworkDLLs, nonFrameworkResolutions)
            }
        )

    // Link all the assemblies together and produce the input typecheck accumulator
    let CombineImportedAssembliesTask
        (
            assemblyName,
            tcConfig: TcConfig,
            tcConfigP,
            tcGlobals,
            frameworkTcImports,
            nonFrameworkResolutions,
            unresolvedReferences,
            dependencyProvider,
            loadClosureOpt: LoadClosure option,
            basicDependencies,
            importsInvalidatedByTypeProvider: Event<unit>
        ) =

        node {
            let diagnosticsLogger =
                CompilationDiagnosticLogger("CombineImportedAssembliesTask", tcConfig.diagnosticsOptions)

            use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

            let! tcImports =
                node {
                    try
                        let! tcImports =
                            TcImports.BuildNonFrameworkTcImports(
                                tcConfigP,
                                frameworkTcImports,
                                nonFrameworkResolutions,
                                unresolvedReferences,
                                dependencyProvider
                            )
#if !NO_TYPEPROVIDERS
                        // TODO: review and handle the event
                        tcImports.GetCcusExcludingBase()
                        |> Seq.iter (fun ccu ->
                            // When a CCU reports an invalidation, merge them together and just report a
                            // general "imports invalidated". This triggers a rebuild.
                            //
                            // We are explicit about what the handler closure captures to help reason about the
                            // lifetime of captured objects, especially in case the type provider instance gets leaked
                            // or keeps itself alive mistakenly, e.g. via some global state in the type provider instance.
                            //
                            // The handler only captures
                            //    1. a weak reference to the importsInvalidated event.
                            //
                            // The IncrementalBuilder holds the strong reference the importsInvalidated event.
                            //
                            // In the invalidation handler we use a weak reference to allow the IncrementalBuilder to
                            // be collected if, for some reason, a TP instance is not disposed or not GC'd.
                            let capturedImportsInvalidated = WeakReference<_>(importsInvalidatedByTypeProvider)

                            ccu.Deref.InvalidateEvent.Add(fun _ ->
                                match capturedImportsInvalidated.TryGetTarget() with
                                | true, tg -> tg.Trigger()
                                | _ -> ()))
#endif
#if NO_TYPEPROVIDERS
                        ignore importsInvalidatedByTypeProvider
#endif
                        return tcImports
                    with exn ->
                        Debug.Assert(false, sprintf "Could not BuildAllReferencedDllTcImports %A" exn)
                        diagnosticsLogger.Warning exn
                        return frameworkTcImports
                }

            let tcInitial, openDecls0 =
                GetInitialTcEnv(assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)

            let tcState =
                GetInitialTcState(rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, tcInitial, openDecls0)

            let loadClosureErrors =
                [
                    match loadClosureOpt with
                    | None -> ()
                    | Some loadClosure ->
                        for inp in loadClosure.Inputs do
                            yield! inp.MetaCommandDiagnostics
                ]

            let initialErrors =
                Array.append (Array.ofList loadClosureErrors) (diagnosticsLogger.GetDiagnostics())

            let tcInfo =
                {
                    tcState = tcState
                    tcEnvAtEndOfFile = tcInitial
                    topAttribs = None
                    latestCcuSigForFile = None
                    tcDiagnosticsRev = [ initialErrors ]
                    moduleNamesDict = Map.empty
                    tcDependencyFiles = basicDependencies
                    sigNameOpt = None
                }

            return tcImports, tcInfo
        }

    /// Bootstrap info that does not depend on contents of the files
    let ComputeBootstrapInfoStatic (projectSnapshot: FSharpProjectSnapshot) =

        let key = projectSnapshot.WithoutFileVersions.Key

        BootstrapInfoStaticCache.Get(
            key,
            node {
                use _ =
                    Activity.start
                        "ComputeBootstrapInfoStatic"
                        [| Activity.Tags.project, projectSnapshot.ProjectFileName |> Path.GetFileName |]

                let useSimpleResolutionSwitch = "--simpleresolution"
                let commandLineArgs = projectSnapshot.OtherOptions
                let defaultFSharpBinariesDir = FSharpCheckerResultsSettings.defaultFSharpBinariesDir
                let useScriptResolutionRules = projectSnapshot.UseScriptResolutionRules

                let projectReferences = getProjectReferences projectSnapshot "ComputeBootstrapInfo"

                // TODO: script support
                let loadClosureOpt: LoadClosure option = None

                let tcConfigB, sourceFiles =

                    let getSwitchValue (switchString: string) =
                        match commandLineArgs |> List.tryFindIndex (fun s -> s.StartsWithOrdinal switchString) with
                        | Some idx -> Some(commandLineArgs[ idx ].Substring(switchString.Length))
                        | _ -> None

                    let sdkDirOverride =
                        match loadClosureOpt with
                        | None -> None
                        | Some loadClosure -> loadClosure.SdkDirOverride

                    // see also fsc.fs: runFromCommandLineToImportingAssemblies(), as there are many similarities to where the PS creates a tcConfigB
                    let tcConfigB =
                        TcConfigBuilder.CreateNew(
                            legacyReferenceResolver,
                            defaultFSharpBinariesDir,
                            implicitIncludeDir = projectSnapshot.ProjectDirectory,
                            reduceMemoryUsage = ReduceMemoryFlag.Yes,
                            isInteractive = useScriptResolutionRules,
                            isInvalidationSupported = true,
                            defaultCopyFSharpCore = CopyFSharpCoreFlag.No,
                            tryGetMetadataSnapshot = tryGetMetadataSnapshot,
                            sdkDirOverride = sdkDirOverride,
                            rangeForErrors = range0
                        )

                    tcConfigB.primaryAssembly <-
                        match loadClosureOpt with
                        | None -> PrimaryAssembly.Mscorlib
                        | Some loadClosure ->
                            if loadClosure.UseDesktopFramework then
                                PrimaryAssembly.Mscorlib
                            else
                                PrimaryAssembly.System_Runtime

                    tcConfigB.resolutionEnvironment <- (LegacyResolutionEnvironment.EditingOrCompilation true)

                    tcConfigB.conditionalDefines <-
                        let define =
                            if useScriptResolutionRules then
                                "INTERACTIVE"
                            else
                                "COMPILED"

                        define :: tcConfigB.conditionalDefines

                    tcConfigB.projectReferences <- projectReferences

                    tcConfigB.useSimpleResolution <- (getSwitchValue useSimpleResolutionSwitch) |> Option.isSome

                    // Apply command-line arguments and collect more source files if they are in the arguments
                    let sourceFilesNew =
                        ApplyCommandLineArgs(tcConfigB, projectSnapshot.SourceFileNames, commandLineArgs)

                    // Never open PDB files for the language service, even if --standalone is specified
                    tcConfigB.openDebugInformationForLaterStaticLinking <- false

                    tcConfigB.xmlDocInfoLoader <-
                        { new IXmlDocumentationInfoLoader with
                            /// Try to load xml documentation associated with an assembly by the same file path with the extension ".xml".
                            member _.TryLoad(assemblyFileName) =
                                let xmlFileName = Path.ChangeExtension(assemblyFileName, ".xml")

                                // REVIEW: File IO - Will eventually need to change this to use a file system interface of some sort.
                                XmlDocumentationInfo.TryCreateFromFile(xmlFileName)
                        }
                        |> Some

                    tcConfigB.parallelReferenceResolution <- parallelReferenceResolution
                    tcConfigB.captureIdentifiersWhenParsing <- captureIdentifiersWhenParsing

                    tcConfigB, sourceFilesNew

                // If this is a builder for a script, re-apply the settings inferred from the
                // script and its load closure to the configuration.
                //
                // NOTE: it would probably be cleaner and more accurate to re-run the load closure at this point.
                let setupConfigFromLoadClosure () =
                    match loadClosureOpt with
                    | Some loadClosure ->
                        let dllReferences =
                            [
                                for reference in tcConfigB.referencedDLLs do
                                    // If there's (one or more) resolutions of closure references then yield them all
                                    match
                                        loadClosure.References
                                        |> List.tryFind (fun (resolved, _) -> resolved = reference.Text)
                                    with
                                    | Some (resolved, closureReferences) ->
                                        for closureReference in closureReferences do
                                            yield AssemblyReference(closureReference.originalReference.Range, resolved, None)
                                    | None -> yield reference
                            ]

                        tcConfigB.referencedDLLs <- []

                        tcConfigB.primaryAssembly <-
                            (if loadClosure.UseDesktopFramework then
                                 PrimaryAssembly.Mscorlib
                             else
                                 PrimaryAssembly.System_Runtime)
                        // Add one by one to remove duplicates
                        dllReferences
                        |> List.iter (fun dllReference -> tcConfigB.AddReferencedAssemblyByPath(dllReference.Range, dllReference.Text))

                        tcConfigB.knownUnresolvedReferences <- loadClosure.UnresolvedReferences
                    | None -> ()

                setupConfigFromLoadClosure ()

                let tcConfig = TcConfig.Create(tcConfigB, validate = true)
                let _outfile, _, assemblyName = tcConfigB.DecideNames sourceFiles

                // Resolve assemblies and create the framework TcImports. This caches a level of "system" references. No type providers are
                // included in these references.

                let frameworkDLLs, nonFrameworkResolutions, unresolvedReferences =
                    TcAssemblyResolutions.SplitNonFoundationalResolutions(tcConfig)

                // Prepare the frameworkTcImportsCache
                let! tcGlobals, frameworkTcImports = ComputeFrameworkImports tcConfig frameworkDLLs nonFrameworkResolutions

                // Note we are not calling diagnosticsLogger.GetDiagnostics() anywhere for this task.
                // This is ok because not much can actually go wrong here.
                let diagnosticsLogger =
                    CompilationDiagnosticLogger("nonFrameworkAssemblyInputs", tcConfig.diagnosticsOptions)

                use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

                // TODO: might need to put something like this somewhere
                //// Get the names and time stamps of all the non-framework referenced assemblies, which will act
                //// as inputs to one of the nodes in the build.
                ////
                //// This operation is done when constructing the builder itself, rather than as an incremental task.
                //let nonFrameworkAssemblyInputs =
                //    // Note we are not calling diagnosticsLogger.GetDiagnostics() anywhere for this task.
                //    // This is ok because not much can actually go wrong here.
                //    let diagnosticsLogger = CompilationDiagnosticLogger("nonFrameworkAssemblyInputs", tcConfig.diagnosticsOptions)
                //    // Return the disposable object that cleans up
                //    use _holder = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

                //    [ for r in nonFrameworkResolutions do
                //        let fileName = r.resolvedPath
                //        yield (Choice1Of2 fileName, (fun (cache: TimeStampCache) -> cache.GetFileTimeStamp fileName))

                //      for pr in projectReferences  do
                //        yield Choice2Of2 pr, (fun (cache: TimeStampCache) -> cache.GetProjectReferenceTimeStamp pr) ]

                let tcConfigP = TcConfigProvider.Constant tcConfig

                let importsInvalidatedByTypeProvider = Event<unit>()

                // Check for the existence of loaded sources and prepend them to the sources list if present.
                let sourceFiles =
                    tcConfig.GetAvailableLoadedSources()
                    @ (sourceFiles |> List.map (fun s -> rangeStartup, s))

                // Mark up the source files with an indicator flag indicating if they are the last source file in the project
                let sourceFiles =
                    let flags, isExe = tcConfig.ComputeCanContainEntryPoint(sourceFiles |> List.map snd)
                    ((sourceFiles, flags) ||> List.map2 (fun (m, nm) flag -> (m, nm, (flag, isExe))))

                let basicDependencies =
                    [
                        for UnresolvedAssemblyReference (referenceText, _) in unresolvedReferences do
                            // Exclude things that are definitely not a file name
                            if not (FileSystem.IsInvalidPathShim referenceText) then
                                let file =
                                    if FileSystem.IsPathRootedShim referenceText then
                                        referenceText
                                    else
                                        Path.Combine(projectSnapshot.ProjectDirectory, referenceText)

                                yield file

                        for r in nonFrameworkResolutions do
                            yield r.resolvedPath
                    ]

                // For scripts, the dependency provider is already available.
                // For projects create a fresh one for the project.
                let dependencyProvider =
                    if projectSnapshot.UseScriptResolutionRules then
                        dependencyProviderForScripts
                    else
                        new DependencyProvider()

                let! tcImports, initialTcInfo =
                    CombineImportedAssembliesTask(
                        assemblyName,
                        tcConfig,
                        tcConfigP,
                        tcGlobals,
                        frameworkTcImports,
                        nonFrameworkResolutions,
                        unresolvedReferences,
                        dependencyProvider,
                        loadClosureOpt,
                        basicDependencies,
                        importsInvalidatedByTypeProvider
                    )

                return sourceFiles, tcConfig, tcImports, tcGlobals, initialTcInfo, loadClosureOpt, importsInvalidatedByTypeProvider
            }
        )

    let computeBootstrapInfoInner (projectSnapshot: FSharpProjectSnapshot) =
        node {
            let! sourceFiles, tcConfig, tcImports, tcGlobals, initialTcInfo, loadClosureOpt, _importsInvalidatedByTypeProvider =
                ComputeBootstrapInfoStatic projectSnapshot

            let fileSnapshots = Map [ for f in projectSnapshot.SourceFiles -> f.FileName, f ]

            let sourceFiles =
                sourceFiles
                |> List.map (fun (m, fileName, (isLastCompiland, isExe)) ->
                    let source =
                        fileSnapshots.TryFind fileName
                        |> Option.defaultWith (fun () ->
                            // TODO: does this commonly happen?
                            {
                                FileName = fileName
                                Version = (FileSystem.GetLastWriteTimeShim fileName).Ticks.ToString()
                                GetSource = (fun () -> fileName |> File.ReadAllText |> SourceText.ofString |> Task.FromResult)
                            })

                    {
                        Range = m
                        Source = source
                        IsLastCompiland = isLastCompiland
                        IsExe = isExe
                    })

            return
                Some
                    {
                        TcConfig = tcConfig
                        TcImports = tcImports
                        TcGlobals = tcGlobals
                        InitialTcInfo = initialTcInfo
                        SourceFiles = sourceFiles
                        LoadClosure = loadClosureOpt
                    //ImportsInvalidatedByTypeProvider = importsInvalidatedByTypeProvider
                    }
        }

    let ComputeBootstrapInfo (projectSnapshot: FSharpProjectSnapshot) =
        BootstrapInfoCache.Get(
            projectSnapshot.Key,
            node {
                use _ =
                    Activity.start "ComputeBootstrapInfo" [| Activity.Tags.project, projectSnapshot.ProjectFileName |> Path.GetFileName |]

                // Trap and report diagnostics from creation.
                let delayedLogger = CapturingDiagnosticsLogger("IncrementalBuilderCreation")
                use _ = new CompilationGlobalsScope(delayedLogger, BuildPhase.Parameter)

                let! bootstrapInfoOpt =
                    node {
                        try
                            return! computeBootstrapInfoInner projectSnapshot
                        with exn ->
                            errorRecoveryNoRange exn
                            return None
                    }

                let diagnostics =
                    match bootstrapInfoOpt with
                    | Some bootstrapInfo ->
                        let diagnosticsOptions = bootstrapInfo.TcConfig.diagnosticsOptions

                        let diagnosticsLogger =
                            CompilationDiagnosticLogger("IncrementalBuilderCreation", diagnosticsOptions)

                        delayedLogger.CommitDelayedDiagnostics diagnosticsLogger
                        diagnosticsLogger.GetDiagnostics()
                    | _ -> Array.ofList delayedLogger.Diagnostics
                    |> Array.map (fun (diagnostic, severity) ->
                        FSharpDiagnostic.CreateFromException(diagnostic, severity, range.Zero, suggestNamesForErrors))

                return bootstrapInfoOpt, diagnostics
            }
        )

    let ComputeParseFile bootstrapInfo (file: FSharpFile) =
        let key = file.Source.Key, file.IsLastCompiland, file.IsExe

        ParseFileCache.Get(
            key,
            node {
                use _ =
                    Activity.start
                        "ComputeParseFile"
                        [|
                            Activity.Tags.fileName, file.Source.FileName |> Path.GetFileName
                            Activity.Tags.version, file.Source.Version
                        |]

                let tcConfig = bootstrapInfo.TcConfig

                let diagnosticsLogger =
                    CompilationDiagnosticLogger("Parse", tcConfig.diagnosticsOptions)
                // Return the disposable object that cleans up
                use _holder = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parse)

                let flags = file.IsLastCompiland, file.IsExe
                let fileName = file.Source.FileName
                let! sourceText = file.Source.GetSource() |> NodeCode.AwaitTask

                let input =
                    ParseOneInputSourceText(tcConfig, lexResourceManager, fileName, flags, diagnosticsLogger, sourceText)

                return input, diagnosticsLogger.GetDiagnostics(), sourceText
            }
        )

    let ComputeDependencyGraphForLastFile (priorSnapshot: FSharpProjectSnapshot) parsedInputs (tcConfig: TcConfig) =
        let key = priorSnapshot.SourceFiles |> List.map (fun s -> s.Key)

        DependencyGraphForLastFileCache.Get(
            key,
            node {

                let sourceFiles: FileInProject array =
                    parsedInputs
                    |> Seq.toArray
                    |> Array.mapi (fun idx (input: ParsedInput) ->
                        {
                            Idx = idx
                            FileName = input.FileName
                            ParsedInput = input
                        })

                use _ =
                    Activity.start "ComputeDependencyGraphForLastFile" [| Activity.Tags.fileName, (sourceFiles |> Array.last).FileName |]

                let filePairs = FilePairMap(sourceFiles)

                // TODO: we will probably want to cache and re-use larger graphs if available
                let graph =
                    DependencyResolution.mkGraph tcConfig.compilingFSharpCore filePairs sourceFiles
                    |> fst
                    |> Graph.subGraphFor (sourceFiles |> Array.last).Idx

                return TransformDependencyGraph(graph, filePairs)
            }
        )

    let ComputeTcIntermediate
        (projectSnapshot: FSharpProjectSnapshot)
        (fileIndex: FileIndex)
        (parsedInput: ParsedInput, parseErrors)
        bootstrapInfo
        (prevTcInfo: TcInfo)
        =
        // TODO: we need to construct cache key based on only relevant files from the dependency graph
        let key = projectSnapshot.UpTo(fileIndex).Key

        TcIntermediateCache.Get(
            key,
            node {
                let input = parsedInput
                let fileName = input.FileName

                use _ =
                    Activity.start "ComputeTcIntermediate" [| Activity.Tags.fileName, fileName |> Path.GetFileName |]

                let tcConfig = bootstrapInfo.TcConfig
                let tcGlobals = bootstrapInfo.TcGlobals
                let tcImports = bootstrapInfo.TcImports

                let capturingDiagnosticsLogger = CapturingDiagnosticsLogger("TypeCheck")

                let diagnosticsLogger =
                    GetDiagnosticsLoggerFilteringByScopedPragmas(
                        false,
                        input.ScopedPragmas,
                        tcConfig.diagnosticsOptions,
                        capturingDiagnosticsLogger
                    )

                use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.TypeCheck)

                //beforeFileChecked.Trigger fileName

                ApplyMetaCommandsFromInputToTcConfig(tcConfig, input, Path.GetDirectoryName fileName, tcImports.DependencyProvider)
                |> ignore

                let sink = TcResultsSinkImpl(tcGlobals)
                let hadParseErrors = not (Array.isEmpty parseErrors)

                let input, moduleNamesDict =
                    DeduplicateParsedInputModuleName prevTcInfo.moduleNamesDict input

                let! finisher =
                    CheckOneInputWithCallback(
                        (fun () -> hadParseErrors || diagnosticsLogger.ErrorCount > 0),
                        tcConfig,
                        tcImports,
                        tcGlobals,
                        None,
                        TcResultsSink.WithSink sink,
                        prevTcInfo.tcState,
                        input,
                        false
                    )
                    |> NodeCode.FromCancellable

                //fileChecked.Trigger fileName

                let newErrors =
                    Array.append parseErrors (capturingDiagnosticsLogger.Diagnostics |> List.toArray)

                return
                    {
                        finisher = finisher
                        moduleNamesDict = moduleNamesDict
                        tcDiagnosticsRev = [ newErrors ]
                        tcDependencyFiles = [ fileName ]
                        sink = sink
                    }
            }
        )

    let mergeIntermediateResults =
        Array.fold (fun (tcInfo: TcInfo) (tcIntermediate: TcIntermediate) ->
            let (tcEnv, topAttribs, _checkImplFileOpt, ccuSigForFile), tcState =
                tcInfo.tcState |> tcIntermediate.finisher.Invoke

            let tcEnvAtEndOfFile =
                if keepAllBackgroundResolutions then
                    tcEnv
                else
                    tcState.TcEnvFromImpls

            { tcInfo with
                tcState = tcState
                tcEnvAtEndOfFile = tcEnvAtEndOfFile
                topAttribs = Some topAttribs
                tcDiagnosticsRev = tcIntermediate.tcDiagnosticsRev @ tcInfo.tcDiagnosticsRev
                tcDependencyFiles = tcIntermediate.tcDependencyFiles @ tcInfo.tcDependencyFiles
                latestCcuSigForFile = Some ccuSigForFile
            })

    // Type check everything that is needed to check given file
    let ComputeTcPrior (file: FSharpFile) (bootstrapInfo: BootstrapInfo) (projectSnapshot: FSharpProjectSnapshot) =

        let priorSnapshot = projectSnapshot.UpTo file.Source.FileName
        let key = priorSnapshot.Key

        TcPriorCache.Get(
            key,
            node {
                use _ =
                    Activity.start "ComputeTcPrior" [| Activity.Tags.fileName, file.Source.FileName |> Path.GetFileName |]

                // parse required files
                let files =
                    seq {
                        yield! bootstrapInfo.SourceFiles |> Seq.takeWhile ((<>) file)
                        file
                    }

                let! parsedInputs = files |> Seq.map (ComputeParseFile bootstrapInfo) |> NodeCode.Parallel

                let! graph = ComputeDependencyGraphForLastFile priorSnapshot (parsedInputs |> Seq.map p13) bootstrapInfo.TcConfig

                let fileNames =
                    parsedInputs
                    |> Seq.mapi (fun idx (input, _, _) -> idx, Path.GetFileName input.FileName)
                    |> Map.ofSeq

                let debugGraph =
                    graph
                    |> Graph.map (function
                        | NodeToTypeCheck.PhysicalFile i -> i, fileNames[i]
                        | NodeToTypeCheck.ArtificialImplFile i -> -(i + 1), $"AIF : {fileNames[i]}")

                Trace.TraceInformation("\n" + (debugGraph |> Graph.serialiseToMermaid))

                // layers that can be processed in parallel
                let layers = Graph.leafSequence graph |> Seq.toList

                // remove the final layer, which should be the target file
                let layers = layers |> List.take (layers.Length - 1)

                let rec processLayer (layers: Set<NodeToTypeCheck> list) tcInfo =
                    node {
                        match layers with
                        | [] -> return tcInfo
                        | layer :: rest ->
                            let! results =
                                layer
                                |> Seq.map (fun fileNode ->

                                    match fileNode with
                                    | NodeToTypeCheck.PhysicalFile fileIndex ->
                                        let parsedInput, parseErrors, _ = parsedInputs[fileIndex]
                                        ComputeTcIntermediate projectSnapshot fileIndex (parsedInput, parseErrors) bootstrapInfo tcInfo
                                    | NodeToTypeCheck.ArtificialImplFile fileIndex ->

                                        let finisher tcState =
                                            let parsedInput, _parseErrors, _ = parsedInputs[fileIndex]
                                            let prefixPathOpt = None
                                            // Retrieve the type-checked signature information and add it to the TcEnvFromImpls.
                                            AddSignatureResultToTcImplEnv
                                                (bootstrapInfo.TcImports,
                                                 bootstrapInfo.TcGlobals,
                                                 prefixPathOpt,
                                                 TcResultsSink.NoSink,
                                                 tcState,
                                                 parsedInput)
                                                tcState

                                        let tcIntermediate =
                                            {
                                                finisher = finisher
                                                moduleNamesDict = tcInfo.moduleNamesDict
                                                tcDiagnosticsRev = []
                                                tcDependencyFiles = []
                                                sink = Unchecked.defaultof<_>
                                            }

                                        node.Return(tcIntermediate))
                                |> NodeCode.Parallel

                            return! processLayer rest (mergeIntermediateResults tcInfo results)
                    }

                return! processLayer layers bootstrapInfo.InitialTcInfo
            }
        )

    let getParseResult (bootstrapInfo: BootstrapInfo) creationDiags fileName =
        node {
            let file = bootstrapInfo.GetFile fileName

            let! parseTree, parseDiagnostics, sourceText = ComputeParseFile bootstrapInfo file

            let parseDiagnostics =
                DiagnosticHelpers.CreateDiagnostics(
                    bootstrapInfo.TcConfig.diagnosticsOptions,
                    false,
                    fileName,
                    parseDiagnostics,
                    suggestNamesForErrors
                )

            let diagnostics = [| yield! creationDiags; yield! parseDiagnostics |]

            return
                FSharpParseFileResults(
                    diagnostics = diagnostics,
                    input = parseTree,
                    parseHadErrors = (parseDiagnostics.Length > 0),
                    // TODO: check if we really need this in parse results
                    dependencyFiles = [||]
                ),
                sourceText
        }

    let emptyParseResult fileName diagnostics =
        let parseTree = EmptyParsedInput(fileName, (false, false))
        FSharpParseFileResults(diagnostics, parseTree, true, [||])

    let ComputeParseAndCheckFileInProject (fileName: string) (projectSnapshot: FSharpProjectSnapshot) =
        let key = fileName, projectSnapshot.Key

        ParseAndCheckFileInProjectCache.Get(
            key,
            node {

                use _ =
                    Activity.start "ComputeParseAndCheckFileInProject" [| Activity.Tags.fileName, fileName |> Path.GetFileName |]

                match! ComputeBootstrapInfo projectSnapshot with
                | None, creationDiags -> return emptyParseResult fileName creationDiags, FSharpCheckFileAnswer.Aborted

                | Some bootstrapInfo, creationDiags ->

                    let! parseResults, sourceText = getParseResult bootstrapInfo creationDiags fileName

                    let file = bootstrapInfo.GetFile fileName
                    let! tcInfo = ComputeTcPrior file bootstrapInfo projectSnapshot

                    let! checkResults =
                        FSharpCheckFileResults.CheckOneFile(
                            parseResults,
                            sourceText,
                            fileName,
                            projectSnapshot.ProjectFileName,
                            bootstrapInfo.TcConfig,
                            bootstrapInfo.TcGlobals,
                            bootstrapInfo.TcImports,
                            tcInfo.tcState,
                            tcInfo.moduleNamesDict,
                            bootstrapInfo.LoadClosure,
                            tcInfo.TcDiagnostics,
                            projectSnapshot.IsIncompleteTypeCheckEnvironment,
                            projectSnapshot.ToOptions(),
                            None,
                            Array.ofList tcInfo.tcDependencyFiles,
                            creationDiags,
                            parseResults.Diagnostics,
                            keepAssemblyContents,
                            suggestNamesForErrors
                        )
                        |> NodeCode.FromCancellable

                    return parseResults, FSharpCheckFileAnswer.Succeeded checkResults
            }
        )

    let tryGetSink fileName (projectSnapshot: FSharpProjectSnapshot) =
        node {
            use _ =
                Activity.start "ComputeSemanticClassification" [| Activity.Tags.fileName, fileName |> Path.GetFileName |]

            match! ComputeBootstrapInfo projectSnapshot with
            | None, _ -> return None
            | Some bootstrapInfo, _creationDiags ->

                let file = bootstrapInfo.GetFile fileName

                let! tcInfo = ComputeTcPrior file bootstrapInfo projectSnapshot
                let! parseTree, parseDiagnostics, _sourceText = ComputeParseFile bootstrapInfo file

                let fileIndex = projectSnapshot.IndexOf fileName
                let! { sink = sink } = ComputeTcIntermediate projectSnapshot fileIndex (parseTree, parseDiagnostics) bootstrapInfo tcInfo

                return Some(sink, bootstrapInfo)
        }

    let ComputeSemanticClassification (fileName: string, projectSnapshot: FSharpProjectSnapshot) =
        let key = (projectSnapshot.UpTo fileName).Key

        SemanticClassificationCache.Get(
            key,
            node {
                use _ =
                    Activity.start "ComputeSemanticClassification" [| Activity.Tags.fileName, fileName |> Path.GetFileName |]

                let! sinkOpt = tryGetSink fileName projectSnapshot

                return
                    sinkOpt
                    |> Option.bind (fun (sink, bootstrapInfo) ->
                        let sResolutions = sink.GetResolutions()

                        let semanticClassification =
                            sResolutions.GetSemanticClassification(
                                bootstrapInfo.TcGlobals,
                                bootstrapInfo.TcImports.GetImportMap(),
                                sink.GetFormatSpecifierLocations(),
                                None
                            )

                        let sckBuilder = SemanticClassificationKeyStoreBuilder()
                        sckBuilder.WriteAll semanticClassification

                        sckBuilder.TryBuildAndReset())
                    |> Option.map (fun sck -> sck.GetView())
            }
        )

    let ComputeItemKeyStore (fileName: string, projectSnapshot: FSharpProjectSnapshot) =
        let key = (projectSnapshot.UpTo fileName).Key

        ItemKeyStoreCache.Get(
            key,
            node {
                use _ =
                    Activity.start "ComputeItemKeyStore" [| Activity.Tags.fileName, fileName |> Path.GetFileName |]

                let! sinkOpt = tryGetSink fileName projectSnapshot

                return
                    sinkOpt
                    |> Option.bind (fun (sink, _) ->
                        let sResolutions = sink.GetResolutions()

                        let builder = ItemKeyStoreBuilder()

                        let preventDuplicates =
                            HashSet(
                                { new IEqualityComparer<struct (pos * pos)> with
                                    member _.Equals((s1, e1): struct (pos * pos), (s2, e2): struct (pos * pos)) =
                                        Position.posEq s1 s2 && Position.posEq e1 e2

                                    member _.GetHashCode o = o.GetHashCode()
                                }
                            )

                        sResolutions.CapturedNameResolutions
                        |> Seq.iter (fun cnr ->
                            let r = cnr.Range

                            if preventDuplicates.Add struct (r.Start, r.End) then
                                builder.Write(cnr.Range, cnr.Item))

                        builder.TryBuildAndReset())
            }
        )

    member _.ParseFile(fileName, projectSnapshot: FSharpProjectSnapshot, _userOpName) =
        node {
            use _ =
                Activity.start "ParseFile" [| Activity.Tags.fileName, fileName |> Path.GetFileName |]

            match! ComputeBootstrapInfo projectSnapshot with
            | None, creationDiags -> return emptyParseResult fileName creationDiags
            | Some bootstrapInfo, creationDiags ->
                let! parseResult, _ = getParseResult bootstrapInfo creationDiags fileName
                return parseResult
        }

    member _.ParseAndCheckFileInProject(fileName: string, projectSnapshot: FSharpProjectSnapshot, userOpName: string) =
        ignore userOpName
        ComputeParseAndCheckFileInProject fileName projectSnapshot

    member _.FindReferencesInFile(fileName: string, projectSnapshot: FSharpProjectSnapshot, symbol: FSharpSymbol, userOpName: string) =
        ignore userOpName

        node {
            match! ComputeItemKeyStore(fileName, projectSnapshot) with
            | None -> return Seq.empty
            | Some itemKeyStore -> return itemKeyStore.FindAll symbol.Item
        }

    interface IBackgroundCompiler with

        member _.CacheEvent = cacheEvent.Publish

        member this.BeforeBackgroundFileCheck: IEvent<string * FSharpProjectOptions> =
            backgroundCompiler.BeforeBackgroundFileCheck

        member _.CheckFileInProject
            (
                parseResults: FSharpParseFileResults,
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpCheckFileAnswer> =
            backgroundCompiler.CheckFileInProject(parseResults, fileName, fileVersion, sourceText, options, userOpName)

        member _.CheckFileInProjectAllowingStaleCachedResults
            (
                parseResults: FSharpParseFileResults,
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpCheckFileAnswer option> =
            backgroundCompiler.CheckFileInProjectAllowingStaleCachedResults(
                parseResults,
                fileName,
                fileVersion,
                sourceText,
                options,
                userOpName
            )

        member _.ClearCache(options: seq<FSharpProjectOptions>, userOpName: string) : unit =
            backgroundCompiler.ClearCache(options, userOpName)

        member _.ClearCaches() : unit = backgroundCompiler.ClearCaches()
        member _.DownsizeCaches() : unit = backgroundCompiler.DownsizeCaches()

        member _.FileChecked: IEvent<string * FSharpProjectOptions> =
            backgroundCompiler.FileChecked

        member _.FileParsed: IEvent<string * FSharpProjectOptions> =
            backgroundCompiler.FileParsed

        member _.FindReferencesInFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                symbol: FSharpSymbol,
                canInvalidateProject: bool,
                userOpName: string
            ) : NodeCode<seq<range>> =
            backgroundCompiler.FindReferencesInFile(fileName, options, symbol, canInvalidateProject, userOpName)

        member this.FindReferencesInFile(fileName, projectSnapshot, symbol, userOpName) =
            this.FindReferencesInFile(fileName, projectSnapshot, symbol, userOpName)

        member _.FrameworkImportsCache: FrameworkImportsCache =
            backgroundCompiler.FrameworkImportsCache

        member _.GetAssemblyData(options: FSharpProjectOptions, userOpName: string) : NodeCode<ProjectAssemblyDataResult> =
            backgroundCompiler.GetAssemblyData(options, userOpName)

        member _.GetBackgroundCheckResultsForFileInProject
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpParseFileResults * FSharpCheckFileResults> =
            backgroundCompiler.GetBackgroundCheckResultsForFileInProject(fileName, options, userOpName)

        member _.GetBackgroundParseResultsForFileInProject
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpParseFileResults> =
            backgroundCompiler.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName)

        member _.GetCachedCheckFileResult
            (
                builder: IncrementalBuilder,
                fileName: string,
                sourceText: ISourceText,
                options: FSharpProjectOptions
            ) : NodeCode<(FSharpParseFileResults * FSharpCheckFileResults) option> =
            backgroundCompiler.GetCachedCheckFileResult(builder, fileName, sourceText, options)

        member _.GetProjectOptionsFromScript
            (
                fileName: string,
                sourceText: ISourceText,
                previewEnabled: bool option,
                loadedTimeStamp: DateTime option,
                otherFlags: string array option,
                useFsiAuxLib: bool option,
                useSdkRefs: bool option,
                sdkDirOverride: string option,
                assumeDotNetFramework: bool option,
                optionsStamp: int64 option,
                userOpName: string
            ) : Async<FSharpProjectOptions * FSharpDiagnostic list> =
            backgroundCompiler.GetProjectOptionsFromScript(
                fileName,
                sourceText,
                previewEnabled,
                loadedTimeStamp,
                otherFlags,
                useFsiAuxLib,
                useSdkRefs,
                sdkDirOverride,
                assumeDotNetFramework,
                optionsStamp,
                userOpName
            )

        member _.GetSemanticClassificationForFile(fileName: string, snapshot: FSharpProjectSnapshot, userOpName: string) =
            ignore userOpName
            ComputeSemanticClassification(fileName, snapshot)

        member _.GetSemanticClassificationForFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<EditorServices.SemanticClassificationView option> =
            backgroundCompiler.GetSemanticClassificationForFile(fileName, options, userOpName)

        member _.InvalidateConfiguration(options: FSharpProjectOptions, userOpName: string) : unit =
            backgroundCompiler.InvalidateConfiguration(options, userOpName)

        member _.NotifyFileChanged(fileName: string, options: FSharpProjectOptions, userOpName: string) : NodeCode<unit> =
            backgroundCompiler.NotifyFileChanged(fileName, options, userOpName)

        member _.NotifyProjectCleaned(options: FSharpProjectOptions, userOpName: string) : Async<unit> =
            backgroundCompiler.NotifyProjectCleaned(options, userOpName)

        member _.ParseAndCheckFileInProject
            (
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : NodeCode<FSharpParseFileResults * FSharpCheckFileAnswer> =

            backgroundCompiler.ParseAndCheckFileInProject(fileName, fileVersion, sourceText, options, userOpName)

        member this.ParseAndCheckFileInProject(fileName: string, projectSnapshot: FSharpProjectSnapshot, userOpName: string) =
            this.ParseAndCheckFileInProject(fileName, projectSnapshot, userOpName)

        member _.ParseAndCheckProject(options: FSharpProjectOptions, userOpName: string) : NodeCode<FSharpCheckProjectResults> =
            backgroundCompiler.ParseAndCheckProject(options, userOpName)

        member this.ParseFile(fileName, projectSnapshot, userOpName) =
            this.ParseFile(fileName, projectSnapshot, userOpName)

        member _.ParseFile
            (
                fileName: string,
                sourceText: ISourceText,
                options: FSharpParsingOptions,
                cache: bool,
                userOpName: string
            ) : Async<FSharpParseFileResults> =
            backgroundCompiler.ParseFile(fileName, sourceText, options, cache, userOpName)

        member _.ProjectChecked: IEvent<FSharpProjectOptions> =
            backgroundCompiler.ProjectChecked

        member _.TryGetRecentCheckResultsForFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                sourceText: ISourceText option,
                userOpName: string
            ) : (FSharpParseFileResults * FSharpCheckFileResults * SourceTextHash) option =
            backgroundCompiler.TryGetRecentCheckResultsForFile(fileName, options, sourceText, userOpName)
