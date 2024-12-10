// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

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
open FSharp.Compiler.CodeAnalysis.TransparentCompiler
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

/// Callback that indicates whether a requested result has become obsolete.
[<NoComparison; NoEquality>]
type IsResultObsolete = IsResultObsolete of (unit -> bool)

module CompileHelpers =
    let mkCompilationDiagnosticsHandlers (flatErrors) =
        let diagnostics = ResizeArray<_>()

        let diagnosticsLogger =
            { new DiagnosticsLogger("CompileAPI") with

                member _.DiagnosticSink(diag, isError) =
                    diagnostics.Add(FSharpDiagnostic.CreateFromException(diag, isError, range0, true, flatErrors, None)) // Suggest names for errors

                member _.ErrorCount =
                    diagnostics
                    |> Seq.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)
                    |> Seq.length
            }

        let loggerProvider =
            { new IDiagnosticsLoggerProvider with
                member _.CreateLogger(_tcConfigB, _exiter) = diagnosticsLogger
            }

        diagnostics, diagnosticsLogger, loggerProvider

    let tryCompile diagnosticsLogger f =
        use _ = UseBuildPhase BuildPhase.Parse
        use _ = UseDiagnosticsLogger diagnosticsLogger

        let exiter = StopProcessingExiter()

        try
            f exiter
            None
        with e ->
            stopProcessingRecovery e range0
            Some e

    /// Compile using the given flags.  Source files names are resolved via the FileSystem API. The output file must be given by a -o flag.
    let compileFromArgs (ctok, argv: string[], legacyReferenceResolver, tcImportsCapture, dynamicAssemblyCreator) =

        let diagnostics, diagnosticsLogger, loggerProvider =
            mkCompilationDiagnosticsHandlers (argv |> Array.contains "--flaterrors")

        let result =
            tryCompile diagnosticsLogger (fun exiter ->
                CompileFromCommandLineArguments(
                    ctok,
                    argv,
                    legacyReferenceResolver,
                    true,
                    ReduceMemoryFlag.Yes,
                    CopyFSharpCoreFlag.No,
                    exiter,
                    loggerProvider,
                    tcImportsCapture,
                    dynamicAssemblyCreator
                ))

        diagnostics.ToArray(), result

