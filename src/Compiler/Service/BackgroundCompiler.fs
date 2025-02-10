namespace FSharp.Compiler.CodeAnalysis

open FSharp.Compiler.Text
open FSharp.Compiler.BuildGraph

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Reflection.Emit
open System.Threading
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.ILDynamicAssemblyWriter
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Driver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Tokenization
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.BuildGraph
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

type SourceTextHash = int64
type CacheStamp = int64
type FileName = string
type FilePath = string
type ProjectPath = string
type FileVersion = int

type FSharpProjectSnapshot = FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot

type internal IBackgroundCompiler =

    /// Type-check the result obtained by parsing. Force the evaluation of the antecedent type checking context if needed.
    abstract member CheckFileInProject:
        parseResults: FSharpParseFileResults *
        fileName: string *
        fileVersion: int *
        sourceText: ISourceText *
        options: FSharpProjectOptions *
        userOpName: string ->
            Async<FSharpCheckFileAnswer>

    /// Type-check the result obtained by parsing, but only if the antecedent type checking context is available.
    abstract member CheckFileInProjectAllowingStaleCachedResults:
        parseResults: FSharpParseFileResults *
        fileName: string *
        fileVersion: int *
        sourceText: ISourceText *
        options: FSharpProjectOptions *
        userOpName: string ->
            Async<FSharpCheckFileAnswer option>

    abstract member ClearCache: options: seq<FSharpProjectOptions> * userOpName: string -> unit

    abstract member ClearCache: projects: ProjectSnapshot.FSharpProjectIdentifier seq * userOpName: string -> unit

    abstract member ClearCaches: unit -> unit

    abstract member DownsizeCaches: unit -> unit

    abstract member FindReferencesInFile:
        fileName: string *
        options: FSharpProjectOptions *
        symbol: FSharp.Compiler.Symbols.FSharpSymbol *
        canInvalidateProject: bool *
        userOpName: string ->
            Async<seq<FSharp.Compiler.Text.range>>

    abstract member FindReferencesInFile:
        fileName: string * projectSnapshot: FSharpProjectSnapshot * symbol: FSharp.Compiler.Symbols.FSharpSymbol * userOpName: string ->
            Async<seq<FSharp.Compiler.Text.range>>

    abstract member GetAssemblyData:
        options: FSharpProjectOptions * outputFileName: string * userOpName: string ->
            Async<FSharp.Compiler.CompilerConfig.ProjectAssemblyDataResult>

    abstract member GetAssemblyData:
        projectSnapshot: FSharpProjectSnapshot * outputFileName: string * userOpName: string ->
            Async<FSharp.Compiler.CompilerConfig.ProjectAssemblyDataResult>

    /// Fetch the check information from the background compiler (which checks w.r.t. the FileSystem API)
    abstract member GetBackgroundCheckResultsForFileInProject:
        fileName: string * options: FSharpProjectOptions * userOpName: string -> Async<FSharpParseFileResults * FSharpCheckFileResults>

    /// Fetch the parse information from the background compiler (which checks w.r.t. the FileSystem API)
    abstract member GetBackgroundParseResultsForFileInProject:
        fileName: string * options: FSharpProjectOptions * userOpName: string -> Async<FSharpParseFileResults>

    abstract member GetCachedCheckFileResult:
        builder: IncrementalBuilder * fileName: string * sourceText: ISourceText * options: FSharpProjectOptions ->
            Async<(FSharpParseFileResults * FSharpCheckFileResults) option>

    abstract member GetProjectOptionsFromScript:
        fileName: string *
        sourceText: ISourceText *
        previewEnabled: bool option *
        loadedTimeStamp: System.DateTime option *
        otherFlags: string array option *
        useFsiAuxLib: bool option *
        useSdkRefs: bool option *
        sdkDirOverride: string option *
        assumeDotNetFramework: bool option *
        optionsStamp: int64 option *
        userOpName: string ->
            Async<FSharpProjectOptions * FSharp.Compiler.Diagnostics.FSharpDiagnostic list>

    abstract GetProjectSnapshotFromScript:
        fileName: string *
        sourceText: ISourceTextNew *
        documentSource: DocumentSource *
        previewEnabled: bool option *
        loadedTimeStamp: System.DateTime option *
        otherFlags: string array option *
        useFsiAuxLib: bool option *
        useSdkRefs: bool option *
        sdkDirOverride: string option *
        assumeDotNetFramework: bool option *
        optionsStamp: int64 option *
        userOpName: string ->
            Async<FSharpProjectSnapshot * FSharpDiagnostic list>

    abstract member GetSemanticClassificationForFile:
        fileName: string * options: FSharpProjectOptions * userOpName: string ->
            Async<FSharp.Compiler.EditorServices.SemanticClassificationView option>

    abstract member GetSemanticClassificationForFile:
        fileName: string * snapshot: FSharpProjectSnapshot * userOpName: string ->
            Async<FSharp.Compiler.EditorServices.SemanticClassificationView option>

    abstract member InvalidateConfiguration: options: FSharpProjectOptions * userOpName: string -> unit

    abstract InvalidateConfiguration: projectSnapshot: FSharpProjectSnapshot * userOpName: string -> unit

    abstract member NotifyFileChanged: fileName: string * options: FSharpProjectOptions * userOpName: string -> Async<unit>

    abstract member NotifyProjectCleaned: options: FSharpProjectOptions * userOpName: string -> Async<unit>

    /// Parses and checks the source file and returns untyped AST and check results.
    abstract member ParseAndCheckFileInProject:
        fileName: string * fileVersion: int * sourceText: ISourceText * options: FSharpProjectOptions * userOpName: string ->
            Async<FSharpParseFileResults * FSharpCheckFileAnswer>

    abstract member ParseAndCheckFileInProject:
        fileName: string * projectSnapshot: FSharpProjectSnapshot * userOpName: string ->
            Async<FSharpParseFileResults * FSharpCheckFileAnswer>

    /// Parse and typecheck the whole project.
    abstract member ParseAndCheckProject: options: FSharpProjectOptions * userOpName: string -> Async<FSharpCheckProjectResults>

    abstract member ParseAndCheckProject: projectSnapshot: FSharpProjectSnapshot * userOpName: string -> Async<FSharpCheckProjectResults>

    abstract member ParseFile:
        fileName: string * sourceText: ISourceText * options: FSharpParsingOptions * cache: bool * flatErrors: bool * userOpName: string ->
            Async<FSharpParseFileResults>

    abstract member ParseFile:
        fileName: string * projectSnapshot: FSharpProjectSnapshot * userOpName: string -> Async<FSharpParseFileResults>

    /// Try to get recent approximate type check results for a file.
    abstract member TryGetRecentCheckResultsForFile:
        fileName: string * options: FSharpProjectOptions * sourceText: ISourceText option * userOpName: string ->
            (FSharpParseFileResults * FSharpCheckFileResults * SourceTextHash) option

    abstract member TryGetRecentCheckResultsForFile:
        fileName: string * projectSnapshot: FSharpProjectSnapshot * userOpName: string ->
            (FSharpParseFileResults * FSharpCheckFileResults) option

    abstract member BeforeBackgroundFileCheck: IEvent<string * FSharpProjectOptions>

    abstract member FileChecked: IEvent<string * FSharpProjectOptions>

    abstract member FileParsed: IEvent<string * FSharpProjectOptions>

    abstract member FrameworkImportsCache: FrameworkImportsCache

    abstract member ProjectChecked: IEvent<FSharpProjectOptions>

