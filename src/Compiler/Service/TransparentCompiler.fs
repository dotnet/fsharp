namespace FSharp.Compiler.CodeAnalysis.TransparentCompiler

open System
open System.Linq
open System.Collections.Generic
open System.Runtime.CompilerServices
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
open FSharp.Compiler.CheckBasics
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
open FSharp.Compiler.CreateILModule
open FSharp.Compiler.TypedTreeOps
open System.Threading
open Internal.Utilities.Hashing

open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

/// Accumulated results of type checking. The minimum amount of state in order to continue type-checking following files.
[<NoEquality; NoComparison>]
type internal TcInfo =
    {
        tcState: TcState
        tcEnvAtEndOfFile: TcEnv

        /// Disambiguation table for module names
        moduleNamesDict: ModuleNamesDict

        topAttribs: TopAttribs option

        latestCcuSigForFile: ModuleOrNamespaceType option

        /// Accumulated diagnostics, last file first
        tcDiagnosticsRev: (PhasedDiagnostic * FSharpDiagnosticSeverity)[] list

        tcDependencyFiles: string list

        sigNameOpt: (string * QualifiedNameOfFile) option

        graphNode: NodeToTypeCheck option

        stateContainsNodes: Set<NodeToTypeCheck>

        sink: TcResultsSinkImpl list
    }

    member x.TcDiagnostics = Array.concat (List.rev x.tcDiagnosticsRev)

[<NoEquality; NoComparison>]
type internal TcIntermediate =
    {
        finisher: Finisher<NodeToTypeCheck, TcState, PartialResult>
        //tcEnvAtEndOfFile: TcEnv

        /// Disambiguation table for module names
        moduleNamesDict: ModuleNamesDict

        /// Accumulated diagnostics, last file first
        tcDiagnosticsRev: (PhasedDiagnostic * FSharpDiagnosticSeverity)[] list

        tcDependencyFiles: string list

        sink: TcResultsSinkImpl
    }

/// Things we need to start parsing and checking files for a given project snapshot
type internal BootstrapInfo =
    {
        // Each instance gets an Id on creation, unfortunately partial type check results using different instances are not compatible
        // So if this needs to be recreated for whatever reason then we need to re type check all files
        Id: int

        AssemblyName: string
        OutFile: string
        TcConfig: TcConfig
        TcImports: TcImports
        TcGlobals: TcGlobals
        InitialTcInfo: TcInfo

        // TODO: Figure out how these work and if they need to be added to the snapshot...
        LoadedSources: (range * FSharpFileSnapshot) list

        // TODO: Might be a bit more complicated if we want to support adding files to the project via OtherOptions
        // ExtraSourceFilesAfter: FSharpFileSnapshot list

        LoadClosure: LoadClosure option
        LastFileName: string
        ImportsInvalidatedByTypeProvider: Event<unit>
    }

type internal TcIntermediateResult = TcInfo * TcResultsSinkImpl * CheckedImplFile option * string

[<RequireQualifiedAccess>]
type internal DependencyGraphType =
    /// A dependency graph for a single file - it will be missing files which this file does not depend on
    | File
    /// A dependency graph for a project - it will contain all files in the project
    | Project