[<Sealed; AutoSerializable(false)>]
// There is typically only one instance of this type in an IDE process.
type FSharpChecker
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
        getSource,
        useChangeNotifications,
        useTransparentCompiler,
        ?transparentCompilerCacheSizes
    ) =

    let backgroundCompiler =
        if useTransparentCompiler = Some true then
            TransparentCompiler(
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
                ?cacheSizes = transparentCompilerCacheSizes
            )
            :> IBackgroundCompiler
        else
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

    static let globalInstance = lazy FSharpChecker.Create()

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.braceMatchCache. Most recently used cache for brace matching. Accessed on the
    // background UI thread, not on the compiler thread.
    //
    // This cache is safe for concurrent access.
    let braceMatchCache =
        MruCache<AnyCallerThreadToken, _, _>(braceMatchCacheSize, areSimilar = AreSimilarForParsing, areSame = AreSameForParsing)

    static let inferParallelReferenceResolution (parallelReferenceResolution: bool option) =
        let explicitValue =
            parallelReferenceResolution
            |> Option.defaultValue false
            |> function
                | true -> ParallelReferenceResolution.On
                | false -> ParallelReferenceResolution.Off

        let withEnvOverride =
            // Override ParallelReferenceResolution set on the constructor with an environment setting if present.
            getParallelReferenceResolutionFromEnvironment ()
            |> Option.defaultValue explicitValue

        withEnvOverride

    static member getParallelReferenceResolutionFromEnvironment() =
        getParallelReferenceResolutionFromEnvironment ()

    /// Instantiate an interactive checker.
    static member Create
        (
            ?projectCacheSize,
            ?keepAssemblyContents,
            ?keepAllBackgroundResolutions,
            ?legacyReferenceResolver,
            ?tryGetMetadataSnapshot,
            ?suggestNamesForErrors,
            ?keepAllBackgroundSymbolUses,
            ?enableBackgroundItemKeyStoreAndSemanticClassification,
            ?enablePartialTypeChecking,
            ?parallelReferenceResolution: bool,
            ?captureIdentifiersWhenParsing: bool,
            ?documentSource: DocumentSource,
            ?useTransparentCompiler: bool,
            ?transparentCompilerCacheSizes: CacheSizes
        ) =

        use _ = Activity.startNoTags "FSharpChecker.Create"

        let legacyReferenceResolver =
            match legacyReferenceResolver with
            | Some rr -> rr
            | None -> SimulatedMSBuildReferenceResolver.getResolver ()

        let keepAssemblyContents = defaultArg keepAssemblyContents false
        let keepAllBackgroundResolutions = defaultArg keepAllBackgroundResolutions true
        let projectCacheSizeReal = defaultArg projectCacheSize projectCacheSizeDefault
        let tryGetMetadataSnapshot = defaultArg tryGetMetadataSnapshot (fun _ -> None)
        let suggestNamesForErrors = defaultArg suggestNamesForErrors false
        let keepAllBackgroundSymbolUses = defaultArg keepAllBackgroundSymbolUses true

        let enableBackgroundItemKeyStoreAndSemanticClassification =
            defaultArg enableBackgroundItemKeyStoreAndSemanticClassification false

        let enablePartialTypeChecking = defaultArg enablePartialTypeChecking false
        let captureIdentifiersWhenParsing = defaultArg captureIdentifiersWhenParsing false

        let useChangeNotifications =
            match documentSource with
            | Some(DocumentSource.Custom _) -> true
            | _ -> false

        if keepAssemblyContents && enablePartialTypeChecking then
            invalidArg "enablePartialTypeChecking" "'keepAssemblyContents' and 'enablePartialTypeChecking' cannot be both enabled."

        let parallelReferenceResolution = inferParallelReferenceResolution parallelReferenceResolution

        FSharpChecker(
            legacyReferenceResolver,
            projectCacheSizeReal,
            keepAssemblyContents,
            keepAllBackgroundResolutions,
            tryGetMetadataSnapshot,
            suggestNamesForErrors,
            keepAllBackgroundSymbolUses,
            enableBackgroundItemKeyStoreAndSemanticClassification,
            enablePartialTypeChecking,
            parallelReferenceResolution,
            captureIdentifiersWhenParsing,
            (match documentSource with
             | Some(DocumentSource.Custom f) -> Some f
             | _ -> None),
            useChangeNotifications,
            useTransparentCompiler,
            ?transparentCompilerCacheSizes = transparentCompilerCacheSizes
        )

    member _.UsesTransparentCompiler = useTransparentCompiler = Some true

    member _.TransparentCompiler =
        match useTransparentCompiler with
        | Some true -> backgroundCompiler :?> TransparentCompiler
        | _ -> failwith "Transparent Compiler is not enabled."

    member this.Caches = this.TransparentCompiler.Caches

    member _.ReferenceResolver = legacyReferenceResolver

    member _.MatchBraces(fileName, sourceText: ISourceText, options: FSharpParsingOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        use _ =
            Activity.start "FSharpChecker.MatchBraces" [| Activity.Tags.fileName, fileName; Activity.Tags.userOpName, userOpName |]

        let hash = sourceText.GetHashCode() |> int64

        async {
            match braceMatchCache.TryGet(AnyCallerThread, (fileName, hash, options)) with
            | Some res -> return res
            | None ->
                let! ct = Async.CancellationToken

                let res =
                    ParseAndCheckFile.matchBraces (sourceText, fileName, options, userOpName, suggestNamesForErrors, ct)

                braceMatchCache.Set(AnyCallerThread, (fileName, hash, options), res)
                return res
        }

    member ic.MatchBraces(fileName, source: string, options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let parsingOptions, _ = ic.GetParsingOptionsFromProjectOptions(options)
        ic.MatchBraces(fileName, SourceText.ofString source, parsingOptions, userOpName)

    member ic.GetParsingOptionsFromProjectOptions(options) : FSharpParsingOptions * _ =
        let sourceFiles = List.ofArray options.SourceFiles
        let argv = List.ofArray options.OtherOptions
        ic.GetParsingOptionsFromCommandLineArgs(sourceFiles, argv, options.UseScriptResolutionRules)

    member _.ParseFile(fileName, sourceText, options, ?cache, ?userOpName: string) =
        let cache = defaultArg cache true
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.ParseFile(fileName, sourceText, options, cache, false, userOpName)

    member _.ParseFile(fileName, projectSnapshot, ?userOpName) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseFile(fileName, projectSnapshot, userOpName)

    member ic.ParseFileInProject(fileName, source: string, options, ?cache: bool, ?userOpName: string) =
        let parsingOptions, _ = ic.GetParsingOptionsFromProjectOptions(options)
        ic.ParseFile(fileName, SourceText.ofString source, parsingOptions, ?cache = cache, ?userOpName = userOpName)

    member _.GetBackgroundParseResultsForFileInProject(fileName, options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName)

    member _.GetBackgroundCheckResultsForFileInProject(fileName, options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetBackgroundCheckResultsForFileInProject(fileName, options, userOpName)

    /// Try to get recent approximate type check results for a file.
    member _.TryGetRecentCheckResultsForFile(fileName: string, options: FSharpProjectOptions, ?sourceText, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.TryGetRecentCheckResultsForFile(fileName, options, sourceText, userOpName)

    member _.TryGetRecentCheckResultsForFile(fileName: string, projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.TryGetRecentCheckResultsForFile(fileName, projectSnapshot, userOpName)

    member _.Compile(argv: string[], ?userOpName: string) =
        let _userOpName = defaultArg userOpName "Unknown"
        use _ = Activity.start "FSharpChecker.Compile" [| Activity.Tags.userOpName, _userOpName |]

        async {
            let ctok = CompilationThreadToken()
            return CompileHelpers.compileFromArgs (ctok, argv, legacyReferenceResolver, None, None)
        }

    /// This function is called when the entire environment is known to have changed for reasons not encoded in the ProjectOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    member ic.InvalidateAll() = ic.ClearCaches()

    member ic.ClearCaches() =
        let utok = AnyCallerThread
        braceMatchCache.Clear(utok)
        backgroundCompiler.ClearCaches()
        ClearAllILModuleReaderCache()

    member ic.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() =
        use _ =
            Activity.startNoTags "FsharpChecker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients"

        ic.ClearCaches()
        GC.Collect()
        GC.WaitForPendingFinalizers()
        FxResolver.ClearStaticCaches()

    /// This function is called when the configuration is known to have changed for reasons not encoded in the ProjectOptions.
    /// For example, dependent references may have been deleted or created.
    member _.InvalidateConfiguration(options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.InvalidateConfiguration(options, userOpName)

    member _.InvalidateConfiguration(projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.InvalidateConfiguration(projectSnapshot, userOpName)

    /// Clear the internal cache of the given projects.
    member _.ClearCache(options: seq<FSharpProjectOptions>, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.ClearCache(options, userOpName)

    member _.ClearCache(projects: ProjectSnapshot.FSharpProjectIdentifier seq, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.ClearCache(projects, userOpName)

    /// This function is called when a project has been cleaned, and thus type providers should be refreshed.
    member _.NotifyProjectCleaned(options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.NotifyProjectCleaned(options, userOpName)

    member _.NotifyFileChanged(fileName: string, options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.NotifyFileChanged(fileName, options, userOpName)

    /// Typecheck a source code file, returning a handle to the results of the
    /// parse including the reconstructed types in the file.
    member _.CheckFileInProjectAllowingStaleCachedResults
        (
            parseResults: FSharpParseFileResults,
            fileName: string,
            fileVersion: int,
            source: string,
            options: FSharpProjectOptions,
            ?userOpName: string
        ) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.CheckFileInProjectAllowingStaleCachedResults(
            parseResults,
            fileName,
            fileVersion,
            SourceText.ofString source,
            options,
            userOpName
        )

    /// Typecheck a source code file, returning a handle to the results of the
    /// parse including the reconstructed types in the file.
    member _.CheckFileInProject
        (
            parseResults: FSharpParseFileResults,
            fileName: string,
            fileVersion: int,
            sourceText: ISourceText,
            options: FSharpProjectOptions,
            ?userOpName: string
        ) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.CheckFileInProject(parseResults, fileName, fileVersion, sourceText, options, userOpName)

    /// Typecheck a source code file, returning a handle to the results of the
    /// parse including the reconstructed types in the file.
    member _.ParseAndCheckFileInProject
        (
            fileName: string,
            fileVersion: int,
            sourceText: ISourceText,
            options: FSharpProjectOptions,
            ?userOpName: string
        ) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseAndCheckFileInProject(fileName, fileVersion, sourceText, options, userOpName)

    member _.ParseAndCheckFileInProject(fileName: string, projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseAndCheckFileInProject(fileName, projectSnapshot, userOpName)

    member _.ParseAndCheckProject(options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseAndCheckProject(options, userOpName)

    member _.ParseAndCheckProject(projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseAndCheckProject(projectSnapshot, userOpName)

    member _.FindBackgroundReferencesInFile
        (
            fileName: string,
            options: FSharpProjectOptions,
            symbol: FSharpSymbol,
            ?canInvalidateProject: bool,
            ?fastCheck: bool,
            ?userOpName: string
        ) =
        let canInvalidateProject = defaultArg canInvalidateProject true
        let userOpName = defaultArg userOpName "Unknown"

        async {
            if fastCheck <> Some true || not captureIdentifiersWhenParsing then
                return! backgroundCompiler.FindReferencesInFile(fileName, options, symbol, canInvalidateProject, userOpName)
            else
                let! parseResults = backgroundCompiler.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName)

                if
                    parseResults.ParseTree.Identifiers |> Set.contains symbol.DisplayNameCore
                    || parseResults.ParseTree.Identifiers |> NamesContainAttribute symbol
                then
                    return! backgroundCompiler.FindReferencesInFile(fileName, options, symbol, canInvalidateProject, userOpName)
                else
                    return Seq.empty
        }

    member _.FindBackgroundReferencesInFile(fileName: string, projectSnapshot: FSharpProjectSnapshot, symbol: FSharpSymbol, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        async {
            let! parseResults = backgroundCompiler.ParseFile(fileName, projectSnapshot, userOpName)

            if
                parseResults.ParseTree.Identifiers |> Set.contains symbol.DisplayNameCore
                || parseResults.ParseTree.Identifiers |> NamesContainAttribute symbol
            then
                return! backgroundCompiler.FindReferencesInFile(fileName, projectSnapshot, symbol, userOpName)
            else
                return Seq.empty
        }

    member _.GetBackgroundSemanticClassificationForFile(fileName: string, options: FSharpProjectOptions, ?userOpName) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetSemanticClassificationForFile(fileName, options, userOpName)

    member _.GetBackgroundSemanticClassificationForFile(fileName: string, snapshot: FSharpProjectSnapshot, ?userOpName) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetSemanticClassificationForFile(fileName, snapshot, userOpName)

    /// For a given script file, get the ProjectOptions implied by the #load closure
    member _.GetProjectOptionsFromScript
        (
            fileName,
            source,
            ?previewEnabled,
            ?loadedTimeStamp,
            ?otherFlags,
            ?useFsiAuxLib,
            ?useSdkRefs,
            ?assumeDotNetFramework,
            ?sdkDirOverride,
            ?optionsStamp: int64,
            ?userOpName: string
        ) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetProjectOptionsFromScript(
            fileName,
            source,
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

    /// For a given script file, get the ProjectSnapshot implied by the #load closure
    member _.GetProjectSnapshotFromScript
        (
            fileName,
            source,
            ?documentSource,
            ?previewEnabled,
            ?loadedTimeStamp,
            ?otherFlags,
            ?useFsiAuxLib,
            ?useSdkRefs,
            ?assumeDotNetFramework,
            ?sdkDirOverride,
            ?optionsStamp: int64,
            ?userOpName: string
        ) =
        let userOpName = defaultArg userOpName "Unknown"
        let documentSource = defaultArg documentSource DocumentSource.FileSystem

        backgroundCompiler.GetProjectSnapshotFromScript(
            fileName,
            source,
            documentSource,
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

    member _.GetProjectOptionsFromCommandLineArgs(projectFileName, argv, ?loadedTimeStamp, ?isInteractive, ?isEditing) =
        let isEditing = defaultArg isEditing false
        let isInteractive = defaultArg isInteractive false
        let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading

        let argv =
            let define =
                if isInteractive then
                    "--define:INTERACTIVE"
                else
                    "--define:COMPILED"

            Array.append argv [| define |]

        let argv =
            if isEditing then
                Array.append argv [| "--define:EDITING" |]
            else
                argv

        {
            ProjectFileName = projectFileName
            ProjectId = None
            SourceFiles = [||] // the project file names will be inferred from the ProjectOptions
            OtherOptions = argv
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = loadedTimeStamp
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = None
        }

    member _.GetParsingOptionsFromCommandLineArgs(sourceFiles, argv, ?isInteractive, ?isEditing) =
        let isEditing = defaultArg isEditing false
        let isInteractive = defaultArg isInteractive false
        use errorScope = new DiagnosticsScope(argv |> List.contains "--flaterrors")

        let tcConfigB =
            TcConfigBuilder.CreateNew(
                legacyReferenceResolver,
                defaultFSharpBinariesDir = FSharpCheckerResultsSettings.defaultFSharpBinariesDir,
                reduceMemoryUsage = ReduceMemoryFlag.Yes,
                implicitIncludeDir = "",
                isInteractive = isInteractive,
                isInvalidationSupported = false,
                defaultCopyFSharpCore = CopyFSharpCoreFlag.No,
                tryGetMetadataSnapshot = tryGetMetadataSnapshot,
                sdkDirOverride = None,
                rangeForErrors = range0
            )

        // These defines are implied by the F# compiler
        tcConfigB.conditionalDefines <-
            let define = if isInteractive then "INTERACTIVE" else "COMPILED"
            define :: tcConfigB.conditionalDefines

        if isEditing then
            tcConfigB.conditionalDefines <- "EDITING" :: tcConfigB.conditionalDefines

        tcConfigB.realsig <- List.contains "--realsig" argv || List.contains "--realsig+" argv

        // Apply command-line arguments and collect more source files if they are in the arguments
        let sourceFilesNew = ApplyCommandLineArgs(tcConfigB, sourceFiles, argv)
        FSharpParsingOptions.FromTcConfigBuilder(tcConfigB, Array.ofList sourceFilesNew, isInteractive), errorScope.Diagnostics

    member ic.GetParsingOptionsFromCommandLineArgs(argv, ?isInteractive: bool, ?isEditing) =
        ic.GetParsingOptionsFromCommandLineArgs([], argv, ?isInteractive = isInteractive, ?isEditing = isEditing)

    member _.BeforeBackgroundFileCheck = backgroundCompiler.BeforeBackgroundFileCheck

    member _.FileParsed = backgroundCompiler.FileParsed

    member _.FileChecked = backgroundCompiler.FileChecked

    member _.ProjectChecked = backgroundCompiler.ProjectChecked

    static member ActualParseFileCount = BackgroundCompiler.ActualParseFileCount

    static member ActualCheckFileCount = BackgroundCompiler.ActualCheckFileCount

    static member Instance = globalInstance.Force()

    member internal _.FrameworkImportsCache = backgroundCompiler.FrameworkImportsCache

    /// Tokenize a single line, returning token information and a tokenization state represented by an integer
    member _.TokenizeLine(line: string, state: FSharpTokenizerLexState) =
        let tokenizer = FSharpSourceTokenizer([], None, None, None)
        let lineTokenizer = tokenizer.CreateLineTokenizer line
        let mutable state = (None, state)

        let tokens =
            [|
                while (state <- lineTokenizer.ScanToken(snd state)
                       (fst state).IsSome) do
                    yield (fst state).Value
            |]

        tokens, snd state

    /// Tokenize an entire file, line by line
    member x.TokenizeFile(source: string) : FSharpTokenInfo[][] =
        let lines = source.Split('\n')

        let tokens =
            [|
                let mutable state = FSharpTokenizerLexState.Initial

                for line in lines do
                    let tokens, n = x.TokenizeLine(line, state)
                    state <- n
                    yield tokens
            |]

        tokens

namespace FSharp.Compiler

open System
open System.IO
open Internal.Utilities
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text.Range
open FSharp.Compiler.DiagnosticsLogger

type CompilerEnvironment() =
    /// Source file extensions
    static let compilableExtensions = FSharpSigFileSuffixes @ FSharpImplFileSuffixes @ FSharpScriptFileSuffixes

    /// Single file projects extensions
    static let singleFileProjectExtensions = FSharpScriptFileSuffixes

    static member BinFolderOfDefaultFSharpCompiler(?probePoint) =
        FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(probePoint)

    // Legacy entry point, no longer used by FSharp.Editor
    static member DefaultReferencesForOrphanSources assumeDotNetFramework =
        let currentDirectory = Directory.GetCurrentDirectory()

        let fxResolver =
            FxResolver(
                assumeDotNetFramework,
                currentDirectory,
                rangeForErrors = range0,
                useSdkRefs = true,
                isInteractive = false,
                sdkDirOverride = None
            )

        let references, _ = fxResolver.GetDefaultReferences(useFsiAuxLib = false)
        references

    /// Publish compiler-flags parsing logic. Must be fast because its used by the colorizer.
    static member GetConditionalDefinesForEditing(parsingOptions: FSharpParsingOptions) =
        SourceFileImpl.GetImplicitConditionalDefinesForEditing(parsingOptions.IsInteractive)
        @ parsingOptions.ConditionalDefines

    /// Return true if this is a subcategory of error or warning message that the language service can emit
    static member IsCheckerSupportedSubcategory(subcategory: string) =
        // Beware: This code logic is duplicated in DocumentTask.cs in the language service
        PhasedDiagnostic.IsSubcategoryOfCompile(subcategory)

    /// Return the language ID, which is the expression evaluator id that the
    /// debugger will use.
    static member GetDebuggerLanguageID() =
        Guid(0xAB4F38C9u, 0xB6E6us, 0x43baus, 0xBEuy, 0x3Buy, 0x58uy, 0x08uy, 0x0Buy, 0x2Cuy, 0xCCuy, 0xE3uy)

    static member IsScriptFile(fileName: string) = ParseAndCheckInputs.IsScript fileName

    /// Whether or not this file is compilable
    static member IsCompilable(file: string) =
        let ext = Path.GetExtension file

        compilableExtensions
        |> List.exists (fun e -> 0 = String.Compare(e, ext, StringComparison.OrdinalIgnoreCase))

    /// Whether or not this file should be a single-file project
    static member MustBeSingleFileProject(file: string) =
        let ext = Path.GetExtension file

        singleFileProjectExtensions
        |> List.exists (fun e -> 0 = String.Compare(e, ext, StringComparison.OrdinalIgnoreCase))