type internal ParseCacheLockToken() =
    interface LockToken

type CheckFileCacheKey = FileName * SourceTextHash * FSharpProjectOptions
type CheckFileCacheValue = FSharpParseFileResults * FSharpCheckFileResults * SourceTextHash * DateTime

[<AutoOpen>]
module internal EnvMisc =
    let braceMatchCacheSize = GetEnvInteger "FCS_BraceMatchCacheSize" 5
    let parseFileCacheSize = GetEnvInteger "FCS_ParseFileCacheSize" 2
    let checkFileInProjectCacheSize = GetEnvInteger "FCS_CheckFileInProjectCacheSize" 10

    let projectCacheSizeDefault = GetEnvInteger "FCS_ProjectCacheSizeDefault" 3

    let frameworkTcImportsCacheStrongSize =
        GetEnvInteger "FCS_frameworkTcImportsCacheStrongSizeDefault" 8

[<AutoOpen>]
module internal Helpers =

    /// Determine whether two (fileName,options) keys are identical w.r.t. affect on checking
    let AreSameForChecking2 ((fileName1: string, options1: FSharpProjectOptions), (fileName2, options2)) =
        (fileName1 = fileName2)
        && FSharpProjectOptions.AreSameForChecking(options1, options2)

    /// Determine whether two (fileName,options) keys should be identical w.r.t. resource usage
    let AreSubsumable2 ((fileName1: string, o1: FSharpProjectOptions), (fileName2: string, o2: FSharpProjectOptions)) =
        (fileName1 = fileName2) && FSharpProjectOptions.UseSameProject(o1, o2)

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. parsing
    let AreSameForParsing ((fileName1: string, source1Hash: int64, options1), (fileName2, source2Hash, options2)) =
        fileName1 = fileName2 && options1 = options2 && source1Hash = source2Hash

    let AreSimilarForParsing ((fileName1, _, _), (fileName2, _, _)) = fileName1 = fileName2

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. checking
    let AreSameForChecking3 ((fileName1: string, source1Hash: int64, options1: FSharpProjectOptions), (fileName2, source2Hash, options2)) =
        (fileName1 = fileName2)
        && FSharpProjectOptions.AreSameForChecking(options1, options2)
        && source1Hash = source2Hash

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. resource usage
    let AreSubsumable3 ((fileName1: string, _, o1: FSharpProjectOptions), (fileName2: string, _, o2: FSharpProjectOptions)) =
        (fileName1 = fileName2) && FSharpProjectOptions.UseSameProject(o1, o2)

    /// If a symbol is an attribute check if given set of names contains its name without the Attribute suffix
    let rec NamesContainAttribute (symbol: FSharpSymbol) names =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as mofov ->
            mofov.DeclaringEntity
            |> Option.map (fun entity -> NamesContainAttribute entity names)
            |> Option.defaultValue false
        | :? FSharpEntity as entity when entity.IsAttributeType && symbol.DisplayNameCore.EndsWithOrdinal "Attribute" ->
            let nameWithoutAttribute = String.dropSuffix symbol.DisplayNameCore "Attribute"
            names |> Set.contains nameWithoutAttribute
        | _ -> false

// There is only one instance of this type, held in FSharpChecker
type internal BackgroundCompiler
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
        useChangeNotifications
    ) as self =

    let beforeFileChecked = Event<string * FSharpProjectOptions>()
    let fileParsed = Event<string * FSharpProjectOptions>()
    let fileChecked = Event<string * FSharpProjectOptions>()
    let projectChecked = Event<FSharpProjectOptions>()

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.scriptClosureCache
    /// Information about the derived script closure.
    let scriptClosureCache =
        MruCache<AnyCallerThreadToken, FSharpProjectOptions, LoadClosure>(
            projectCacheSize,
            areSame = FSharpProjectOptions.AreSameForChecking,
            areSimilar = FSharpProjectOptions.UseSameProject
        )

    let frameworkTcImportsCache =
        FrameworkImportsCache(frameworkTcImportsCacheStrongSize)

    // We currently share one global dependency provider for all scripts for the FSharpChecker.
    // For projects, one is used per project.
    //
    // Sharing one for all scripts is necessary for good performance from GetProjectOptionsFromScript,
    // which requires a dependency provider to process through the project options prior to working out
    // if the cached incremental builder can be used for the project.
    let dependencyProviderForScripts = new DependencyProvider()

    let getProjectReferences (options: FSharpProjectOptions) userOpName =
        [
            for r in options.ReferencedProjects do

                match r with
                | FSharpReferencedProject.FSharpReference(nm, opts) ->
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
                                    return! self.GetAssemblyData(opts, userOpName + ".CheckReferencedProject(" + nm + ")")
                                }

                            member x.TryGetLogicalTimeStamp(cache) =
                                self.TryGetLogicalTimeStampForProject(cache, opts)

                            member x.FileName = nm
                        }

                | FSharpReferencedProject.PEReference(getStamp, delayedReader) ->
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

                | FSharpReferencedProject.ILModuleReference(nm, getStamp, getReader) ->
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

    /// CreateOneIncrementalBuilder (for background type checking). Note that fsc.fs also
    /// creates an incremental builder used by the command line compiler.
    let CreateOneIncrementalBuilder (options: FSharpProjectOptions, userOpName) =
        async {
            use _ =
                Activity.start "BackgroundCompiler.CreateOneIncrementalBuilder" [| Activity.Tags.project, options.ProjectFileName |]

            Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "CreateOneIncrementalBuilder", options.ProjectFileName)
            let projectReferences = getProjectReferences options userOpName

            let loadClosure = scriptClosureCache.TryGet(AnyCallerThread, options)

            let dependencyProvider =
                if options.UseScriptResolutionRules then
                    Some dependencyProviderForScripts
                else
                    None

            let! builderOpt, diagnostics =
                IncrementalBuilder.TryCreateIncrementalBuilderForProjectOptions(
                    legacyReferenceResolver,
                    FSharpCheckerResultsSettings.defaultFSharpBinariesDir,
                    frameworkTcImportsCache,
                    loadClosure,
                    Array.toList options.SourceFiles,
                    Array.toList options.OtherOptions,
                    projectReferences,
                    options.ProjectDirectory,
                    options.UseScriptResolutionRules,
                    keepAssemblyContents,
                    keepAllBackgroundResolutions,
                    tryGetMetadataSnapshot,
                    suggestNamesForErrors,
                    keepAllBackgroundSymbolUses,
                    enableBackgroundItemKeyStoreAndSemanticClassification,
                    enablePartialTypeChecking,
                    dependencyProvider,
                    parallelReferenceResolution,
                    captureIdentifiersWhenParsing,
                    getSource,
                    useChangeNotifications
                )

            match builderOpt with
            | None -> ()
            | Some builder ->

#if !NO_TYPEPROVIDERS
                // Register the behaviour that responds to CCUs being invalidated because of type
                // provider Invalidate events. This invalidates the configuration in the build.
                builder.ImportsInvalidatedByTypeProvider.Add(fun () -> self.InvalidateConfiguration(options, userOpName))