[<Extension>]
type internal Extensions =
    [<Extension>]
    static member Key<'T when 'T :> IFileSnapshot>(fileSnapshots: 'T list, ?extraKeyFlag) =

        { new ICacheKey<_, _> with
            member _.GetLabel() =
                let lastFile =
                    fileSnapshots
                    |> List.tryLast
                    |> Option.map (fun f -> f.FileName |> shortPath)
                    |> Option.defaultValue "[no file]"

                $"%d{fileSnapshots.Length} files ending with {lastFile}"

            member _.GetKey() =
                Md5Hasher.empty
                |> Md5Hasher.addStrings (fileSnapshots |> Seq.map (fun f -> f.FileName))
                |> pair extraKeyFlag

            member _.GetVersion() =
                Md5Hasher.empty
                |> Md5Hasher.addBytes' (fileSnapshots |> Seq.map (fun f -> f.Version))
                |> Md5Hasher.toString
        }

[<AutoOpen>]
module private TypeCheckingGraphProcessing =
    open FSharp.Compiler.GraphChecking.GraphProcessing

    // TODO Do we need to suppress some error logging if we
    // TODO apply the same partial results multiple times?
    // TODO Maybe we can enable logging only for the final fold
    /// <summary>
    /// Combine type-checking results of dependencies needed to type-check a 'higher' node in the graph
    /// </summary>
    /// <param name="emptyState">Initial state</param>
    /// <param name="deps">Direct dependencies of a node</param>
    /// <param name="transitiveDeps">Transitive dependencies of a node</param>
    /// <param name="folder">A way to fold a single result into existing state</param>
    let private combineResults
        (emptyState: TcInfo)
        (deps: ProcessedNode<NodeToTypeCheck, TcInfo * Finisher<NodeToTypeCheck, TcInfo, PartialResult>> array)
        (transitiveDeps: ProcessedNode<NodeToTypeCheck, TcInfo * Finisher<NodeToTypeCheck, TcInfo, PartialResult>> array)
        (folder: TcInfo -> Finisher<NodeToTypeCheck, TcInfo, PartialResult> -> TcInfo)
        : TcInfo =
        match deps with
        | [||] -> emptyState
        | _ ->
            // Instead of starting with empty state,
            // reuse state produced by the dependency with the biggest number of transitive dependencies.
            // This is to reduce the number of folds required to achieve the final state.
            let biggestDependency =
                let sizeMetric (node: ProcessedNode<_, _>) = node.Info.TransitiveDeps.Length
                deps |> Array.maxBy sizeMetric

            let firstState = biggestDependency.Result |> fst

            // Find items not already included in the state.
            let itemsPresent =
                set
                    [|
                        yield! biggestDependency.Info.TransitiveDeps
                        yield biggestDependency.Info.Item
                    |]

            let resultsToAdd =
                transitiveDeps
                |> Array.filter (fun dep -> itemsPresent.Contains dep.Info.Item = false)
                |> Array.distinctBy (fun dep -> dep.Info.Item)
                |> Array.sortWith (fun a b ->
                    // We preserve the order in which items are folded to the state.
                    match a.Info.Item, b.Info.Item with
                    | NodeToTypeCheck.PhysicalFile aIdx, NodeToTypeCheck.PhysicalFile bIdx
                    | NodeToTypeCheck.ArtificialImplFile aIdx, NodeToTypeCheck.ArtificialImplFile bIdx -> aIdx.CompareTo bIdx
                    | NodeToTypeCheck.PhysicalFile _, NodeToTypeCheck.ArtificialImplFile _ -> -1
                    | NodeToTypeCheck.ArtificialImplFile _, NodeToTypeCheck.PhysicalFile _ -> 1)
                |> Array.map (fun dep -> dep.Result |> snd)

            // Fold results not already included and produce the final state
            let state = Array.fold folder firstState resultsToAdd
            state

    /// <summary>
    /// Process a graph of items.
    /// A version of 'GraphProcessing.processGraph' with a signature specific to type-checking.
    /// </summary>
    let processTypeCheckingGraph
        (graph: Graph<NodeToTypeCheck>)
        (work: NodeToTypeCheck -> TcInfo -> Async<Finisher<NodeToTypeCheck, TcInfo, PartialResult>>)
        (emptyState: TcInfo)
        : Async<(int * PartialResult) list * TcInfo> =
        async {

            let workWrapper
                (getProcessedNode:
                    NodeToTypeCheck -> ProcessedNode<NodeToTypeCheck, TcInfo * Finisher<NodeToTypeCheck, TcInfo, PartialResult>>)
                (node: NodeInfo<NodeToTypeCheck>)
                : Async<TcInfo * Finisher<NodeToTypeCheck, TcInfo, PartialResult>> =
                async {
                    let folder (state: TcInfo) (Finisher(finisher = finisher)) : TcInfo = finisher state |> snd
                    let deps = node.Deps |> Array.except [| node.Item |] |> Array.map getProcessedNode

                    let transitiveDeps =
                        node.TransitiveDeps
                        |> Array.except [| node.Item |]
                        |> Array.map getProcessedNode

                    let inputState = combineResults emptyState deps transitiveDeps folder

                    let! singleRes = work node.Item inputState
                    let state = folder inputState singleRes
                    return state, singleRes
                }

            let! results = processGraphAsync graph workWrapper

            let finalFileResults, state =
                (([], emptyState),
                 results
                 |> Array.choose (fun (item, res) ->
                     match item with
                     | NodeToTypeCheck.ArtificialImplFile _ -> None
                     | NodeToTypeCheck.PhysicalFile file -> Some(file, res)))
                ||> Array.fold (fun (fileResults, state) (item, (_, Finisher(finisher = finisher))) ->
                    let fileResult, state = finisher state
                    (item, fileResult) :: fileResults, state)

            return finalFileResults, state
        }

type CacheSizes =
    {
        ParseFileKeepStrongly: int
        ParseFileKeepWeakly: int
        ParseFileWithoutProjectKeepStrongly: int
        ParseFileWithoutProjectKeepWeakly: int
        ParseAndCheckFileInProjectKeepStrongly: int
        ParseAndCheckFileInProjectKeepWeakly: int
        ParseAndCheckAllFilesInProjectKeepStrongly: int
        ParseAndCheckAllFilesInProjectKeepWeakly: int
        ParseAndCheckProjectKeepStrongly: int
        ParseAndCheckProjectKeepWeakly: int
        FrameworkImportsKeepStrongly: int
        FrameworkImportsKeepWeakly: int
        BootstrapInfoStaticKeepStrongly: int
        BootstrapInfoStaticKeepWeakly: int
        BootstrapInfoKeepStrongly: int
        BootstrapInfoKeepWeakly: int
        TcLastFileKeepStrongly: int
        TcLastFileKeepWeakly: int
        TcIntermediateKeepStrongly: int
        TcIntermediateKeepWeakly: int
        DependencyGraphKeepStrongly: int
        DependencyGraphKeepWeakly: int
        ProjectExtrasKeepStrongly: int
        ProjectExtrasKeepWeakly: int
        AssemblyDataKeepStrongly: int
        AssemblyDataKeepWeakly: int
        SemanticClassificationKeepStrongly: int
        SemanticClassificationKeepWeakly: int
        ItemKeyStoreKeepStrongly: int
        ItemKeyStoreKeepWeakly: int
        ScriptClosureKeepStrongly: int
        ScriptClosureKeepWeakly: int
    }

    static member Create sizeFactor =

        {
            ParseFileKeepStrongly = 50 * sizeFactor
            ParseFileKeepWeakly = 20 * sizeFactor
            ParseFileWithoutProjectKeepStrongly = 5 * sizeFactor
            ParseFileWithoutProjectKeepWeakly = 2 * sizeFactor
            ParseAndCheckFileInProjectKeepStrongly = sizeFactor
            ParseAndCheckFileInProjectKeepWeakly = 2 * sizeFactor
            ParseAndCheckAllFilesInProjectKeepStrongly = sizeFactor
            ParseAndCheckAllFilesInProjectKeepWeakly = 2 * sizeFactor
            ParseAndCheckProjectKeepStrongly = sizeFactor
            ParseAndCheckProjectKeepWeakly = 2 * sizeFactor
            FrameworkImportsKeepStrongly = sizeFactor
            FrameworkImportsKeepWeakly = 2 * sizeFactor
            BootstrapInfoStaticKeepStrongly = sizeFactor
            BootstrapInfoStaticKeepWeakly = 2 * sizeFactor
            BootstrapInfoKeepStrongly = sizeFactor
            BootstrapInfoKeepWeakly = 2 * sizeFactor
            TcLastFileKeepStrongly = sizeFactor
            TcLastFileKeepWeakly = 2 * sizeFactor
            TcIntermediateKeepStrongly = 20 * sizeFactor
            TcIntermediateKeepWeakly = 20 * sizeFactor
            DependencyGraphKeepStrongly = sizeFactor
            DependencyGraphKeepWeakly = 2 * sizeFactor
            ProjectExtrasKeepStrongly = sizeFactor
            ProjectExtrasKeepWeakly = 2 * sizeFactor
            AssemblyDataKeepStrongly = sizeFactor
            AssemblyDataKeepWeakly = 2 * sizeFactor
            SemanticClassificationKeepStrongly = sizeFactor
            SemanticClassificationKeepWeakly = 2 * sizeFactor
            ItemKeyStoreKeepStrongly = sizeFactor
            ItemKeyStoreKeepWeakly = 2 * sizeFactor
            ScriptClosureKeepStrongly = sizeFactor
            ScriptClosureKeepWeakly = 2 * sizeFactor
        }

    static member Default =
        let sizeFactor = 100
        CacheSizes.Create sizeFactor

type internal CompilerCaches(cacheSizes: CacheSizes) =
    let cs = cacheSizes

    member _.CacheSizes = cs

    member val ParseFile = AsyncMemoize(keepStrongly = cs.ParseFileKeepStrongly, keepWeakly = cs.ParseFileKeepWeakly, name = "ParseFile")

    member val ParseFileWithoutProject =
        AsyncMemoize<string, string, FSharpParseFileResults>(
            cs.ParseFileWithoutProjectKeepStrongly,
            keepWeakly = cs.ParseFileWithoutProjectKeepWeakly,
            name = "ParseFileWithoutProject"
        )

    member val ParseAndCheckFileInProject =
        AsyncMemoize(
            cs.ParseAndCheckFileInProjectKeepStrongly,
            cs.ParseAndCheckFileInProjectKeepWeakly,
            name = "ParseAndCheckFileInProject"
        )

    member val ParseAndCheckAllFilesInProject =
        AsyncMemoizeDisabled(
            cs.ParseAndCheckAllFilesInProjectKeepStrongly,
            cs.ParseAndCheckAllFilesInProjectKeepWeakly,
            name = "ParseAndCheckFullProject"
        )

    member val ParseAndCheckProject =
        AsyncMemoize(cs.ParseAndCheckProjectKeepStrongly, cs.ParseAndCheckProjectKeepWeakly, name = "ParseAndCheckProject")

    member val FrameworkImports = AsyncMemoize(cs.FrameworkImportsKeepStrongly, cs.FrameworkImportsKeepWeakly, name = "FrameworkImports")

    member val BootstrapInfoStatic =
        AsyncMemoize(cs.BootstrapInfoStaticKeepStrongly, cs.BootstrapInfoStaticKeepWeakly, name = "BootstrapInfoStatic")

    member val BootstrapInfo = AsyncMemoize(cs.BootstrapInfoKeepStrongly, cs.BootstrapInfoKeepWeakly, name = "BootstrapInfo")

    member val TcLastFile = AsyncMemoizeDisabled(cs.TcLastFileKeepStrongly, cs.TcLastFileKeepWeakly, name = "TcLastFile")

    member val TcIntermediate = AsyncMemoize(cs.TcIntermediateKeepStrongly, cs.TcIntermediateKeepWeakly, name = "TcIntermediate")

    member val DependencyGraph = AsyncMemoize(cs.DependencyGraphKeepStrongly, cs.DependencyGraphKeepWeakly, name = "DependencyGraph")

    member val ProjectExtras = AsyncMemoizeDisabled(cs.ProjectExtrasKeepStrongly, cs.ProjectExtrasKeepWeakly, name = "ProjectExtras")

    member val AssemblyData = AsyncMemoize(cs.AssemblyDataKeepStrongly, cs.AssemblyDataKeepWeakly, name = "AssemblyData")

    member val SemanticClassification =
        AsyncMemoize(cs.SemanticClassificationKeepStrongly, cs.SemanticClassificationKeepWeakly, name = "SemanticClassification")

    member val ItemKeyStore = AsyncMemoize(cs.ItemKeyStoreKeepStrongly, cs.ItemKeyStoreKeepWeakly, name = "ItemKeyStore")

    member val ScriptClosure = AsyncMemoize(cs.ScriptClosureKeepStrongly, cs.ScriptClosureKeepWeakly, name = "ScriptClosure")

    member this.Clear(projects: Set<FSharpProjectIdentifier>) =
        let shouldClear project = projects |> Set.contains project

        this.ParseFile.Clear(fst >> shouldClear)
        this.ParseAndCheckFileInProject.Clear(snd >> shouldClear)
        this.ParseAndCheckProject.Clear(shouldClear)
        this.BootstrapInfoStatic.Clear(shouldClear)
        this.BootstrapInfo.Clear(shouldClear)
        this.TcIntermediate.Clear(snd >> shouldClear)
        this.AssemblyData.Clear(shouldClear)
        this.SemanticClassification.Clear(snd >> shouldClear)
        this.ItemKeyStore.Clear(snd >> shouldClear)
        this.ScriptClosure.Clear(snd >> shouldClear)

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
        ?cacheSizes
    ) as self =

    let documentSource =
        match getSource with
        | Some getSource -> DocumentSource.Custom getSource
        | None -> DocumentSource.FileSystem

    // Is having just one of these ok?
    let lexResourceManager = Lexhelp.LexResourceManager()

    let cacheSizes = defaultArg cacheSizes CacheSizes.Default

    // Mutable so we can easily clear them by creating a new instance
    let mutable caches = CompilerCaches(cacheSizes)

    // TODO: do we need this?
    //let maxTypeCheckingParallelism = max 1 (Environment.ProcessorCount / 2)
    //let maxParallelismSemaphore = new SemaphoreSlim(maxTypeCheckingParallelism)

    // We currently share one global dependency provider for all scripts for the FSharpChecker.
    // For projects, one is used per project.
    //
    // Sharing one for all scripts is necessary for good performance from GetProjectOptionsFromScript,
    // which requires a dependency provider to process through the project options prior to working out
    // if the cached incremental builder can be used for the project.
    let dependencyProviderForScripts = new DependencyProvider()

    // Legacy events, they're used in tests... eventually they should go away
    let beforeFileChecked = Event<string * FSharpProjectOptions>()
    let fileParsed = Event<string * FSharpProjectOptions>()
    let fileChecked = Event<string * FSharpProjectOptions>()
    let projectChecked = Event<FSharpProjectOptions>()

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
            useChangeNotifications
        )
        :> IBackgroundCompiler

    let ComputeScriptClosureInner
        (fileName: string)
        (source: ISourceTextNew)
        (defaultFSharpBinariesDir: string)
        (useSimpleResolution: bool)
        (useFsiAuxLib: bool)
        (useSdkRefs: bool)
        (sdkDirOverride: string option)
        (assumeDotNetFramework: bool)
        (otherOptions: string list)
        =
        let reduceMemoryUsage = ReduceMemoryFlag.Yes

        let applyCompilerOptions tcConfig =
            let fsiCompilerOptions = GetCoreFsiCompilerOptions tcConfig
            ParseCompilerOptions(ignore, fsiCompilerOptions, otherOptions)

        let closure =
            LoadClosure.ComputeClosureOfScriptText(
                legacyReferenceResolver,
                defaultFSharpBinariesDir,
                fileName,
                source,
                CodeContext.Editing,
                useSimpleResolution,
                useFsiAuxLib,
                useSdkRefs,
                sdkDirOverride,
                Lexhelp.LexResourceManager(),
                applyCompilerOptions,
                assumeDotNetFramework,
                tryGetMetadataSnapshot,
                reduceMemoryUsage,
                dependencyProviderForScripts
            )

        closure

    let mkScriptClosureCacheKey
        (fileName: string)
        (source: ISourceTextNew)
        (useSimpleResolution: bool)
        (useFsiAuxLib: bool)
        (useSdkRefs: bool)
        (assumeDotNetFramework: bool)
        (projectIdentifier: FSharpProjectIdentifier)
        (otherOptions: string list)
        (stamp: int64 option)
        =
        { new ICacheKey<string * FSharpProjectIdentifier, string> with
            member _.GetKey() = fileName, projectIdentifier
            member _.GetLabel() = $"ScriptClosure for %s{fileName}"

            member _.GetVersion() =
                Md5Hasher.empty
                |> Md5Hasher.addStrings
                    [|
                        yield! otherOptions
                        match stamp with
                        | None -> ()
                        | Some stamp -> string stamp
                    |]
                |> Md5Hasher.addBytes (source.GetChecksum().ToArray())
                |> Md5Hasher.addBool useSimpleResolution
                |> Md5Hasher.addBool useFsiAuxLib
                |> Md5Hasher.addBool useSdkRefs
                |> Md5Hasher.addBool assumeDotNetFramework
                |> Md5Hasher.toString
        }

    let ComputeScriptClosure
        (fileName: string)
        (source: ISourceTextNew)
        (defaultFSharpBinariesDir: string)
        (useSimpleResolution: bool)
        (useFsiAuxLib: bool option)
        (useSdkRefs: bool option)
        (sdkDirOverride: string option)
        (assumeDotNetFramework: bool option)
        (projectIdentifier: FSharpProjectIdentifier)
        (otherOptions: string list)
        (stamp: int64 option)
        =
        let useFsiAuxLib = defaultArg useFsiAuxLib true
        let useSdkRefs = defaultArg useSdkRefs true
        let assumeDotNetFramework = defaultArg assumeDotNetFramework false

        let key =
            mkScriptClosureCacheKey
                fileName
                source
                useSimpleResolution
                useFsiAuxLib
                useSdkRefs
                assumeDotNetFramework
                projectIdentifier
                otherOptions
                stamp

        caches.ScriptClosure.Get(
            key,
            async {
                return
                    ComputeScriptClosureInner
                        fileName
                        source
                        defaultFSharpBinariesDir
                        useSimpleResolution
                        useFsiAuxLib
                        useSdkRefs
                        sdkDirOverride
                        assumeDotNetFramework
                        otherOptions
            }
        )

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
                tcConfig.langVersion.SpecifiedVersion,
                tcConfig.checkNullness
            )

        caches.FrameworkImports.Get(
            key,
            async {
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

        async {
            let diagnosticsLogger =
                CompilationDiagnosticLogger("CombineImportedAssembliesTask", tcConfig.diagnosticsOptions)

            use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

            let! tcImports =
                async {
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
                    with
                    | :? OperationCanceledException ->
                        // if it's been canceled then it shouldn't be needed anymore
                        return frameworkTcImports
                    | exn ->
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
                    graphNode = None
                    stateContainsNodes = Set.empty
                    sink = []
                }

            return tcImports, tcInfo
        }

    let getProjectReferences (project: ProjectSnapshotBase<_>) userOpName =
        [
            for r in project.ReferencedProjects do

                match r with
                | FSharpReferencedProjectSnapshot.FSharpReference(nm, projectSnapshot) ->
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
                                async {
                                    Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "GetAssemblyData", nm)

                                    return!
                                        self.GetAssemblyData(
                                            projectSnapshot.ProjectSnapshot,
                                            nm,
                                            userOpName + ".CheckReferencedProject(" + nm + ")"
                                        )
                                }

                            member x.TryGetLogicalTimeStamp(cache) =
                                // TODO:
                                None

                            member x.FileName = nm
                        }
                | FSharpReferencedProjectSnapshot.PEReference(getStamp, delayedReader) ->
                    { new IProjectReference with
                        member x.EvaluateRawContents() =
                            cancellable {
                                let! ilReaderOpt = delayedReader.TryGetILModuleReader()

                                match ilReaderOpt with
                                | Some ilReader ->
                                    let ilModuleDef, ilAsmRefs = ilReader.ILModuleDef, ilReader.ILAssemblyRefs
                                    let data = RawFSharpAssemblyData(ilModuleDef, ilAsmRefs) :> IRawFSharpAssemblyData
                                    return ProjectAssemblyDataResult.Available data
                                | _ ->
                                    // Note 'false' - if a PEReference doesn't find an ILModuleReader then we don't
                                    // continue to try to use an on-disk DLL
                                    return ProjectAssemblyDataResult.Unavailable false
                            }
                            |> Cancellable.toAsync

                        member x.TryGetLogicalTimeStamp _ = getStamp () |> Some
                        member x.FileName = delayedReader.OutputFile
                    }

                | FSharpReferencedProjectSnapshot.ILModuleReference(nm, getStamp, getReader) ->
                    { new IProjectReference with
                        member x.EvaluateRawContents() =
                            cancellable {
                                let ilReader = getReader ()
                                let ilModuleDef, ilAsmRefs = ilReader.ILModuleDef, ilReader.ILAssemblyRefs
                                let data = RawFSharpAssemblyData(ilModuleDef, ilAsmRefs) :> IRawFSharpAssemblyData
                                return ProjectAssemblyDataResult.Available data
                            }
                            |> Cancellable.toAsync

                        member x.TryGetLogicalTimeStamp _ = getStamp () |> Some
                        member x.FileName = nm
                    }
        ]

    let ComputeTcConfigBuilder (projectSnapshot: ProjectSnapshot) =
        async {
            let useSimpleResolutionSwitch = "--simpleresolution"
            let commandLineArgs = projectSnapshot.CommandLineOptions
            let defaultFSharpBinariesDir = FSharpCheckerResultsSettings.defaultFSharpBinariesDir
            let useScriptResolutionRules = projectSnapshot.UseScriptResolutionRules

            let projectReferences =
                getProjectReferences projectSnapshot "ComputeTcConfigBuilder"

            let getSwitchValue (switchString: string) =
                match commandLineArgs |> List.tryFindIndex (fun s -> s.StartsWithOrdinal switchString) with
                | Some idx -> Some(commandLineArgs[idx].Substring(switchString.Length))
                | _ -> None

            let useSimpleResolution =
                (getSwitchValue useSimpleResolutionSwitch) |> Option.isSome

            let! (loadClosureOpt: LoadClosure option) =
                let lastScriptFile =
                    match List.tryLast projectSnapshot.SourceFiles with
                    | None -> None
                    | Some file -> if IsScript file.FileName then Some file else None

                match lastScriptFile, projectSnapshot.UseScriptResolutionRules with
                | Some fsxFile, true -> // assuming UseScriptResolutionRules and a single source file means we are doing this for a script
                    async {
                        let! source = fsxFile.GetSource() |> Async.AwaitTask

                        let! closure =
                            ComputeScriptClosure
                                fsxFile.FileName
                                source
                                defaultFSharpBinariesDir
                                useSimpleResolution
                                None
                                None
                                None
                                None
                                projectSnapshot.Identifier
                                projectSnapshot.OtherOptions
                                projectSnapshot.Stamp

                        return (Some closure)
                    }
                | _ -> async { return None }

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

            tcConfigB.realsig <-
                List.contains "--realsig" commandLineArgs
                || List.contains "--realsig+" commandLineArgs

            tcConfigB.projectReferences <- projectReferences

            tcConfigB.useSimpleResolution <- useSimpleResolution

            // Apply command-line arguments and collect more source files if they are in the arguments
            let sourceFilesNew =
                ApplyCommandLineArgs(tcConfigB, projectSnapshot.SourceFileNames, commandLineArgs)

            // Never open PDB files for the language service, even if --standalone is specified
            tcConfigB.openDebugInformationForLaterStaticLinking <- false

            tcConfigB.xmlDocInfoLoader <-
                { new IXmlDocumentationInfoLoader with
                    /// Try to load xml documentation associated with an assembly by the same file path with the extension ".xml".
                    member _.TryLoad(assemblyFileName) =
                        let xmlFileName = !! Path.ChangeExtension(assemblyFileName, ".xml")

                        // REVIEW: File IO - Will eventually need to change this to use a file system interface of some sort.
                        XmlDocumentationInfo.TryCreateFromFile(xmlFileName)
                }
                |> Some

            tcConfigB.parallelReferenceResolution <- parallelReferenceResolution
            tcConfigB.captureIdentifiersWhenParsing <- captureIdentifiersWhenParsing

            return tcConfigB, sourceFilesNew, loadClosureOpt
        }

    let mutable BootstrapInfoIdCounter = 0

    /// Bootstrap info that does not depend source files
    let ComputeBootstrapInfoStatic (projectSnapshot: ProjectSnapshotBase<_>, tcConfig: TcConfig, assemblyName: string, loadClosureOpt) =
        let cacheKey = projectSnapshot.BaseCacheKeyWith("BootstrapInfoStatic", assemblyName)

        caches.BootstrapInfoStatic.Get(
            cacheKey,
            async {
                use _ =
                    Activity.start
                        "ComputeBootstrapInfoStatic"
                        [|
                            Activity.Tags.project, projectSnapshot.ProjectFileName |> Path.GetFileName |> (!!)
                            "references", projectSnapshot.ReferencedProjects.Length.ToString()
                        |]

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

                let tcConfigP = TcConfigProvider.Constant tcConfig

                let importsInvalidatedByTypeProvider = Event<unit>()

                let basicDependencies =
                    [
                        for UnresolvedAssemblyReference(referenceText, _) in unresolvedReferences do
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

                let bootstrapId = Interlocked.Increment &BootstrapInfoIdCounter

                // TODO: In the future it might make sense to expose the event on the ProjectSnapshot and let the consumer deal with this.
                // We could include a timestamp as part of the ProjectSnapshot key that represents the last time since the TypeProvider assembly was invalidated.
                // When the event trigger, the consumer could then create a new snapshot based on the updated time.
                importsInvalidatedByTypeProvider.Publish.Add(fun () -> caches.Clear(Set.singleton projectSnapshot.Identifier))

                return bootstrapId, tcImports, tcGlobals, initialTcInfo, importsInvalidatedByTypeProvider
            }
        )

    let computeBootstrapInfoInner (projectSnapshot: ProjectSnapshot) =
        async {

            let! tcConfigB, sourceFiles, loadClosureOpt = ComputeTcConfigBuilder projectSnapshot

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
                                | Some(resolved, closureReferences) ->
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
            let outFile, _, assemblyName = tcConfigB.DecideNames sourceFiles

            let! bootstrapId, tcImports, tcGlobals, initialTcInfo, importsInvalidatedByTypeProvider =
                ComputeBootstrapInfoStatic(projectSnapshot, tcConfig, assemblyName, loadClosureOpt)

            // Check for the existence of loaded sources and prepend them to the sources list if present.
            let loadedSources =
                tcConfig.GetAvailableLoadedSources()
                |> List.map (fun (m, fileName) -> m, FSharpFileSnapshot.CreateFromFileSystem(fileName))

            return
                match sourceFiles with
                | [] -> None
                | _ ->
                    Some
                        {
                            Id = bootstrapId
                            AssemblyName = assemblyName
                            OutFile = outFile
                            TcConfig = tcConfig
                            TcImports = tcImports
                            TcGlobals = tcGlobals
                            InitialTcInfo = initialTcInfo
                            LoadedSources = loadedSources
                            LoadClosure = loadClosureOpt
                            LastFileName = sourceFiles |> List.tryLast |> Option.defaultValue ""
                            ImportsInvalidatedByTypeProvider = importsInvalidatedByTypeProvider
                        }
        }

    let ComputeBootstrapInfo (projectSnapshot: ProjectSnapshot) =

        caches.BootstrapInfo.Get(
            projectSnapshot.NoFileVersionsKey,
            async {
                use _ =
                    Activity.start
                        "ComputeBootstrapInfo"
                        [|
                            Activity.Tags.project, projectSnapshot.ProjectFileName |> Path.GetFileName |> (!!)
                        |]

                // Trap and report diagnostics from creation.
                let delayedLogger = CapturingDiagnosticsLogger("IncrementalBuilderCreation")
                use _ = new CompilationGlobalsScope(delayedLogger, BuildPhase.Parameter)

                let! bootstrapInfoOpt =
                    async {
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
                        let flatErrors =
                            bootstrapInfoOpt
                            |> Option.map (fun bootstrapInfo -> bootstrapInfo.TcConfig.flatErrors)
                            |> Option.defaultValue false // TODO: do we need to figure this out?

                        FSharpDiagnostic.CreateFromException(diagnostic, severity, range.Zero, suggestNamesForErrors, flatErrors, None))

                return bootstrapInfoOpt, diagnostics
            }
        )

    // TODO: Not sure if we should cache this. For VS probably not. Maybe it can be configurable by FCS user.
    let LoadSource (file: FSharpFileSnapshot) isExe isLastCompiland =
        async {
            let! source = file.GetSource() |> Async.AwaitTask

            return
                FSharpFileSnapshotWithSource(
                    FileName = file.FileName,
                    Source = source,
                    SourceHash = source.GetChecksum(),
                    IsLastCompiland = isLastCompiland,
                    IsExe = isExe
                )
        }

    let LoadSources (bootstrapInfo: BootstrapInfo) (projectSnapshot: ProjectSnapshot) =
        async {
            let isExe = bootstrapInfo.TcConfig.target.IsExe

            let! sources =
                projectSnapshot.SourceFiles
                |> Seq.map (fun f -> LoadSource f isExe (f.FileName = bootstrapInfo.LastFileName))
                |> MultipleDiagnosticsLoggers.Parallel

            return ProjectSnapshotWithSources(projectSnapshot.ProjectConfig, projectSnapshot.ReferencedProjects, sources |> Array.toList)

        }

    let ComputeParseFile (projectSnapshot: ProjectSnapshotBase<_>) (tcConfig: TcConfig) (file: FSharpFileSnapshotWithSource) =

        let key =
            { new ICacheKey<_, _> with
                member _.GetLabel() = file.FileName |> shortPath

                member _.GetKey() =
                    projectSnapshot.ProjectConfig.Identifier, file.FileName

                member _.GetVersion() =
                    projectSnapshot.ParsingVersion,
                    file.StringVersion,
                    // TODO: is there a situation where this is not enough and we need to have them separate?
                    file.IsLastCompiland && file.IsExe
            }

        caches.ParseFile.Get(
            key,
            async {
                use _ =
                    Activity.start
                        "ComputeParseFile"
                        [|
                            Activity.Tags.fileName, file.FileName |> shortPath
                            Activity.Tags.version, file.StringVersion
                        |]

                let diagnosticsLogger =
                    CompilationDiagnosticLogger("Parse", tcConfig.diagnosticsOptions)
                // Return the disposable object that cleans up
                use _holder = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parse)

                let flags = file.IsLastCompiland, file.IsExe
                let fileName = file.FileName
                let sourceText = file.Source

                let input =
                    ParseOneInputSourceText(tcConfig, lexResourceManager, fileName, flags, diagnosticsLogger, sourceText)

                // TODO: Hashing of syntax tree
                let inputHash = file.Version

                fileParsed.Trigger(fileName, Unchecked.defaultof<_>)

                return FSharpParsedFile(fileName, inputHash, sourceText, input, diagnosticsLogger.GetDiagnostics())
            }
        )

    // In case we don't want to use any parallel processing
    let mkLinearGraph count : Graph<FileIndex> =
        seq {
            0, [||]

            yield!
                [ 0 .. count - 1 ]
                |> Seq.rev
                |> Seq.pairwise
                |> Seq.map (fun (a, b) -> a, [| b |])
        }
        |> Graph.make

    let computeDependencyGraph (tcConfig: TcConfig) parsedInputs (processGraph: Graph<FileIndex> -> Graph<FileIndex>) =
        async {
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
                Activity.start "ComputeDependencyGraph" [| Activity.Tags.fileName, (sourceFiles |> Array.last).FileName |]

            let filePairs = FilePairMap(sourceFiles)

            // TODO: we will probably want to cache and re-use larger graphs if available

            let graph =
                if tcConfig.compilingFSharpCore then
                    mkLinearGraph sourceFiles.Length
                else
                    DependencyResolution.mkGraph filePairs sourceFiles |> fst |> processGraph

            let nodeGraph = TransformDependencyGraph(graph, filePairs)

            let fileNames =
                parsedInputs
                |> Seq.mapi (fun idx input -> idx, Path.GetFileName input.FileName)
                |> Map.ofSeq

            let debugGraph =
                nodeGraph
                |> Graph.map (function
                    | NodeToTypeCheck.PhysicalFile i -> i, $"[{i}] {fileNames[i]}"
                    | NodeToTypeCheck.ArtificialImplFile i -> -(i + 1), $"AIF [{i}] : {fileNames[i]}")
                |> Graph.serialiseToMermaid

            //Trace.TraceInformation("\n" + debugGraph)

            match Activity.Current with
            | Null -> ()
            | NonNull a -> a.AddTag("graph", debugGraph) |> ignore

            return nodeGraph, graph
        }

    let removeImplFilesThatHaveSignatures (projectSnapshot: ProjectSnapshot) (graph: Graph<FileIndex>) =

        let removeIndexes =
            projectSnapshot.SourceFileNames
            |> Seq.mapi pair
            |> Seq.groupBy (
                snd
                >> (fun fileName ->
                    if fileName.EndsWith(".fsi") then
                        fileName.Substring(0, fileName.Length - 1)
                    else
                        fileName)
            )
            |> Seq.map (snd >> (Seq.toList))
            |> Seq.choose (function
                | [ idx1, _; idx2, _ ] -> max idx1 idx2 |> Some
                | _ -> None)
            |> Set

        graph
        |> Seq.filter (fun x -> not (removeIndexes.Contains x.Key))
        |> Seq.map (fun x -> x.Key, x.Value |> Array.filter (fun node -> not (removeIndexes.Contains node)))
        |> Graph.make

    let removeImplFilesThatHaveSignaturesExceptLastOne (projectSnapshot: ProjectSnapshotBase<_>) (graph: Graph<FileIndex>) =

        let removeIndexes =
            projectSnapshot.SourceFileNames
            |> Seq.mapi pair
            |> Seq.groupBy (
                snd
                >> (fun fileName ->
                    if fileName.EndsWith(".fsi") then
                        fileName.Substring(0, fileName.Length - 1)
                    else
                        fileName)
            )
            |> Seq.map (snd >> (Seq.toList))
            |> Seq.choose (function
                | [ idx1, _; idx2, _ ] -> max idx1 idx2 |> Some
                | _ -> None)
            |> Set
            // Don't remove the last file
            |> Set.remove (projectSnapshot.SourceFiles.Length - 1)

        graph
        |> Seq.filter (fun x -> not (removeIndexes.Contains x.Key))
        |> Seq.map (fun x -> x.Key, x.Value |> Array.filter (fun node -> not (removeIndexes.Contains node)))
        |> Graph.make

    let ComputeDependencyGraphForFile (tcConfig: TcConfig) (priorSnapshot: ProjectSnapshotBase<FSharpParsedFile>) =
        let key = priorSnapshot.SourceFiles.Key(DependencyGraphType.File)
        //let lastFileIndex = (parsedInputs |> Array.length) - 1
        //caches.DependencyGraph.Get(key, computeDependencyGraph parsedInputs (Graph.subGraphFor lastFileIndex))
        caches.DependencyGraph.Get(
            key,
            computeDependencyGraph
                tcConfig
                (priorSnapshot.SourceFiles |> Seq.map (fun f -> f.ParsedInput))
                (removeImplFilesThatHaveSignaturesExceptLastOne priorSnapshot)
        )

    let ComputeDependencyGraphForProject (tcConfig: TcConfig) (projectSnapshot: ProjectSnapshotBase<FSharpParsedFile>) =

        let key = projectSnapshot.SourceFiles.Key(DependencyGraphType.Project)
        //caches.DependencyGraph.Get(key, computeDependencyGraph parsedInputs (removeImplFilesThatHaveSignatures projectSnapshot))
        caches.DependencyGraph.Get(
            key,
            computeDependencyGraph tcConfig (projectSnapshot.SourceFiles |> Seq.map (fun f -> f.ParsedInput)) id
        )

    let ComputeTcIntermediate
        (projectSnapshot: ProjectSnapshotBase<FSharpParsedFile>)
        (dependencyGraph: Graph<FileIndex>)
        (index: FileIndex)
        (nodeToCheck: NodeToTypeCheck)
        bootstrapInfo
        (prevTcInfo: TcInfo)
        =

        ignore dependencyGraph

        let key = projectSnapshot.FileKey(index).WithExtraVersion(bootstrapInfo.Id)

        let _label, _k, _version = key.GetLabel(), key.GetKey(), key.GetVersion()

        caches.TcIntermediate.Get(
            key,
            async {

                let file = projectSnapshot.SourceFiles[index]

                let input = file.ParsedInput
                let fileName = file.FileName

                use _ =
                    Activity.start
                        "ComputeTcIntermediate"
                        [|
                            Activity.Tags.fileName, fileName |> Path.GetFileName |> (!!)
                            "key", key.GetLabel()
                            "version", "-" // key.GetVersion()
                        |]

                beforeFileChecked.Trigger(fileName, Unchecked.defaultof<_>)

                let tcConfig = bootstrapInfo.TcConfig
                let tcGlobals = bootstrapInfo.TcGlobals
                let tcImports = bootstrapInfo.TcImports

                let mainInputFileName = file.FileName
                let sourceText = file.SourceText
                let parsedMainInput = file.ParsedInput

                // Initialize the error handler
                let errHandler =
                    ParseAndCheckFile.DiagnosticsHandler(
                        true,
                        mainInputFileName,
                        tcConfig.diagnosticsOptions,
                        sourceText,
                        suggestNamesForErrors,
                        tcConfig.flatErrors
                    )

                // Apply nowarns to tcConfig (may generate errors, so ensure diagnosticsLogger is installed)
                let tcConfig =
                    ApplyNoWarnsToTcConfig(tcConfig, parsedMainInput, !! Path.GetDirectoryName(mainInputFileName))

                let diagnosticsLogger = errHandler.DiagnosticsLogger

                let diagnosticsLogger =
                    GetDiagnosticsLoggerFilteringByScopedPragmas(false, input.ScopedPragmas, tcConfig.diagnosticsOptions, diagnosticsLogger)

                use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.TypeCheck)

                //beforeFileChecked.Trigger fileName

                ApplyMetaCommandsFromInputToTcConfig(tcConfig, input, Path.GetDirectoryName fileName |> (!!), tcImports.DependencyProvider)
                |> ignore

                let sink = TcResultsSinkImpl(tcGlobals, file.SourceText)

                let hadParseErrors =
                    file.ParseDiagnostics
                    |> Array.exists (snd >> (=) FSharpDiagnosticSeverity.Error)

                let input, moduleNamesDict =
                    DeduplicateParsedInputModuleName prevTcInfo.moduleNamesDict input

                //let! ct = NodeCode.CancellationToken

                try
                    //do! maxParallelismSemaphore.WaitAsync(ct) |> NodeCode.AwaitTask

                    let! finisher =
                        CheckOneInputWithCallback
                            nodeToCheck
                            ((fun () -> hadParseErrors || diagnosticsLogger.CheckForRealErrorsIgnoringWarnings),
                             tcConfig,
                             tcImports,
                             tcGlobals,
                             None,
                             TcResultsSink.WithSink sink,
                             prevTcInfo.tcState,
                             input,
                             true)
                        |> Cancellable.toAsync

                    //fileChecked.Trigger fileName

                    fileChecked.Trigger(fileName, Unchecked.defaultof<_>)

                    return
                        {
                            finisher = finisher
                            moduleNamesDict = moduleNamesDict
                            tcDiagnosticsRev = [ errHandler.CollectedPhasedDiagnostics ]
                            tcDependencyFiles = [ fileName ]
                            sink = sink
                        }
                finally
                    ()
            //maxParallelismSemaphore.Release() |> ignore
            }
        )

    let processGraphNode projectSnapshot bootstrapInfo dependencyFiles collectSinks (fileNode: NodeToTypeCheck) tcInfo =
        // TODO: should this be node?
        async {
            match fileNode with
            | NodeToTypeCheck.PhysicalFile index ->

                let! tcIntermediate = ComputeTcIntermediate projectSnapshot dependencyFiles index fileNode bootstrapInfo tcInfo

                let (Finisher(node = node; finisher = finisher)) = tcIntermediate.finisher

                return
                    Finisher(
                        node,
                        (fun tcInfo ->

                            let partialResult, tcState = finisher tcInfo.tcState

                            let tcEnv, topAttribs, _checkImplFileOpt, ccuSigForFile = partialResult

                            let tcEnvAtEndOfFile =
                                if keepAllBackgroundResolutions then
                                    tcEnv
                                else
                                    tcState.TcEnvFromImpls

                            partialResult,
                            { tcInfo with
                                tcState = tcState
                                tcEnvAtEndOfFile = tcEnvAtEndOfFile
                                moduleNamesDict = tcIntermediate.moduleNamesDict
                                topAttribs = Some topAttribs
                                tcDiagnosticsRev = tcIntermediate.tcDiagnosticsRev @ tcInfo.tcDiagnosticsRev
                                tcDependencyFiles = tcIntermediate.tcDependencyFiles @ tcInfo.tcDependencyFiles
                                latestCcuSigForFile = Some ccuSigForFile
                                graphNode = Some node
                                stateContainsNodes = tcInfo.stateContainsNodes |> Set.add node
                                sink =
                                    if collectSinks then
                                        tcIntermediate.sink :: tcInfo.sink
                                    else
                                        [ tcIntermediate.sink ]
                            })
                    )

            | NodeToTypeCheck.ArtificialImplFile index ->
                return
                    Finisher(
                        fileNode,
                        (fun tcInfo ->

                            let parsedInput = projectSnapshot.SourceFiles[index].ParsedInput
                            let prefixPathOpt = None
                            // Retrieve the type-checked signature information and add it to the TcEnvFromImpls.
                            let partialResult, tcState =
                                AddSignatureResultToTcImplEnv
                                    (bootstrapInfo.TcImports,
                                     bootstrapInfo.TcGlobals,
                                     prefixPathOpt,
                                     TcResultsSink.NoSink,
                                     tcInfo.tcState,
                                     parsedInput)
                                    tcInfo.tcState

                            let tcEnv, topAttribs, _checkImplFileOpt, ccuSigForFile = partialResult

                            let tcEnvAtEndOfFile =
                                if keepAllBackgroundResolutions then
                                    tcEnv
                                else
                                    tcState.TcEnvFromImpls

                            partialResult,
                            { tcInfo with
                                tcState = tcState
                                tcEnvAtEndOfFile = tcEnvAtEndOfFile
                                topAttribs = Some topAttribs
                                latestCcuSigForFile = Some ccuSigForFile
                                graphNode = Some fileNode
                                stateContainsNodes = tcInfo.stateContainsNodes |> Set.add fileNode
                            })
                    )

        }

    let parseSourceFiles (projectSnapshot: ProjectSnapshotWithSources) tcConfig =
        async {
            let! parsedInputs =
                projectSnapshot.SourceFiles
                |> Seq.map (ComputeParseFile projectSnapshot tcConfig)
                |> MultipleDiagnosticsLoggers.Parallel

            return ProjectSnapshotBase<_>(projectSnapshot.ProjectConfig, projectSnapshot.ReferencedProjects, parsedInputs |> Array.toList)
        }

    // Type check file and all its dependencies
    let ComputeTcLastFile (bootstrapInfo: BootstrapInfo) (projectSnapshot: ProjectSnapshotWithSources) =
        let fileName = projectSnapshot.SourceFiles |> List.last |> (fun f -> f.FileName)

        caches.TcLastFile.Get(
            projectSnapshot.FileKey fileName,
            async {
                let file = projectSnapshot.SourceFiles |> List.last

                use _ =
                    Activity.start "ComputeTcLastFile" [| Activity.Tags.fileName, file.FileName |> Path.GetFileName |> (!!) |]

                let! projectSnapshot = parseSourceFiles projectSnapshot bootstrapInfo.TcConfig

                let! graph, dependencyFiles = ComputeDependencyGraphForFile bootstrapInfo.TcConfig projectSnapshot

                let! results, tcInfo =
                    processTypeCheckingGraph
                        graph
                        (processGraphNode projectSnapshot bootstrapInfo dependencyFiles false)
                        bootstrapInfo.InitialTcInfo

                let lastResult = results |> List.head |> snd

                return lastResult, tcInfo
            }
        )

    let getParseResult (projectSnapshot: ProjectSnapshot) creationDiags file (tcConfig: TcConfig) =
        async {
            let! parsedFile = ComputeParseFile projectSnapshot tcConfig file

            let parseDiagnostics =
                DiagnosticHelpers.CreateDiagnostics(
                    tcConfig.diagnosticsOptions,
                    false,
                    file.FileName,
                    parsedFile.ParseDiagnostics,
                    suggestNamesForErrors,
                    tcConfig.flatErrors,
                    None
                )

            let diagnostics = [| yield! creationDiags; yield! parseDiagnostics |]

            return
                FSharpParseFileResults(
                    diagnostics = diagnostics,
                    input = parsedFile.ParsedInput,
                    parseHadErrors = (parseDiagnostics.Length > 0),
                    // TODO: check if we really need this in parse results
                    dependencyFiles = [||]
                )
        }

    let emptyParseResult fileName diagnostics =
        let parseTree = EmptyParsedInput(fileName, (false, false))
        FSharpParseFileResults(diagnostics, parseTree, true, [||])

    let ComputeParseAndCheckFileInProject (fileName: string) (projectSnapshot: ProjectSnapshot) =
        caches.ParseAndCheckFileInProject.Get(
            projectSnapshot.FileKeyWithExtraFileSnapshotVersion fileName,
            async {
                use! _holder = Cancellable.UseToken()

                use _ =
                    Activity.start "ComputeParseAndCheckFileInProject" [| Activity.Tags.fileName, fileName |> Path.GetFileName |> (!!) |]

                match! ComputeBootstrapInfo projectSnapshot with
                | None, creationDiags -> return emptyParseResult fileName creationDiags, FSharpCheckFileAnswer.Aborted

                | Some bootstrapInfo, creationDiags ->

                    let priorSnapshot = projectSnapshot.UpTo fileName
                    let! snapshotWithSources = LoadSources bootstrapInfo priorSnapshot
                    let file = snapshotWithSources.SourceFiles |> List.last

                    let! parseResults = getParseResult projectSnapshot Seq.empty file bootstrapInfo.TcConfig

                    let! result, tcInfo = ComputeTcLastFile bootstrapInfo snapshotWithSources

                    let (tcEnv, _topAttribs, checkedImplFileOpt, ccuSigForFile) = result

                    let tcState = tcInfo.tcState

                    let sink = tcInfo.sink.Head // TODO: don't use head

                    let tcResolutions = sink.GetResolutions()
                    let tcSymbolUses = sink.GetSymbolUses()
                    let tcOpenDeclarations = sink.GetOpenDeclarations()

                    // TODO: Apparently creating diagnostics can produce further diagnostics. So let's capture those too. Hopefully there is a more elegant solution...
                    // Probably diagnostics need to be evaluated during typecheck anyway for proper formatting, which might take care of this too.
                    let extraLogger = CapturingDiagnosticsLogger("DiagnosticsWhileCreatingDiagnostics")
                    use _ = new CompilationGlobalsScope(extraLogger, BuildPhase.TypeCheck)

                    // Apply nowarns to tcConfig (may generate errors, so ensure diagnosticsLogger is installed)
                    let tcConfig =
                        ApplyNoWarnsToTcConfig(bootstrapInfo.TcConfig, parseResults.ParseTree, Path.GetDirectoryName fileName |> (!!))

                    let diagnosticsOptions = tcConfig.diagnosticsOptions

                    let symbolEnv =
                        SymbolEnv(bootstrapInfo.TcGlobals, tcState.Ccu, Some tcState.CcuSig, bootstrapInfo.TcImports)

                    let tcDiagnostics =
                        DiagnosticHelpers.CreateDiagnostics(
                            diagnosticsOptions,
                            false,
                            fileName,
                            tcInfo.TcDiagnostics,
                            suggestNamesForErrors,
                            bootstrapInfo.TcConfig.flatErrors,
                            Some symbolEnv
                        )

                    let extraDiagnostics =
                        DiagnosticHelpers.CreateDiagnostics(
                            diagnosticsOptions,
                            false,
                            fileName,
                            extraLogger.Diagnostics,
                            suggestNamesForErrors,
                            bootstrapInfo.TcConfig.flatErrors,
                            Some symbolEnv
                        )

                    let tcDiagnostics = [| yield! extraDiagnostics; yield! tcDiagnostics |]

                    let! loadClosure =
                        ComputeScriptClosure
                            fileName
                            file.Source
                            tcConfig.fsharpBinariesDir
                            tcConfig.useSimpleResolution
                            (Some tcConfig.useFsiAuxLib)
                            (Some tcConfig.useSdkRefs)
                            tcConfig.sdkDirOverride
                            (Some tcConfig.assumeDotNetFramework)
                            projectSnapshot.Identifier
                            projectSnapshot.OtherOptions
                            projectSnapshot.Stamp

                    let typedResults =
                        FSharpCheckFileResults.Make(
                            fileName,
                            projectSnapshot.ProjectFileName,
                            bootstrapInfo.TcConfig,
                            bootstrapInfo.TcGlobals,
                            projectSnapshot.IsIncompleteTypeCheckEnvironment,
                            None,
                            None,
                            Array.ofList tcInfo.tcDependencyFiles,
                            creationDiags,
                            parseResults.Diagnostics,
                            tcDiagnostics,
                            keepAssemblyContents,
                            ccuSigForFile,
                            tcState.Ccu,
                            bootstrapInfo.TcImports,
                            tcEnv.AccessRights,
                            tcResolutions,
                            tcSymbolUses,
                            tcEnv.NameEnv,
                            Some loadClosure,
                            checkedImplFileOpt,
                            tcOpenDeclarations
                        )

                    return (parseResults, FSharpCheckFileAnswer.Succeeded typedResults)
            }
        )

    let ComputeParseAndCheckAllFilesInProject (bootstrapInfo: BootstrapInfo) (projectSnapshot: ProjectSnapshotWithSources) =
        caches.ParseAndCheckAllFilesInProject.Get(
            projectSnapshot.FullKey,
            async {
                use _ =
                    Activity.start
                        "ComputeParseAndCheckAllFilesInProject"
                        [|
                            Activity.Tags.project, projectSnapshot.ProjectFileName |> Path.GetFileName |> (!!)
                        |]

                let! projectSnapshot = parseSourceFiles projectSnapshot bootstrapInfo.TcConfig

                let! graph, dependencyFiles = ComputeDependencyGraphForProject bootstrapInfo.TcConfig projectSnapshot

                return!
                    processTypeCheckingGraph
                        graph
                        (processGraphNode projectSnapshot bootstrapInfo dependencyFiles true)
                        bootstrapInfo.InitialTcInfo
            }
        )

    let TryGetRecentCheckResultsForFile
        (
            fileName: string,
            projectSnapshot: FSharpProjectSnapshot,
            userOpName: string
        ) : (FSharpParseFileResults * FSharpCheckFileResults) option =
        ignore userOpName

        let cacheKey =
            projectSnapshot.ProjectSnapshot.FileKeyWithExtraFileSnapshotVersion fileName

        let version = cacheKey.GetVersion()

        let parseFileResultsAndcheckFileAnswer =
            caches.ParseAndCheckFileInProject.TryGet(
                cacheKey.GetKey(),
                (fun (_fullVersion, fileContentVersion) -> fileContentVersion = (snd version))
            )

        match parseFileResultsAndcheckFileAnswer with
        | Some(parseFileResults, FSharpCheckFileAnswer.Succeeded checkFileResults) -> Some(parseFileResults, checkFileResults)
        | _ -> None

    let ComputeProjectExtras (bootstrapInfo: BootstrapInfo) (projectSnapshot: ProjectSnapshotWithSources) =
        caches.ProjectExtras.Get(
            projectSnapshot.SignatureKey,
            async {

                let! results, finalInfo = ComputeParseAndCheckAllFilesInProject bootstrapInfo projectSnapshot

                let assemblyName = bootstrapInfo.AssemblyName
                let tcConfig = bootstrapInfo.TcConfig
                let tcGlobals = bootstrapInfo.TcGlobals

                let results = results |> Seq.sortBy fst |> Seq.map snd |> Seq.toList

                // Finish the checking
                let (_tcEnvAtEndOfLastFile, topAttrs, checkedImplFiles, _), tcState =
                    CheckMultipleInputsFinish(results, finalInfo.tcState)

                let tcState, _, ccuContents = CheckClosedInputSetFinish([], tcState)

                let generatedCcu = tcState.Ccu.CloneWithFinalizedContents(ccuContents)

                // Compute the identity of the generated assembly based on attributes, options etc.
                // Some of this is duplicated from fsc.fs
                let ilAssemRef =
                    let publicKey =
                        try
                            let signingInfo = ValidateKeySigningAttributes(tcConfig, tcGlobals, topAttrs)

                            match GetStrongNameSigner signingInfo with
                            | None -> None
                            | Some s -> Some(PublicKey.KeyAsToken(s.PublicKey))
                        with exn ->
                            errorRecoveryNoRange exn
                            None

                    let locale =
                        TryFindFSharpStringAttribute
                            tcGlobals
                            (tcGlobals.FindSysAttrib "System.Reflection.AssemblyCultureAttribute")
                            topAttrs.assemblyAttrs

                    let assemVerFromAttrib =
                        TryFindFSharpStringAttribute
                            tcGlobals
                            (tcGlobals.FindSysAttrib "System.Reflection.AssemblyVersionAttribute")
                            topAttrs.assemblyAttrs
                        |> Option.bind (fun v ->
                            try
                                Some(parseILVersion v)
                            with _ ->
                                None)

                    let ver =
                        match assemVerFromAttrib with
                        | None -> tcConfig.version.GetVersionInfo(tcConfig.implicitIncludeDir)
                        | Some v -> v

                    ILAssemblyRef.Create(assemblyName, None, publicKey, false, Some ver, locale)

                let assemblyDataResult =
                    try
                        // Assemblies containing type provider components cannot successfully be used via cross-assembly references.
                        // We return 'None' for the assembly portion of the cross-assembly reference
                        let hasTypeProviderAssemblyAttrib =
                            topAttrs.assemblyAttrs
                            |> List.exists (fun (Attrib(tcref, _, _, _, _, _, _)) ->
                                let nm = tcref.CompiledRepresentationForNamedType.BasicQualifiedName

                                nm = !!typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderAssemblyAttribute>.FullName)

                        if tcState.CreatesGeneratedProvidedTypes || hasTypeProviderAssemblyAttrib then
                            ProjectAssemblyDataResult.Unavailable true
                        else
                            ProjectAssemblyDataResult.Available(
                                RawFSharpAssemblyDataBackedByLanguageService(
                                    bootstrapInfo.TcConfig,
                                    bootstrapInfo.TcGlobals,
                                    generatedCcu,
                                    bootstrapInfo.OutFile,
                                    topAttrs,
                                    bootstrapInfo.AssemblyName,
                                    ilAssemRef
                                )
                                :> IRawFSharpAssemblyData
                            )
                    with exn ->
                        errorRecoveryNoRange exn
                        ProjectAssemblyDataResult.Unavailable true

                return finalInfo, ilAssemRef, assemblyDataResult, checkedImplFiles
            }
        )

    let ComputeAssemblyData (projectSnapshot: ProjectSnapshot) fileName =
        caches.AssemblyData.Get(
            projectSnapshot.SignatureKey,
            async {
                use! _holder = Cancellable.UseToken()

                try

                    let availableOnDiskModifiedTime =
                        if FileSystem.FileExistsShim fileName then
                            Some <| FileSystem.GetLastWriteTimeShim fileName
                        else
                            None

                    // TODO: This kinda works, but the problem is that in order to switch a project to "in-memory" mode
                    //  - some file needs to be edited (this triggers a re-check, but LastModifiedTimeOnDisk won't change)
                    //  - saved (this will not trigger anything)
                    //  - and then another change has to be made (to any file buffer) - so that recheck is triggered and we get here again
                    // Until that sequence happens the project will be used from disk (if available).
                    // To get around it we probably need to detect changes made in the editor and record a timestamp for them.
                    let shouldUseOnDisk =
                        availableOnDiskModifiedTime
                        |> Option.exists (fun t -> t >= projectSnapshot.GetLastModifiedTimeOnDisk())

                    let name = projectSnapshot.ProjectFileName |> Path.GetFileNameWithoutExtension

                    if shouldUseOnDisk then
                        Trace.TraceInformation($"Using assembly on disk: {name}")
                        return ProjectAssemblyDataResult.Unavailable true
                    else
                        match! ComputeBootstrapInfo projectSnapshot with
                        | None, _ ->
                            Trace.TraceInformation($"Using assembly on disk (unintentionally): {name}")
                            return ProjectAssemblyDataResult.Unavailable true
                        | Some bootstrapInfo, _creationDiags ->

                            let! snapshotWithSources = LoadSources bootstrapInfo projectSnapshot

                            let! _, _, assemblyDataResult, _ = ComputeProjectExtras bootstrapInfo snapshotWithSources
                            Trace.TraceInformation($"Using in-memory project reference: {name}")

                            return assemblyDataResult
                with ex ->
                    errorR (exn ($"Error while computing assembly data for project {projectSnapshot.Label}: {ex}"))
                    return ProjectAssemblyDataResult.Unavailable true
            }
        )

    let ComputeParseAndCheckProject (projectSnapshot: ProjectSnapshot) =
        caches.ParseAndCheckProject.Get(
            projectSnapshot.FullKey,
            async {
                use! _holder = Cancellable.UseToken()

                match! ComputeBootstrapInfo projectSnapshot with
                | None, creationDiags ->
                    return FSharpCheckProjectResults(projectSnapshot.ProjectFileName, None, keepAssemblyContents, creationDiags, None)
                | Some bootstrapInfo, creationDiags ->
                    let! snapshotWithSources = LoadSources bootstrapInfo projectSnapshot

                    let! tcInfo, ilAssemRef, assemblyDataResult, checkedImplFiles = ComputeProjectExtras bootstrapInfo snapshotWithSources

                    let diagnosticsOptions = bootstrapInfo.TcConfig.diagnosticsOptions
                    let fileName = DummyFileNameForRangesWithoutASpecificLocation

                    let topAttribs = tcInfo.topAttribs
                    let tcState = tcInfo.tcState
                    let tcEnvAtEnd = tcInfo.tcEnvAtEndOfFile
                    let tcDiagnostics = tcInfo.TcDiagnostics
                    let tcDependencyFiles = tcInfo.tcDependencyFiles

                    let symbolEnv =
                        SymbolEnv(bootstrapInfo.TcGlobals, tcInfo.tcState.Ccu, Some tcInfo.tcState.CcuSig, bootstrapInfo.TcImports)
                        |> Some

                    let tcDiagnostics =
                        DiagnosticHelpers.CreateDiagnostics(
                            diagnosticsOptions,
                            true,
                            fileName,
                            tcDiagnostics,
                            suggestNamesForErrors,
                            bootstrapInfo.TcConfig.flatErrors,
                            symbolEnv
                        )

                    let diagnostics = [| yield! creationDiags; yield! tcDiagnostics |]

                    let getAssemblyData () =
                        match assemblyDataResult with
                        | ProjectAssemblyDataResult.Available data -> Some data
                        | _ -> None

                    let symbolUses =
                        tcInfo.sink |> Seq.rev |> Seq.map (fun sink -> sink.GetSymbolUses())

                    let details =
                        (bootstrapInfo.TcGlobals,
                         bootstrapInfo.TcImports,
                         tcState.Ccu,
                         tcState.CcuSig,
                         Choice2Of2(async.Return symbolUses),
                         topAttribs,
                         getAssemblyData,
                         ilAssemRef,
                         tcEnvAtEnd.AccessRights,
                         Some checkedImplFiles,
                         Array.ofList tcDependencyFiles,
                         None)

                    let results =
                        FSharpCheckProjectResults(
                            projectSnapshot.ProjectFileName,
                            Some bootstrapInfo.TcConfig,
                            keepAssemblyContents,
                            diagnostics,
                            Some details
                        )

                    return results
            }
        )

    let tryGetSink (fileName: string) (projectSnapshot: ProjectSnapshot) =
        async {
            use! _holder = Cancellable.UseToken()

            match! ComputeBootstrapInfo projectSnapshot with
            | None, _ -> return None
            | Some bootstrapInfo, _creationDiags ->

                let! snapshotWithSources = projectSnapshot.UpTo fileName |> LoadSources bootstrapInfo

                let! _, tcInfo = ComputeTcLastFile bootstrapInfo snapshotWithSources

                return tcInfo.sink |> List.tryHead |> Option.map (fun sink -> sink, bootstrapInfo)
        }

    let ComputeSemanticClassification (fileName: string, projectSnapshot: ProjectSnapshot) =
        caches.SemanticClassification.Get(
            projectSnapshot.FileKey fileName,
            async {
                use _ =
                    Activity.start "ComputeSemanticClassification" [| Activity.Tags.fileName, fileName |> Path.GetFileName |> (!!) |]

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

    let ComputeItemKeyStore (fileName: string, projectSnapshot: ProjectSnapshot) =
        caches.ItemKeyStore.Get(
            projectSnapshot.FileKey fileName,
            async {
                use _ =
                    Activity.start "ComputeItemKeyStore" [| Activity.Tags.fileName, fileName |> Path.GetFileName |> (!!) |]

                let! sinkOpt = tryGetSink fileName projectSnapshot

                return
                    sinkOpt
                    |> Option.bind (fun (sink, { TcGlobals = g }) ->
                        let sResolutions = sink.GetResolutions()

                        let builder = ItemKeyStoreBuilder(g)

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

    member _.ParseFile(fileName, projectSnapshot: ProjectSnapshot, _userOpName) =
        async {
            //use _ =
            //    Activity.start "ParseFile" [| Activity.Tags.fileName, fileName |> Path.GetFileName |]

            // TODO: might need to deal with exceptions here:
            use _ = new CompilationGlobalsScope(DiscardErrorsLogger, BuildPhase.Parse)
            let! tcConfigB, sourceFileNames, _ = ComputeTcConfigBuilder projectSnapshot

            let tcConfig = TcConfig.Create(tcConfigB, validate = true)

            let _index, fileSnapshot =
                projectSnapshot.SourceFiles
                |> Seq.mapi pair
                |> Seq.tryFind (fun (_, f) -> f.FileName = fileName)
                |> Option.defaultWith (fun () -> failwith $"File not found: {fileName}")

            let isExe = tcConfig.target.IsExe
            let isLastCompiland = fileName = (sourceFileNames |> List.last)

            let! file = LoadSource fileSnapshot isExe isLastCompiland
            let! parseResult = getParseResult projectSnapshot Seq.empty file tcConfig
            return parseResult
        }

    member _.ParseFileWithoutProject
        (
            fileName: string,
            sourceText: ISourceText,
            options: FSharpParsingOptions,
            cache: bool,
            flatErrors: bool,
            userOpName: string
        ) : Async<FSharpParseFileResults> =
        let parseFileAsync =
            async {
                let! ct = Async.CancellationToken

                let diagnostics, parsedInput, anyErrors =
                    ParseAndCheckFile.parseFile (sourceText, fileName, options, userOpName, false, flatErrors, false, ct)

                return FSharpParseFileResults(diagnostics, parsedInput, anyErrors, Array.empty)
            }

        if not cache then
            parseFileAsync
        else
            let cacheKey =
                let sourceText = SourceTextNew.ofISourceText sourceText

                { new ICacheKey<_, _> with
                    member _.GetLabel() = shortPath fileName

                    member _.GetKey() = fileName

                    member _.GetVersion() =
                        Md5Hasher.empty
                        |> Md5Hasher.addStrings
                            [
                                yield fileName
                                yield! options.ConditionalDefines
                                yield! options.SourceFiles
                                yield options.LangVersionText
                            ]
                        |> Md5Hasher.addBytes (sourceText.GetChecksum().ToArray())
                        |> Md5Hasher.addIntegers
                            [
                                yield options.DiagnosticOptions.WarnLevel
                                yield! options.DiagnosticOptions.WarnOff
                                yield! options.DiagnosticOptions.WarnOn
                                yield! options.DiagnosticOptions.WarnAsError
                                yield! options.DiagnosticOptions.WarnAsWarn
                            ]
                        |> Md5Hasher.addBooleans
                            [
                                yield options.ApplyLineDirectives
                                yield options.DiagnosticOptions.GlobalWarnAsError
                                yield options.IsInteractive
                                yield! (Option.toList options.IndentationAwareSyntax)
                                yield! (Option.toList options.StrictIndentation)
                                yield options.CompilingFSharpCore
                                yield options.IsExe
                            ]
                        |> Md5Hasher.toString
                }

            caches.ParseFileWithoutProject.Get(cacheKey, parseFileAsync)

    member _.ParseAndCheckFileInProject(fileName: string, projectSnapshot: ProjectSnapshot, userOpName: string) =
        ignore userOpName
        ComputeParseAndCheckFileInProject fileName projectSnapshot

    member _.FindReferencesInFile(fileName: string, projectSnapshot: ProjectSnapshot, symbol: FSharpSymbol, userOpName: string) =
        ignore userOpName

        async {
            match! ComputeItemKeyStore(fileName, projectSnapshot) with
            | None -> return Seq.empty
            | Some itemKeyStore -> return itemKeyStore.FindAll symbol.Item
        }

    member _.GetAssemblyData(projectSnapshot: ProjectSnapshot, fileName, _userOpName) =
        ComputeAssemblyData projectSnapshot fileName

    member _.Caches = caches

    member _.SetCacheSize(cacheSize: CacheSizes) =
        if cacheSize <> caches.CacheSizes then
            caches <- CompilerCaches(cacheSize)

    member x.SetCacheSizeFactor(sizeFactor: int) =
        let newCacheSize = CacheSizes.Create sizeFactor
        x.SetCacheSize newCacheSize

    interface IBackgroundCompiler with

        member this.CheckFileInProject
            (
                parseResults: FSharpParseFileResults,
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpCheckFileAnswer> =
            async {
                let! snapshot = FSharpProjectSnapshot.FromOptions(options, fileName, fileVersion, sourceText, documentSource)

                ignore parseResults

                let! _, result = this.ParseAndCheckFileInProject(fileName, snapshot.ProjectSnapshot, userOpName)

                return result
            }

        member this.CheckFileInProjectAllowingStaleCachedResults
            (
                parseResults: FSharpParseFileResults,
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpCheckFileAnswer option> =
            async {
                let! snapshot = FSharpProjectSnapshot.FromOptions(options, fileName, fileVersion, sourceText, documentSource)

                ignore parseResults

                let! _, result = this.ParseAndCheckFileInProject(fileName, snapshot.ProjectSnapshot, userOpName)

                return Some result
            }

        member this.ClearCache(projects: FSharpProjectIdentifier seq, userOpName: string) : unit =
            use _ =
                Activity.start "TransparentCompiler.ClearCache" [| Activity.Tags.userOpName, userOpName |]

            this.Caches.Clear(Set projects)

        member this.ClearCache(options: seq<FSharpProjectOptions>, userOpName: string) : unit =
            use _ =
                Activity.start "TransparentCompiler.ClearCache" [| Activity.Tags.userOpName, userOpName |]

            backgroundCompiler.ClearCache(options, userOpName)

            this.Caches.Clear(
                options
                |> Seq.map (fun o -> o.GetProjectIdentifier() |> FSharpProjectIdentifier)
                |> Set
            )

        member _.ClearCaches() : unit =
            backgroundCompiler.ClearCaches()
            caches <- CompilerCaches(cacheSizes) // TODO: check

        member _.DownsizeCaches() : unit = backgroundCompiler.DownsizeCaches()

        member _.BeforeBackgroundFileCheck = beforeFileChecked.Publish

        member _.FileParsed = fileParsed.Publish

        member _.FileChecked = fileChecked.Publish

        member _.ProjectChecked = projectChecked.Publish

        member this.FindReferencesInFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                symbol: FSharpSymbol,
                canInvalidateProject: bool,
                userOpName: string
            ) : Async<seq<range>> =
            async {
                ignore canInvalidateProject

                let! snapshot = FSharpProjectSnapshot.FromOptions(options, documentSource)

                return! this.FindReferencesInFile(fileName, snapshot.ProjectSnapshot, symbol, userOpName)
            }

        member this.FindReferencesInFile(fileName, projectSnapshot, symbol, userOpName) =
            this.FindReferencesInFile(fileName, projectSnapshot.ProjectSnapshot, symbol, userOpName)

        member _.FrameworkImportsCache: FrameworkImportsCache =
            backgroundCompiler.FrameworkImportsCache

        member this.GetAssemblyData(options: FSharpProjectOptions, fileName, userOpName: string) : Async<ProjectAssemblyDataResult> =
            async {
                let! snapshot = FSharpProjectSnapshot.FromOptions(options, documentSource)

                return! this.GetAssemblyData(snapshot.ProjectSnapshot, fileName, userOpName)
            }

        member this.GetAssemblyData
            (
                projectSnapshot: FSharpProjectSnapshot,
                fileName,
                userOpName: string
            ) : Async<ProjectAssemblyDataResult> =
            this.GetAssemblyData(projectSnapshot.ProjectSnapshot, fileName, userOpName)

        member this.GetBackgroundCheckResultsForFileInProject
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpParseFileResults * FSharpCheckFileResults> =
            async {
                let! snapshot = FSharpProjectSnapshot.FromOptions(options, documentSource)

                match! this.ParseAndCheckFileInProject(fileName, snapshot.ProjectSnapshot, userOpName) with
                | parseResult, FSharpCheckFileAnswer.Succeeded checkResult -> return parseResult, checkResult
                | parseResult, FSharpCheckFileAnswer.Aborted -> return parseResult, FSharpCheckFileResults.MakeEmpty(fileName, [||], true)
            }

        member this.GetBackgroundParseResultsForFileInProject
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpParseFileResults> =
            async {
                let! snapshot = FSharpProjectSnapshot.FromOptions(options, documentSource)

                return! this.ParseFile(fileName, snapshot.ProjectSnapshot, userOpName)
            }

        member this.GetCachedCheckFileResult
            (
                builder: IncrementalBuilder,
                fileName: string,
                sourceText: ISourceText,
                options: FSharpProjectOptions
            ) : Async<(FSharpParseFileResults * FSharpCheckFileResults) option> =
            async {
                ignore builder

                let! snapshot = FSharpProjectSnapshot.FromOptions(options, fileName, 1, sourceText, documentSource)

                match! this.ParseAndCheckFileInProject(fileName, snapshot.ProjectSnapshot, "GetCachedCheckFileResult") with
                | parseResult, FSharpCheckFileAnswer.Succeeded checkResult -> return Some(parseResult, checkResult)
                | _, FSharpCheckFileAnswer.Aborted -> return None
            }

        member this.GetProjectOptionsFromScript
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
            async {
                let bc = this :> IBackgroundCompiler

                let! snapshot, diagnostics =
                    bc.GetProjectSnapshotFromScript(
                        fileName,
                        SourceTextNew.ofISourceText sourceText,
                        DocumentSource.FileSystem,
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

                let projectOptions = snapshot.ToOptions()
                return projectOptions, diagnostics
            }

        member this.GetProjectSnapshotFromScript
            (
                fileName: string,
                sourceText: ISourceTextNew,
                documentSource: DocumentSource,
                previewEnabled: bool option,
                loadedTimeStamp: DateTime option,
                otherFlags: string array option,
                useFsiAuxLib: bool option,
                useSdkRefs: bool option,
                sdkDirOverride: string option,
                assumeDotNetFramework: bool option,
                optionsStamp: int64 option,
                userOpName: string
            ) : Async<FSharpProjectSnapshot * FSharpDiagnostic list> =
            use _ =
                Activity.start
                    "BackgroundCompiler.GetProjectOptionsFromScript"
                    [| Activity.Tags.fileName, fileName; Activity.Tags.userOpName, userOpName |]

            async {
                // Use the same default as the background compiler.
                let useFsiAuxLib = defaultArg useFsiAuxLib true
                let useSdkRefs = defaultArg useSdkRefs true
                let previewEnabled = defaultArg previewEnabled false

                // Do we assume .NET Framework references for scripts?
                // No, because the bootstrap info call also doesn't
                let assumeDotNetFramework = defaultArg assumeDotNetFramework false

                let extraFlags =
                    if previewEnabled then
                        [| "--langversion:preview" |]
                    else
                        [||]

                let otherFlags = defaultArg otherFlags extraFlags
                use diagnostics = new DiagnosticsScope(otherFlags |> Array.contains "--flaterrors")

                let useSimpleResolution =
                    otherFlags |> Array.exists (fun x -> x = "--simpleresolution")

                let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading
                let projectFileName = fileName + ".fsproj"

                let currentSourceFile =
                    FSharpFileSnapshot.Create(fileName, sourceText.GetHashCode().ToString(), (fun () -> Task.FromResult sourceText))

                let otherFlags = List.ofArray otherFlags

                // Always perform the load closure as we cannot be sure that the incoming file does not load any new additional files.
                // Consider the scenario where a.fsx loads b.fsx. Based purely on a.fsx, we cannot know if b.fsx loads another file.
                // Therefore we cannot rely on any caching for the script closure in this API.
                let loadClosure =
                    ComputeScriptClosureInner
                        fileName
                        sourceText
                        FSharpCheckerResultsSettings.defaultFSharpBinariesDir
                        useSimpleResolution
                        useFsiAuxLib
                        useSdkRefs
                        sdkDirOverride
                        assumeDotNetFramework
                        otherFlags

                let otherFlags =
                    [
                        yield "--noframework"
                        yield "--warn:3"
                        yield! otherFlags
                        for code, _ in loadClosure.NoWarns do
                            yield "--nowarn:" + code
                    ]

                // Once we do have the script closure, we can populate the cache to re-use can later.
                let loadClosureKey =
                    mkScriptClosureCacheKey
                        fileName
                        sourceText
                        useSimpleResolution
                        useFsiAuxLib
                        useSdkRefs
                        assumeDotNetFramework
                        (FSharpProjectIdentifier(projectFileName, ""))
                        otherFlags
                        optionsStamp

                // Populate the cache.
                let! _ = caches.ScriptClosure.Get(loadClosureKey, async { return loadClosure })

                let sourceFiles =
                    loadClosure.SourceFiles
                    |> List.map (fun (sf, _) ->
                        if sf = fileName then
                            currentSourceFile
                        else
                            FSharpFileSnapshot.CreateFromDocumentSource(sf, documentSource))

                let references =
                    loadClosure.References
                    |> List.map (fun (r, _) ->
                        let lastModified = FileSystem.GetLastWriteTimeShim r

                        {
                            Path = r
                            LastModified = lastModified
                        })

                let snapshot =
                    FSharpProjectSnapshot.Create(
                        fileName + ".fsproj",
                        None,
                        None,
                        sourceFiles,
                        references,
                        otherFlags,
                        List.empty,
                        false,
                        true,
                        loadedTimeStamp,
                        Some(FSharpUnresolvedReferencesSet(loadClosure.UnresolvedReferences)),
                        loadClosure.OriginalLoadReferences,
                        optionsStamp
                    )

                let diags =
                    loadClosure.LoadClosureRootFileDiagnostics
                    |> List.map (fun (exn, isError) ->
                        FSharpDiagnostic.CreateFromException(
                            exn,
                            isError,
                            range.Zero,
                            false,
                            otherFlags |> List.contains "--flaterrors",
                            None
                        ))

                return snapshot, (diags @ diagnostics.Diagnostics)
            }

        member this.GetSemanticClassificationForFile(fileName: string, snapshot: FSharpProjectSnapshot, userOpName: string) =
            async {
                ignore userOpName
                return! ComputeSemanticClassification(fileName, snapshot.ProjectSnapshot)
            }

        member this.GetSemanticClassificationForFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<EditorServices.SemanticClassificationView option> =
            async {
                ignore userOpName

                let! snapshot = FSharpProjectSnapshot.FromOptions(options, documentSource)

                return! ComputeSemanticClassification(fileName, snapshot.ProjectSnapshot)
            }

        member this.InvalidateConfiguration(options: FSharpProjectOptions, userOpName: string) : unit =
            backgroundCompiler.InvalidateConfiguration(options, userOpName)

        member this.InvalidateConfiguration(projectSnapshot: FSharpProjectSnapshot, _userOpName: string) : unit =
            this.Caches.Clear(Set.singleton projectSnapshot.Identifier)

        member this.NotifyFileChanged(fileName: string, options: FSharpProjectOptions, userOpName: string) : Async<unit> =
            backgroundCompiler.NotifyFileChanged(fileName, options, userOpName)

        member this.NotifyProjectCleaned(options: FSharpProjectOptions, userOpName: string) : Async<unit> =
            backgroundCompiler.NotifyProjectCleaned(options, userOpName)

        member this.ParseAndCheckFileInProject
            (
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpParseFileResults * FSharpCheckFileAnswer> =
            async {
                let! snapshot = FSharpProjectSnapshot.FromOptions(options, fileName, fileVersion, sourceText, documentSource)

                return! this.ParseAndCheckFileInProject(fileName, snapshot.ProjectSnapshot, userOpName)
            }

        member this.ParseAndCheckFileInProject(fileName: string, projectSnapshot: FSharpProjectSnapshot, userOpName: string) =
            this.ParseAndCheckFileInProject(fileName, projectSnapshot.ProjectSnapshot, userOpName)

        member this.ParseAndCheckProject(options: FSharpProjectOptions, userOpName: string) : Async<FSharpCheckProjectResults> =
            async {
                ignore userOpName

                let! snapshot = FSharpProjectSnapshot.FromOptions(options, documentSource)

                return! ComputeParseAndCheckProject snapshot.ProjectSnapshot
            }

        member this.ParseAndCheckProject(projectSnapshot: FSharpProjectSnapshot, userOpName: string) : Async<FSharpCheckProjectResults> =
            async {
                ignore userOpName
                return! ComputeParseAndCheckProject projectSnapshot.ProjectSnapshot
            }

        member this.ParseFile(fileName, projectSnapshot, userOpName) =
            this.ParseFile(fileName, projectSnapshot.ProjectSnapshot, userOpName)

        member this.ParseFile
            (
                fileName: string,
                sourceText: ISourceText,
                options: FSharpParsingOptions,
                cache: bool,
                flatErrors: bool,
                userOpName: string
            ) : Async<FSharpParseFileResults> =
            this.ParseFileWithoutProject(fileName, sourceText, options, cache, flatErrors, userOpName)

        member this.TryGetRecentCheckResultsForFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                sourceText: ISourceText option,
                userOpName: string
            ) : (FSharpParseFileResults * FSharpCheckFileResults * SourceTextHash) option =
            backgroundCompiler.TryGetRecentCheckResultsForFile(fileName, options, sourceText, userOpName)

        member this.TryGetRecentCheckResultsForFile
            (
                fileName: string,
                projectSnapshot: FSharpProjectSnapshot,
                userOpName: string
            ) : (FSharpParseFileResults * FSharpCheckFileResults) option =
            TryGetRecentCheckResultsForFile(fileName, projectSnapshot, userOpName)