#endif

                // Register the callback called just before a file is typechecked by the background builder (without recording
                // errors or intellisense information).
                //
                // This indicates to the UI that the file type check state is dirty. If the file is open and visible then
                // the UI will sooner or later request a typecheck of the file, recording errors and intellisense information.
                builder.BeforeFileChecked.Add(fun file -> beforeFileChecked.Trigger(file, options))
                builder.FileParsed.Add(fun file -> fileParsed.Trigger(file, options))
                builder.FileChecked.Add(fun file -> fileChecked.Trigger(file, options))
                builder.ProjectChecked.Add(fun () -> projectChecked.Trigger options)

            return (builderOpt, diagnostics)
        }

    let parseCacheLock = Lock<ParseCacheLockToken>()

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.parseFileInProjectCache. Most recently used cache for parsing files.
    let parseFileCache =
        MruCache<ParseCacheLockToken, _ * SourceTextHash * _, _>(
            parseFileCacheSize,
            areSimilar = AreSimilarForParsing,
            areSame = AreSameForParsing
        )

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.checkFileInProjectCache
    //
    /// Cache which holds recently seen type-checks.
    /// This cache may hold out-of-date entries, in two senses
    ///    - there may be a more recent antecedent state available because the background build has made it available
    ///    - the source for the file may have changed

    // Also keyed on source. This can only be out of date if the antecedent is out of date
    let checkFileInProjectCache =
        MruCache<ParseCacheLockToken, CheckFileCacheKey, GraphNode<CheckFileCacheValue>>(
            keepStrongly = checkFileInProjectCacheSize,
            areSame = AreSameForChecking3,
            areSimilar = AreSubsumable3
        )

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.incrementalBuildersCache. This root typically holds more
    // live information than anything else in the F# Language Service, since it holds up to 3 (projectCacheStrongSize) background project builds
    // strongly.
    //
    /// Cache of builds keyed by options.
    let gate = obj ()

    let incrementalBuildersCache =
        MruCache<AnyCallerThreadToken, FSharpProjectOptions, GraphNode<IncrementalBuilder option * FSharpDiagnostic[]>>(
            keepStrongly = projectCacheSize,
            keepMax = projectCacheSize,
            areSame = FSharpProjectOptions.AreSameForChecking,
            areSimilar = FSharpProjectOptions.UseSameProject
        )

    let tryGetBuilderNode options =
        incrementalBuildersCache.TryGet(AnyCallerThread, options)

    let tryGetBuilder options : Async<IncrementalBuilder option * FSharpDiagnostic[]> option =
        tryGetBuilderNode options |> Option.map (fun x -> x.GetOrComputeValue())

    let tryGetSimilarBuilder options : Async<IncrementalBuilder option * FSharpDiagnostic[]> option =
        incrementalBuildersCache.TryGetSimilar(AnyCallerThread, options)
        |> Option.map (fun x -> x.GetOrComputeValue())

    let tryGetAnyBuilder options : Async<IncrementalBuilder option * FSharpDiagnostic[]> option =
        incrementalBuildersCache.TryGetAny(AnyCallerThread, options)
        |> Option.map (fun x -> x.GetOrComputeValue())

    let createBuilderNode (options, userOpName, ct: CancellationToken) =
        lock gate (fun () ->
            if ct.IsCancellationRequested then
                GraphNode.FromResult(None, [||])
            else
                let getBuilderNode = GraphNode(CreateOneIncrementalBuilder(options, userOpName))
                incrementalBuildersCache.Set(AnyCallerThread, options, getBuilderNode)
                getBuilderNode)

    let createAndGetBuilder (options, userOpName) =
        async {
            let! ct = Async.CancellationToken
            let getBuilderNode = createBuilderNode (options, userOpName, ct)
            return! getBuilderNode.GetOrComputeValue()
        }

    let getOrCreateBuilder (options, userOpName) : Async<IncrementalBuilder option * FSharpDiagnostic[]> =
        async {
            use! _holder = Cancellable.UseToken()

            match tryGetBuilder options with
            | Some getBuilder ->
                match! getBuilder with
                | builderOpt, creationDiags when builderOpt.IsNone || not builderOpt.Value.IsReferencesInvalidated ->
                    return builderOpt, creationDiags
                | _ ->
                    // The builder could be re-created,
                    //    clear the check file caches that are associated with it.
                    //    We must do this in order to not return stale results when references
                    //    in the project get changed/added/removed.
                    parseCacheLock.AcquireLock(fun ltok ->
                        options.SourceFiles
                        |> Array.iter (fun sourceFile ->
                            let key = (sourceFile, 0L, options)
                            checkFileInProjectCache.RemoveAnySimilar(ltok, key)))

                    return! createAndGetBuilder (options, userOpName)
            | _ -> return! createAndGetBuilder (options, userOpName)
        }

    let getSimilarOrCreateBuilder (options, userOpName) =
        match tryGetSimilarBuilder options with
        | Some res -> res
        // The builder does not exist at all. Create it.
        | None -> getOrCreateBuilder (options, userOpName)

    let getOrCreateBuilderWithInvalidationFlag (options, canInvalidateProject, userOpName) =
        if canInvalidateProject then
            getOrCreateBuilder (options, userOpName)
        else
            getSimilarOrCreateBuilder (options, userOpName)

    let getAnyBuilder (options, userOpName) =
        match tryGetAnyBuilder options with
        | Some getBuilder -> getBuilder
        | _ -> getOrCreateBuilder (options, userOpName)

    static let mutable actualParseFileCount = 0

    static let mutable actualCheckFileCount = 0

    /// Should be a fast operation. Ensures that we have only one async lazy object per file and its hash.
    let getCheckFileNode (parseResults, sourceText, fileName, options, _fileVersion, builder, tcPrior, tcInfo, creationDiags) =

        // Here we lock for the creation of the node, not its execution
        parseCacheLock.AcquireLock(fun ltok ->
            let key = (fileName, sourceText.GetHashCode() |> int64, options)

            match checkFileInProjectCache.TryGet(ltok, key) with
            | Some res -> res
            | _ ->
                let res =
                    GraphNode(
                        async {
                            let! res =
                                self.CheckOneFileImplAux(
                                    parseResults,
                                    sourceText,
                                    fileName,
                                    options,
                                    builder,
                                    tcPrior,
                                    tcInfo,
                                    creationDiags
                                )

                            Interlocked.Increment(&actualCheckFileCount) |> ignore
                            return res
                        }
                    )

                checkFileInProjectCache.Set(ltok, key, res)
                res)

    member _.ParseFile
        (
            fileName: string,
            sourceText: ISourceText,
            options: FSharpParsingOptions,
            cache: bool,
            flatErrors: bool,
            userOpName: string
        ) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.ParseFile"
                    [|
                        Activity.Tags.fileName, fileName
                        Activity.Tags.userOpName, userOpName
                        Activity.Tags.cache, cache.ToString()
                    |]

            if cache then
                let hash = sourceText.GetHashCode() |> int64

                match parseCacheLock.AcquireLock(fun ltok -> parseFileCache.TryGet(ltok, (fileName, hash, options))) with
                | Some res -> return res
                | None ->
                    Interlocked.Increment(&actualParseFileCount) |> ignore
                    let! ct = Async.CancellationToken

                    let parseDiagnostics, parseTree, anyErrors =
                        ParseAndCheckFile.parseFile (
                            sourceText,
                            fileName,
                            options,
                            userOpName,
                            suggestNamesForErrors,
                            flatErrors,
                            captureIdentifiersWhenParsing,
                            ct
                        )

                    let res =
                        FSharpParseFileResults(parseDiagnostics, parseTree, anyErrors, options.SourceFiles)

                    parseCacheLock.AcquireLock(fun ltok -> parseFileCache.Set(ltok, (fileName, hash, options), res))
                    return res
            else
                let! ct = Async.CancellationToken

                let parseDiagnostics, parseTree, anyErrors =
                    ParseAndCheckFile.parseFile (
                        sourceText,
                        fileName,
                        options,
                        userOpName,
                        false,
                        flatErrors,
                        captureIdentifiersWhenParsing,
                        ct
                    )

                return FSharpParseFileResults(parseDiagnostics, parseTree, anyErrors, options.SourceFiles)
        }

    /// Fetch the parse information from the background compiler (which checks w.r.t. the FileSystem API)
    member _.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.GetBackgroundParseResultsForFileInProject"
                    [| Activity.Tags.fileName, fileName; Activity.Tags.userOpName, userOpName |]

            let! builderOpt, creationDiags = getOrCreateBuilder (options, userOpName)

            match builderOpt with
            | None ->
                let parseTree = EmptyParsedInput(fileName, (false, false))
                return FSharpParseFileResults(creationDiags, parseTree, true, [||])
            | Some builder ->
                let parseTree, _, _, parseDiagnostics = builder.GetParseResultsForFile fileName

                let parseDiagnostics =
                    DiagnosticHelpers.CreateDiagnostics(
                        builder.TcConfig.diagnosticsOptions,
                        false,
                        fileName,
                        parseDiagnostics,
                        suggestNamesForErrors,
                        builder.TcConfig.flatErrors,
                        None
                    )

                let diagnostics = [| yield! creationDiags; yield! parseDiagnostics |]

                let parseResults =
                    FSharpParseFileResults(
                        diagnostics = diagnostics,
                        input = parseTree,
                        parseHadErrors = false,
                        dependencyFiles = builder.AllDependenciesDeprecated
                    )

                return parseResults
        }

    member _.GetCachedCheckFileResult(builder: IncrementalBuilder, fileName, sourceText: ISourceText, options) =
        async {
            use _ =
                Activity.start "BackgroundCompiler.GetCachedCheckFileResult" [| Activity.Tags.fileName, fileName |]

            let hash = sourceText.GetHashCode() |> int64
            let key = (fileName, hash, options)

            let cachedResultsOpt =
                parseCacheLock.AcquireLock(fun ltok -> checkFileInProjectCache.TryGet(ltok, key))

            match cachedResultsOpt with
            | Some cachedResults ->
                match! cachedResults.GetOrComputeValue() with
                | parseResults, checkResults, _, priorTimeStamp when
                    (match builder.GetCheckResultsBeforeFileInProjectEvenIfStale fileName with
                     | None -> false
                     | Some(tcPrior) ->
                         tcPrior.ProjectTimeStamp = priorTimeStamp
                         && builder.AreCheckResultsBeforeFileInProjectReady(fileName))
                    ->
                    return Some(parseResults, checkResults)
                | _ ->
                    parseCacheLock.AcquireLock(fun ltok -> checkFileInProjectCache.RemoveAnySimilar(ltok, key))
                    return None
            | _ -> return None
        }

    member private _.CheckOneFileImplAux
        (
            parseResults: FSharpParseFileResults,
            sourceText: ISourceText,
            fileName: string,
            options: FSharpProjectOptions,
            builder: IncrementalBuilder,
            tcPrior: PartialCheckResults,
            tcInfo: TcInfo,
            creationDiags: FSharpDiagnostic[]
        ) : Async<CheckFileCacheValue> =

        async {
            // Get additional script #load closure information if applicable.
            // For scripts, this will have been recorded by GetProjectOptionsFromScript.
            let tcConfig = tcPrior.TcConfig
            let loadClosure = scriptClosureCache.TryGet(AnyCallerThread, options)

            let! checkAnswer =
                FSharpCheckFileResults.CheckOneFile(
                    parseResults,
                    sourceText,
                    fileName,
                    options.ProjectFileName,
                    tcConfig,
                    tcPrior.TcGlobals,
                    tcPrior.TcImports,
                    tcInfo.tcState,
                    tcInfo.moduleNamesDict,
                    loadClosure,
                    tcInfo.TcDiagnostics,
                    options.IsIncompleteTypeCheckEnvironment,
                    options,
                    Some builder,
                    Array.ofList tcInfo.tcDependencyFiles,
                    creationDiags,
                    parseResults.Diagnostics,
                    keepAssemblyContents,
                    suggestNamesForErrors
                )
                |> Cancellable.toAsync

            GraphNode.SetPreferredUILang tcConfig.preferredUiLang
            return (parseResults, checkAnswer, sourceText.GetHashCode() |> int64, tcPrior.ProjectTimeStamp)
        }

    member private bc.CheckOneFileImpl
        (
            parseResults: FSharpParseFileResults,
            sourceText: ISourceText,
            fileName: string,
            options: FSharpProjectOptions,
            fileVersion: int,
            builder: IncrementalBuilder,
            tcPrior: PartialCheckResults,
            tcInfo: TcInfo,
            creationDiags: FSharpDiagnostic[]
        ) =

        async {
            match! bc.GetCachedCheckFileResult(builder, fileName, sourceText, options) with
            | Some(_, results) -> return FSharpCheckFileAnswer.Succeeded results
            | _ ->
                let lazyCheckFile =
                    getCheckFileNode (parseResults, sourceText, fileName, options, fileVersion, builder, tcPrior, tcInfo, creationDiags)

                let! _, results, _, _ = lazyCheckFile.GetOrComputeValue()
                return FSharpCheckFileAnswer.Succeeded results
        }

    /// Type-check the result obtained by parsing, but only if the antecedent type checking context is available.
    member bc.CheckFileInProjectAllowingStaleCachedResults
        (
            parseResults: FSharpParseFileResults,
            fileName,
            fileVersion,
            sourceText: ISourceText,
            options,
            userOpName
        ) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.CheckFileInProjectAllowingStaleCachedResults"
                    [|
                        Activity.Tags.project, options.ProjectFileName
                        Activity.Tags.fileName, fileName
                        Activity.Tags.userOpName, userOpName
                    |]

            let! cachedResults =
                async {
                    let! builderOpt, creationDiags = getAnyBuilder (options, userOpName)

                    match builderOpt with
                    | Some builder ->
                        match! bc.GetCachedCheckFileResult(builder, fileName, sourceText, options) with
                        | Some(_, checkResults) -> return Some(builder, creationDiags, Some(FSharpCheckFileAnswer.Succeeded checkResults))
                        | _ -> return Some(builder, creationDiags, None)
                    | _ -> return None // the builder wasn't ready
                }

            match cachedResults with
            | None -> return None
            | Some(_, _, Some x) -> return Some x
            | Some(builder, creationDiags, None) ->
                Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "CheckFileInProjectAllowingStaleCachedResults.CacheMiss", fileName)

                match builder.GetCheckResultsBeforeFileInProjectEvenIfStale fileName with
                | Some tcPrior ->
                    match tcPrior.TryPeekTcInfo() with
                    | Some tcInfo ->
                        let! checkResults =
                            bc.CheckOneFileImpl(
                                parseResults,
                                sourceText,
                                fileName,
                                options,
                                fileVersion,
                                builder,
                                tcPrior,
                                tcInfo,
                                creationDiags
                            )

                        return Some checkResults
                    | None -> return None
                | None -> return None // the incremental builder was not up to date
        }

    /// Type-check the result obtained by parsing. Force the evaluation of the antecedent type checking context if needed.
    member bc.CheckFileInProject
        (
            parseResults: FSharpParseFileResults,
            fileName,
            fileVersion,
            sourceText: ISourceText,
            options,
            userOpName
        ) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.CheckFileInProject"
                    [|
                        Activity.Tags.project, options.ProjectFileName
                        Activity.Tags.fileName, fileName
                        Activity.Tags.userOpName, userOpName
                    |]

            let! builderOpt, creationDiags = getOrCreateBuilder (options, userOpName)

            match builderOpt with
            | None ->
                return FSharpCheckFileAnswer.Succeeded(FSharpCheckFileResults.MakeEmpty(fileName, creationDiags, keepAssemblyContents))
            | Some builder ->
                // Check the cache. We can only use cached results when there is no work to do to bring the background builder up-to-date
                let! cachedResults = bc.GetCachedCheckFileResult(builder, fileName, sourceText, options)

                match cachedResults with
                | Some(_, checkResults) -> return FSharpCheckFileAnswer.Succeeded checkResults
                | _ ->
                    let! tcPrior = builder.GetCheckResultsBeforeFileInProject fileName
                    let! tcInfo = tcPrior.GetOrComputeTcInfo()

                    return!
                        bc.CheckOneFileImpl(
                            parseResults,
                            sourceText,
                            fileName,
                            options,
                            fileVersion,
                            builder,
                            tcPrior,
                            tcInfo,
                            creationDiags
                        )
        }

    /// Parses and checks the source file and returns untyped AST and check results.
    member bc.ParseAndCheckFileInProject
        (
            fileName: string,
            fileVersion,
            sourceText: ISourceText,
            options: FSharpProjectOptions,
            userOpName
        ) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.ParseAndCheckFileInProject"
                    [|
                        Activity.Tags.project, options.ProjectFileName
                        Activity.Tags.fileName, fileName
                        Activity.Tags.userOpName, userOpName
                    |]

            let! builderOpt, creationDiags = getOrCreateBuilder (options, userOpName)

            match builderOpt with
            | None ->
                let parseTree = EmptyParsedInput(fileName, (false, false))
                let parseResults = FSharpParseFileResults(creationDiags, parseTree, true, [||])
                return (parseResults, FSharpCheckFileAnswer.Aborted)

            | Some builder ->
                let! cachedResults = bc.GetCachedCheckFileResult(builder, fileName, sourceText, options)

                match cachedResults with
                | Some(parseResults, checkResults) -> return (parseResults, FSharpCheckFileAnswer.Succeeded checkResults)
                | _ ->
                    let! tcPrior = builder.GetCheckResultsBeforeFileInProject fileName
                    let! tcInfo = tcPrior.GetOrComputeTcInfo()
                    // Do the parsing.
                    let parsingOptions =
                        FSharpParsingOptions.FromTcConfig(
                            builder.TcConfig,
                            Array.ofList builder.SourceFiles,
                            options.UseScriptResolutionRules
                        )

                    GraphNode.SetPreferredUILang tcPrior.TcConfig.preferredUiLang
                    let! ct = Async.CancellationToken

                    let parseDiagnostics, parseTree, anyErrors =
                        ParseAndCheckFile.parseFile (
                            sourceText,
                            fileName,
                            parsingOptions,
                            userOpName,
                            suggestNamesForErrors,
                            builder.TcConfig.flatErrors,
                            captureIdentifiersWhenParsing,
                            ct
                        )

                    let parseResults =
                        FSharpParseFileResults(parseDiagnostics, parseTree, anyErrors, builder.AllDependenciesDeprecated)

                    let! checkResults =
                        bc.CheckOneFileImpl(
                            parseResults,
                            sourceText,
                            fileName,
                            options,
                            fileVersion,
                            builder,
                            tcPrior,
                            tcInfo,
                            creationDiags
                        )

                    return (parseResults, checkResults)
        }

    member _.NotifyFileChanged(fileName, options, userOpName) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.NotifyFileChanged"
                    [|
                        Activity.Tags.project, options.ProjectFileName
                        Activity.Tags.fileName, fileName
                        Activity.Tags.userOpName, userOpName
                    |]

            let! builderOpt, _ = getOrCreateBuilder (options, userOpName)

            match builderOpt with
            | None -> return ()
            | Some builder -> do! builder.NotifyFileChanged(fileName, DateTime.UtcNow)
        }

    /// Fetch the check information from the background compiler (which checks w.r.t. the FileSystem API)
    member _.GetBackgroundCheckResultsForFileInProject(fileName, options, userOpName) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.ParseAndCheckFileInProject"
                    [|
                        Activity.Tags.project, options.ProjectFileName
                        Activity.Tags.fileName, fileName
                        Activity.Tags.userOpName, userOpName
                    |]

            let! builderOpt, creationDiags = getOrCreateBuilder (options, userOpName)

            match builderOpt with
            | None ->
                let parseTree = EmptyParsedInput(fileName, (false, false))
                let parseResults = FSharpParseFileResults(creationDiags, parseTree, true, [||])
                let typedResults = FSharpCheckFileResults.MakeEmpty(fileName, creationDiags, true)
                return (parseResults, typedResults)
            | Some builder ->
                let parseTree, _, _, parseDiagnostics = builder.GetParseResultsForFile fileName
                let! tcProj = builder.GetFullCheckResultsAfterFileInProject fileName

                let! tcInfo, tcInfoExtras = tcProj.GetOrComputeTcInfoWithExtras()

                let tcResolutions = tcInfoExtras.tcResolutions
                let tcSymbolUses = tcInfoExtras.tcSymbolUses
                let tcOpenDeclarations = tcInfoExtras.tcOpenDeclarations
                let latestCcuSigForFile = tcInfo.latestCcuSigForFile
                let tcState = tcInfo.tcState
                let tcEnvAtEnd = tcInfo.tcEnvAtEndOfFile
                let latestImplementationFile = tcInfoExtras.latestImplFile
                let tcDependencyFiles = tcInfo.tcDependencyFiles
                let tcDiagnostics = tcInfo.TcDiagnostics
                let diagnosticsOptions = builder.TcConfig.diagnosticsOptions

                let symbolEnv =
                    SymbolEnv(tcProj.TcGlobals, tcInfo.tcState.Ccu, Some tcInfo.tcState.CcuSig, tcProj.TcImports)
                    |> Some

                let parseDiagnostics =
                    DiagnosticHelpers.CreateDiagnostics(
                        diagnosticsOptions,
                        false,
                        fileName,
                        parseDiagnostics,
                        suggestNamesForErrors,
                        builder.TcConfig.flatErrors,
                        None
                    )

                let parseDiagnostics = [| yield! creationDiags; yield! parseDiagnostics |]

                let tcDiagnostics =
                    DiagnosticHelpers.CreateDiagnostics(
                        diagnosticsOptions,
                        false,
                        fileName,
                        tcDiagnostics,
                        suggestNamesForErrors,
                        builder.TcConfig.flatErrors,
                        symbolEnv
                    )

                let tcDiagnostics = [| yield! creationDiags; yield! tcDiagnostics |]

                let parseResults =
                    FSharpParseFileResults(
                        diagnostics = parseDiagnostics,
                        input = parseTree,
                        parseHadErrors = false,
                        dependencyFiles = builder.AllDependenciesDeprecated
                    )

                let loadClosure = scriptClosureCache.TryGet(AnyCallerThread, options)

                let typedResults =
                    FSharpCheckFileResults.Make(
                        fileName,
                        options.ProjectFileName,
                        tcProj.TcConfig,
                        tcProj.TcGlobals,
                        options.IsIncompleteTypeCheckEnvironment,
                        Some builder,
                        Some options,
                        Array.ofList tcDependencyFiles,
                        creationDiags,
                        parseResults.Diagnostics,
                        tcDiagnostics,
                        keepAssemblyContents,
                        Option.get latestCcuSigForFile,
                        tcState.Ccu,
                        tcProj.TcImports,
                        tcEnvAtEnd.AccessRights,
                        tcResolutions,
                        tcSymbolUses,
                        tcEnvAtEnd.NameEnv,
                        loadClosure,
                        latestImplementationFile,
                        tcOpenDeclarations
                    )

                return (parseResults, typedResults)
        }

    member _.FindReferencesInFile
        (
            fileName: string,
            options: FSharpProjectOptions,
            symbol: FSharpSymbol,
            canInvalidateProject: bool,
            userOpName: string
        ) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.FindReferencesInFile"
                    [|
                        Activity.Tags.project, options.ProjectFileName
                        Activity.Tags.fileName, fileName
                        Activity.Tags.userOpName, userOpName
                        "symbol", symbol.FullName
                    |]

            let! builderOpt, _ = getOrCreateBuilderWithInvalidationFlag (options, canInvalidateProject, userOpName)

            match builderOpt with
            | None -> return Seq.empty
            | Some builder ->
                if builder.ContainsFile fileName then
                    let! checkResults = builder.GetFullCheckResultsAfterFileInProject fileName
                    let! keyStoreOpt = checkResults.GetOrComputeItemKeyStoreIfEnabled()

                    match keyStoreOpt with
                    | None -> return Seq.empty
                    | Some reader -> return reader.FindAll symbol.Item
                else
                    return Seq.empty
        }

    member _.GetSemanticClassificationForFile(fileName: string, options: FSharpProjectOptions, userOpName: string) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.GetSemanticClassificationForFile"
                    [|
                        Activity.Tags.project, options.ProjectFileName
                        Activity.Tags.fileName, fileName
                        Activity.Tags.userOpName, userOpName
                    |]

            let! builderOpt, _ = getOrCreateBuilder (options, userOpName)

            match builderOpt with
            | None -> return None
            | Some builder ->
                let! checkResults = builder.GetFullCheckResultsAfterFileInProject fileName
                let! scopt = checkResults.GetOrComputeSemanticClassificationIfEnabled()

                match scopt with
                | None -> return None
                | Some sc -> return Some(sc.GetView())
        }

    /// Try to get recent approximate type check results for a file.
    member _.TryGetRecentCheckResultsForFile
        (
            fileName: string,
            options: FSharpProjectOptions,
            sourceText: ISourceText option,
            _userOpName: string
        ) =
        use _ =
            Activity.start
                "BackgroundCompiler.GetSemanticClassificationForFile"
                [|
                    Activity.Tags.project, options.ProjectFileName
                    Activity.Tags.fileName, fileName
                    Activity.Tags.userOpName, _userOpName
                |]

        match sourceText with
        | Some sourceText ->
            let hash = sourceText.GetHashCode() |> int64

            let resOpt =
                parseCacheLock.AcquireLock(fun ltok -> checkFileInProjectCache.TryGet(ltok, (fileName, hash, options)))

            match resOpt with
            | Some res ->
                match res.TryPeekValue() with
                | ValueSome(a, b, c, _) -> Some(a, b, c)
                | ValueNone -> None
            | None -> None
        | None -> None

    member _.TryGetRecentCheckResultsForFile(fileName: string, projectSnapshot: FSharpProjectSnapshot, userOpName: string) =
        projectSnapshot.ProjectSnapshot.SourceFiles
        |> List.tryFind (fun f -> f.FileName = fileName)
        |> Option.bind (fun (f: FSharpFileSnapshot) ->
            let options = projectSnapshot.ToOptions()
            let sourceText = f.GetSource() |> Async.AwaitTask |> Async.RunSynchronously

            self.TryGetRecentCheckResultsForFile(fileName, options, Some sourceText, userOpName)
            |> Option.map (fun (parseFileResults, checkFileResults, _hash) -> (parseFileResults, checkFileResults)))

    /// Parse and typecheck the whole project (the implementation, called recursively as project graph is evaluated)
    member private _.ParseAndCheckProjectImpl(options, userOpName) =
        async {

            let! builderOpt, creationDiags = getOrCreateBuilder (options, userOpName)

            match builderOpt with
            | None ->
                let emptyResults =
                    FSharpCheckProjectResults(options.ProjectFileName, None, keepAssemblyContents, creationDiags, None)

                return emptyResults
            | Some builder ->
                let! tcProj, ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt = builder.GetFullCheckResultsAndImplementationsForProject()
                let diagnosticsOptions = tcProj.TcConfig.diagnosticsOptions
                let fileName = DummyFileNameForRangesWithoutASpecificLocation

                // Although we do not use 'tcInfoExtras', computing it will make sure we get an extra info.
                let! tcInfo, _tcInfoExtras = tcProj.GetOrComputeTcInfoWithExtras()

                let topAttribs = tcInfo.topAttribs
                let tcState = tcInfo.tcState
                let tcEnvAtEnd = tcInfo.tcEnvAtEndOfFile
                let tcDiagnostics = tcInfo.TcDiagnostics
                let tcDependencyFiles = tcInfo.tcDependencyFiles

                let symbolEnv =
                    SymbolEnv(tcProj.TcGlobals, tcInfo.tcState.Ccu, Some tcInfo.tcState.CcuSig, tcProj.TcImports)
                    |> Some

                let tcDiagnostics =
                    DiagnosticHelpers.CreateDiagnostics(
                        diagnosticsOptions,
                        true,
                        fileName,
                        tcDiagnostics,
                        suggestNamesForErrors,
                        builder.TcConfig.flatErrors,
                        symbolEnv
                    )

                let diagnostics = [| yield! creationDiags; yield! tcDiagnostics |]

                let getAssemblyData () =
                    match tcAssemblyDataOpt with
                    | ProjectAssemblyDataResult.Available data -> Some data
                    | _ -> None

                let details =
                    (tcProj.TcGlobals,
                     tcProj.TcImports,
                     tcState.Ccu,
                     tcState.CcuSig,
                     Choice1Of2 builder,
                     topAttribs,
                     getAssemblyData,
                     ilAssemRef,
                     tcEnvAtEnd.AccessRights,
                     tcAssemblyExprOpt,
                     Array.ofList tcDependencyFiles,
                     Some options)

                let results =
                    FSharpCheckProjectResults(
                        options.ProjectFileName,
                        Some tcProj.TcConfig,
                        keepAssemblyContents,
                        diagnostics,
                        Some details
                    )

                return results
        }

    member _.GetAssemblyData(options, userOpName) =
        async {
            use _ =
                Activity.start
                    "BackgroundCompiler.GetAssemblyData"
                    [|
                        Activity.Tags.project, options.ProjectFileName
                        Activity.Tags.userOpName, userOpName
                    |]

            let! builderOpt, _ = getOrCreateBuilder (options, userOpName)

            match builderOpt with
            | None -> return ProjectAssemblyDataResult.Unavailable true
            | Some builder ->
                let! _, _, tcAssemblyDataOpt, _ = builder.GetCheckResultsAndImplementationsForProject()
                return tcAssemblyDataOpt
        }

    /// Get the timestamp that would be on the output if fully built immediately
    member private _.TryGetLogicalTimeStampForProject(cache, options) =
        match tryGetBuilderNode options with
        | Some lazyWork ->
            match lazyWork.TryPeekValue() with
            | ValueSome(Some builder, _) -> Some(builder.GetLogicalTimeStampForProject(cache))
            | _ -> None
        | _ -> None

    /// Parse and typecheck the whole project.
    member bc.ParseAndCheckProject(options, userOpName) =
        use _ =
            Activity.start
                "BackgroundCompiler.ParseAndCheckProject"
                [|
                    Activity.Tags.project, options.ProjectFileName
                    Activity.Tags.userOpName, userOpName
                |]

        bc.ParseAndCheckProjectImpl(options, userOpName)

    member _.GetProjectOptionsFromScript
        (
            fileName,
            sourceText,
            previewEnabled,
            loadedTimeStamp,
            otherFlags,
            useFsiAuxLib: bool option,
            useSdkRefs: bool option,
            sdkDirOverride: string option,
            assumeDotNetFramework: bool option,
            optionsStamp: int64 option,
            _userOpName
        ) =
        use _ =
            Activity.start
                "BackgroundCompiler.GetProjectOptionsFromScript"
                [| Activity.Tags.fileName, fileName; Activity.Tags.userOpName, _userOpName |]

        cancellable {
            // Do we add a reference to FSharp.Compiler.Interactive.Settings by default?
            let useFsiAuxLib = defaultArg useFsiAuxLib true
            let useSdkRefs = defaultArg useSdkRefs true
            let reduceMemoryUsage = ReduceMemoryFlag.Yes
            let previewEnabled = defaultArg previewEnabled false

            // Do we assume .NET Framework references for scripts?
            let assumeDotNetFramework = defaultArg assumeDotNetFramework true

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

            let applyCompilerOptions tcConfigB =
                let fsiCompilerOptions = GetCoreFsiCompilerOptions tcConfigB
                ParseCompilerOptions(ignore, fsiCompilerOptions, Array.toList otherFlags)

            let loadClosure =
                LoadClosure.ComputeClosureOfScriptText(
                    legacyReferenceResolver,
                    FSharpCheckerResultsSettings.defaultFSharpBinariesDir,
                    fileName,
                    sourceText,
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

            let otherFlags =
                [|
                    yield "--noframework"
                    yield "--warn:3"
                    yield! otherFlags
                    for r in loadClosure.References do
                        yield "-r:" + fst r
                    for code, _ in loadClosure.NoWarns do
                        yield "--nowarn:" + code
                |]

            let options =
                {
                    ProjectFileName = fileName + ".fsproj" // Make a name that is unique in this directory.
                    ProjectId = None
                    SourceFiles = loadClosure.SourceFiles |> List.map fst |> List.toArray
                    OtherOptions = otherFlags
                    ReferencedProjects = [||]
                    IsIncompleteTypeCheckEnvironment = false
                    UseScriptResolutionRules = true
                    LoadTime = loadedTimeStamp
                    UnresolvedReferences = Some(FSharpUnresolvedReferencesSet(loadClosure.UnresolvedReferences))
                    OriginalLoadReferences = loadClosure.OriginalLoadReferences
                    Stamp = optionsStamp
                }

            scriptClosureCache.Set(AnyCallerThread, options, loadClosure) // Save the full load closure for later correlation.

            let diags =
                loadClosure.LoadClosureRootFileDiagnostics
                |> List.map (fun (exn, isError) ->
                    FSharpDiagnostic.CreateFromException(
                        exn,
                        isError,
                        range.Zero,
                        false,
                        options.OtherOptions |> Array.contains "--flaterrors",
                        None
                    ))

            return options, (diags @ diagnostics.Diagnostics)
        }
        |> Cancellable.toAsync

    member bc.InvalidateConfiguration(options: FSharpProjectOptions, userOpName) =
        use _ =
            Activity.start
                "BackgroundCompiler.InvalidateConfiguration"
                [|
                    Activity.Tags.project, options.ProjectFileName
                    Activity.Tags.userOpName, userOpName
                |]

        if incrementalBuildersCache.ContainsSimilarKey(AnyCallerThread, options) then
            parseCacheLock.AcquireLock(fun ltok ->
                for sourceFile in options.SourceFiles do
                    checkFileInProjectCache.RemoveAnySimilar(ltok, (sourceFile, 0L, options)))

            let _ = createBuilderNode (options, userOpName, CancellationToken.None)
            ()

    member bc.ClearCache(options: seq<FSharpProjectOptions>, _userOpName) =
        use _ =
            Activity.start "BackgroundCompiler.ClearCache" [| Activity.Tags.userOpName, _userOpName |]

        lock gate (fun () ->
            options
            |> Seq.iter (fun options ->
                incrementalBuildersCache.RemoveAnySimilar(AnyCallerThread, options)

                parseCacheLock.AcquireLock(fun ltok ->
                    for sourceFile in options.SourceFiles do
                        checkFileInProjectCache.RemoveAnySimilar(ltok, (sourceFile, 0L, options)))))

    member _.NotifyProjectCleaned(options: FSharpProjectOptions, userOpName) =
        use _ =
            Activity.start
                "BackgroundCompiler.NotifyProjectCleaned"
                [|
                    Activity.Tags.project, options.ProjectFileName
                    Activity.Tags.userOpName, userOpName
                |]

        async {

            let! ct = Async.CancellationToken
            // If there was a similar entry (as there normally will have been) then re-establish an empty builder .  This
            // is a somewhat arbitrary choice - it will have the effect of releasing memory associated with the previous
            // builder, but costs some time.
            if incrementalBuildersCache.ContainsSimilarKey(AnyCallerThread, options) then
                let _ = createBuilderNode (options, userOpName, ct)
                ()
        }

    member _.BeforeBackgroundFileCheck = beforeFileChecked.Publish

    member _.FileParsed = fileParsed.Publish

    member _.FileChecked = fileChecked.Publish

    member _.ProjectChecked = projectChecked.Publish

    member _.ClearCaches() =
        use _ = Activity.startNoTags "BackgroundCompiler.ClearCaches"

        lock gate (fun () ->
            parseCacheLock.AcquireLock(fun ltok ->
                checkFileInProjectCache.Clear(ltok)
                parseFileCache.Clear(ltok))

            incrementalBuildersCache.Clear(AnyCallerThread)
            frameworkTcImportsCache.Clear()
            scriptClosureCache.Clear AnyCallerThread)

    member _.DownsizeCaches() =
        use _ = Activity.startNoTags "BackgroundCompiler.DownsizeCaches"

        lock gate (fun () ->
            parseCacheLock.AcquireLock(fun ltok ->
                checkFileInProjectCache.Resize(ltok, newKeepStrongly = 1)
                parseFileCache.Resize(ltok, newKeepStrongly = 1))

            incrementalBuildersCache.Resize(AnyCallerThread, newKeepStrongly = 1, newKeepMax = 1)
            frameworkTcImportsCache.Downsize()
            scriptClosureCache.Resize(AnyCallerThread, newKeepStrongly = 1, newKeepMax = 1))

    member _.FrameworkImportsCache = frameworkTcImportsCache

    static member ActualParseFileCount = actualParseFileCount

    static member ActualCheckFileCount = actualCheckFileCount

    interface IBackgroundCompiler with

        member _.BeforeBackgroundFileCheck = self.BeforeBackgroundFileCheck

        member _.CheckFileInProject
            (
                parseResults: FSharpParseFileResults,
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpCheckFileAnswer> =
            self.CheckFileInProject(parseResults, fileName, fileVersion, sourceText, options, userOpName)

        member _.CheckFileInProjectAllowingStaleCachedResults
            (
                parseResults: FSharpParseFileResults,
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpCheckFileAnswer option> =
            self.CheckFileInProjectAllowingStaleCachedResults(parseResults, fileName, fileVersion, sourceText, options, userOpName)

        member _.ClearCache(options: seq<FSharpProjectOptions>, userOpName: string) : unit = self.ClearCache(options, userOpName)

        member _.ClearCache(projects: ProjectSnapshot.FSharpProjectIdentifier seq, userOpName: string) = ignore (projects, userOpName)

        member _.ClearCaches() : unit = self.ClearCaches()
        member _.DownsizeCaches() : unit = self.DownsizeCaches()
        member _.FileChecked: IEvent<string * FSharpProjectOptions> = self.FileChecked
        member _.FileParsed: IEvent<string * FSharpProjectOptions> = self.FileParsed

        member _.FindReferencesInFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                symbol: FSharpSymbol,
                canInvalidateProject: bool,
                userOpName: string
            ) : Async<seq<range>> =
            self.FindReferencesInFile(fileName, options, symbol, canInvalidateProject, userOpName)

        member this.FindReferencesInFile(fileName, projectSnapshot, symbol, userOpName) =
            this.FindReferencesInFile(fileName, projectSnapshot.ToOptions(), symbol, true, userOpName)

        member _.FrameworkImportsCache: FrameworkImportsCache = self.FrameworkImportsCache

        member _.GetAssemblyData(options: FSharpProjectOptions, _fileName: string, userOpName: string) : Async<ProjectAssemblyDataResult> =
            self.GetAssemblyData(options, userOpName)

        member _.GetAssemblyData
            (
                projectSnapshot: FSharpProjectSnapshot,
                _fileName: string,
                userOpName: string
            ) : Async<ProjectAssemblyDataResult> =
            self.GetAssemblyData(projectSnapshot.ToOptions(), userOpName)

        member _.GetBackgroundCheckResultsForFileInProject
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpParseFileResults * FSharpCheckFileResults> =
            self.GetBackgroundCheckResultsForFileInProject(fileName, options, userOpName)

        member _.GetBackgroundParseResultsForFileInProject
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpParseFileResults> =
            self.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName)

        member _.GetCachedCheckFileResult
            (
                builder: IncrementalBuilder,
                fileName: string,
                sourceText: ISourceText,
                options: FSharpProjectOptions
            ) : Async<(FSharpParseFileResults * FSharpCheckFileResults) option> =
            self.GetCachedCheckFileResult(builder, fileName, sourceText, options)

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
            self.GetProjectOptionsFromScript(
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

        member _.GetProjectSnapshotFromScript
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
            async {
                let! options, diagnostics =
                    self.GetProjectOptionsFromScript(
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

                let! snapshot = FSharpProjectSnapshot.FromOptions(options, documentSource)
                return snapshot, diagnostics
            }

        member _.GetSemanticClassificationForFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<EditorServices.SemanticClassificationView option> =
            self.GetSemanticClassificationForFile(fileName, options, userOpName)

        member _.GetSemanticClassificationForFile
            (
                fileName: string,
                snapshot: FSharpProjectSnapshot,
                userOpName: string
            ) : Async<EditorServices.SemanticClassificationView option> =
            self.GetSemanticClassificationForFile(fileName, snapshot.ToOptions(), userOpName)

        member _.InvalidateConfiguration(options: FSharpProjectOptions, userOpName: string) : unit =
            self.InvalidateConfiguration(options, userOpName)

        member this.InvalidateConfiguration(projectSnapshot: FSharpProjectSnapshot, userOpName: string) : unit =
            let options = projectSnapshot.ToOptions()
            self.InvalidateConfiguration(options, userOpName)

        member _.NotifyFileChanged(fileName: string, options: FSharpProjectOptions, userOpName: string) : Async<unit> =
            self.NotifyFileChanged(fileName, options, userOpName)

        member _.NotifyProjectCleaned(options: FSharpProjectOptions, userOpName: string) : Async<unit> =
            self.NotifyProjectCleaned(options, userOpName)

        member _.ParseAndCheckFileInProject
            (
                fileName: string,
                fileVersion: int,
                sourceText: ISourceText,
                options: FSharpProjectOptions,
                userOpName: string
            ) : Async<FSharpParseFileResults * FSharpCheckFileAnswer> =
            self.ParseAndCheckFileInProject(fileName, fileVersion, sourceText, options, userOpName)

        member _.ParseAndCheckFileInProject
            (
                fileName: string,
                projectSnapshot: FSharpProjectSnapshot,
                userOpName: string
            ) : Async<FSharpParseFileResults * FSharpCheckFileAnswer> =
            async {
                let fileSnapshot =
                    projectSnapshot.ProjectSnapshot.SourceFiles
                    |> Seq.find (fun f -> f.FileName = fileName)

                let! sourceText = fileSnapshot.GetSource() |> Async.AwaitTask
                let options = projectSnapshot.ToOptions()

                return! self.ParseAndCheckFileInProject(fileName, 0, sourceText, options, userOpName)
            }

        member _.ParseAndCheckProject(options: FSharpProjectOptions, userOpName: string) : Async<FSharpCheckProjectResults> =
            self.ParseAndCheckProject(options, userOpName)

        member _.ParseAndCheckProject(projectSnapshot: FSharpProjectSnapshot, userOpName: string) : Async<FSharpCheckProjectResults> =
            self.ParseAndCheckProject(projectSnapshot.ToOptions(), userOpName)

        member _.ParseFile
            (
                fileName: string,
                sourceText: ISourceText,
                options: FSharpParsingOptions,
                cache: bool,
                flatErrors: bool,
                userOpName: string
            ) =
            self.ParseFile(fileName, sourceText, options, cache, flatErrors, userOpName)

        member _.ParseFile(fileName: string, projectSnapshot: FSharpProjectSnapshot, userOpName: string) =
            let options = projectSnapshot.ToOptions()

            self.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName)

        member _.ProjectChecked: IEvent<FSharpProjectOptions> = self.ProjectChecked

        member _.TryGetRecentCheckResultsForFile
            (
                fileName: string,
                options: FSharpProjectOptions,
                sourceText: ISourceText option,
                userOpName: string
            ) : (FSharpParseFileResults * FSharpCheckFileResults * SourceTextHash) option =
            self.TryGetRecentCheckResultsForFile(fileName, options, sourceText, userOpName)

        member _.TryGetRecentCheckResultsForFile
            (
                fileName: string,
                projectSnapshot: FSharpProjectSnapshot,
                userOpName: string
            ) : (FSharpParseFileResults * FSharpCheckFileResults) option =
            self.TryGetRecentCheckResultsForFile(fileName, projectSnapshot, userOpName)
