namespace FSharp.Compiler.CodeAnalysis

open FSharp.Compiler.Text
open FSharp.Compiler.BuildGraph

open System.Reflection
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics

type SourceTextHash = int64

type CacheStamp = int64

type FileName = string

type FilePath = string

type ProjectPath = string

type FileVersion = int

type FSharpProjectSnapshot = ProjectSnapshot.FSharpProjectSnapshot

type internal IBackgroundCompiler =

    /// Type-check the result obtained by parsing. Force the evaluation of the antecedent type checking context if needed.
    abstract CheckFileInProject:
        parseResults: FSharpParseFileResults *
        fileName: string *
        fileVersion: int *
        sourceText: ISourceText *
        options: FSharpProjectOptions *
        userOpName: string ->
            Async<FSharpCheckFileAnswer>

    /// Type-check the result obtained by parsing, but only if the antecedent type checking context is available.
    abstract CheckFileInProjectAllowingStaleCachedResults:
        parseResults: FSharpParseFileResults *
        fileName: string *
        fileVersion: int *
        sourceText: ISourceText *
        options: FSharpProjectOptions *
        userOpName: string ->
            Async<FSharpCheckFileAnswer option>

    abstract ClearCache: options: FSharpProjectOptions seq * userOpName: string -> unit

    abstract ClearCache: projects: ProjectSnapshot.FSharpProjectIdentifier seq * userOpName: string -> unit

    abstract ClearCaches: unit -> unit

    abstract DownsizeCaches: unit -> unit

    abstract FindReferencesInFile:
        fileName: string *
        projectSnapshot: FSharpProjectSnapshot *
        symbol: FSharp.Compiler.Symbols.FSharpSymbol *
        userOpName: string ->
            Async<range seq>

    abstract FindReferencesInFile:
        fileName: string *
        options: FSharpProjectOptions *
        symbol: FSharp.Compiler.Symbols.FSharpSymbol *
        canInvalidateProject: bool *
        userOpName: string ->
            Async<range seq>

    abstract GetAssemblyData:
        projectSnapshot: FSharpProjectSnapshot * outputFileName: string * userOpName: string ->
            Async<ProjectAssemblyDataResult>

    abstract GetAssemblyData:
        options: FSharpProjectOptions * outputFileName: string * userOpName: string -> Async<ProjectAssemblyDataResult>

    /// Fetch the check information from the background compiler (which checks w.r.t. the FileSystem API)
    abstract GetBackgroundCheckResultsForFileInProject:
        fileName: string * options: FSharpProjectOptions * userOpName: string ->
            Async<FSharpParseFileResults * FSharpCheckFileResults>

    /// Fetch the parse information from the background compiler (which checks w.r.t. the FileSystem API)
    abstract GetBackgroundParseResultsForFileInProject:
        fileName: string * options: FSharpProjectOptions * userOpName: string -> Async<FSharpParseFileResults>

    abstract GetCachedCheckFileResult:
        builder: IncrementalBuilder * fileName: string * sourceText: ISourceText * options: FSharpProjectOptions ->
            Async<(FSharpParseFileResults * FSharpCheckFileResults) option>

    abstract GetProjectOptionsFromScript:
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
            Async<FSharpProjectOptions * FSharpDiagnostic list>

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

    abstract GetSemanticClassificationForFile:
        fileName: string * snapshot: FSharpProjectSnapshot * userOpName: string ->
            Async<FSharp.Compiler.EditorServices.SemanticClassificationView option>

    abstract GetSemanticClassificationForFile:
        fileName: string * options: FSharpProjectOptions * userOpName: string ->
            Async<FSharp.Compiler.EditorServices.SemanticClassificationView option>

    abstract InvalidateConfiguration: options: FSharpProjectOptions * userOpName: string -> unit

    abstract InvalidateConfiguration: projectSnapshot: FSharpProjectSnapshot * userOpName: string -> unit

    abstract NotifyFileChanged: fileName: string * options: FSharpProjectOptions * userOpName: string -> Async<unit>

    abstract NotifyProjectCleaned: options: FSharpProjectOptions * userOpName: string -> Async<unit>

    abstract ParseAndCheckFileInProject:
        fileName: string * projectSnapshot: FSharpProjectSnapshot * userOpName: string ->
            Async<FSharpParseFileResults * FSharpCheckFileAnswer>

    /// Parses and checks the source file and returns untyped AST and check results.
    abstract ParseAndCheckFileInProject:
        fileName: string *
        fileVersion: int *
        sourceText: ISourceText *
        options: FSharpProjectOptions *
        userOpName: string ->
            Async<FSharpParseFileResults * FSharpCheckFileAnswer>

    abstract ParseAndCheckProject:
        projectSnapshot: FSharpProjectSnapshot * userOpName: string -> Async<FSharpCheckProjectResults>

    /// Parse and typecheck the whole project.
    abstract ParseAndCheckProject:
        options: FSharpProjectOptions * userOpName: string -> Async<FSharpCheckProjectResults>

    abstract ParseFile:
        fileName: string * projectSnapshot: FSharpProjectSnapshot * userOpName: string -> Async<FSharpParseFileResults>

    abstract ParseFile:
        fileName: string *
        sourceText: ISourceText *
        options: FSharpParsingOptions *
        cache: bool *
        flatErrors: bool *
        userOpName: string ->
            Async<FSharpParseFileResults>

    /// Try to get recent approximate type check results for a file.
    abstract TryGetRecentCheckResultsForFile:
        fileName: string * options: FSharpProjectOptions * sourceText: ISourceText option * userOpName: string ->
            (FSharpParseFileResults * FSharpCheckFileResults * SourceTextHash) option

    abstract TryGetRecentCheckResultsForFile:
        fileName: string * projectSnapshot: FSharpProjectSnapshot * userOpName: string ->
            (FSharpParseFileResults * FSharpCheckFileResults) option

    abstract BeforeBackgroundFileCheck: IEvent<string * FSharpProjectOptions>

    abstract FileChecked: IEvent<string * FSharpProjectOptions>

    abstract FileParsed: IEvent<string * FSharpProjectOptions>

    abstract FrameworkImportsCache: FrameworkImportsCache

    abstract ProjectChecked: IEvent<FSharpProjectOptions>

[<AutoOpen>]
module internal EnvMisc =

    val braceMatchCacheSize: int

    val parseFileCacheSize: int

    val checkFileInProjectCacheSize: int

    val projectCacheSizeDefault: int

    val frameworkTcImportsCacheStrongSize: int

[<AutoOpen>]
module internal Helpers =

    /// Determine whether two (fileName,options) keys are identical w.r.t. affect on checking
    val AreSameForChecking2: (string * FSharpProjectOptions) * (string * FSharpProjectOptions) -> bool

    /// Determine whether two (fileName,options) keys should be identical w.r.t. resource usage
    val AreSubsumable2: (string * FSharpProjectOptions) * (string * FSharpProjectOptions) -> bool

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. parsing
    val AreSameForParsing: (string * int64 * 'a) * (string * int64 * 'a) -> bool when 'a: equality

    val AreSimilarForParsing: ('a * 'b * 'c) * ('a * 'd * 'e) -> bool when 'a: equality

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. checking
    val AreSameForChecking3: (string * int64 * FSharpProjectOptions) * (string * int64 * FSharpProjectOptions) -> bool

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. resource usage
    val AreSubsumable3: (string * 'a * FSharpProjectOptions) * (string * 'b * FSharpProjectOptions) -> bool

    /// If a symbol is an attribute check if given set of names contains its name without the Attribute suffix
    val NamesContainAttribute: symbol: FSharp.Compiler.Symbols.FSharpSymbol -> names: Set<string> -> bool

type internal BackgroundCompiler =
    interface IBackgroundCompiler

    new:
        legacyReferenceResolver: LegacyReferenceResolver *
        projectCacheSize: int *
        keepAssemblyContents: bool *
        keepAllBackgroundResolutions: bool *
        tryGetMetadataSnapshot: FSharp.Compiler.AbstractIL.ILBinaryReader.ILReaderTryGetMetadataSnapshot *
        suggestNamesForErrors: bool *
        keepAllBackgroundSymbolUses: bool *
        enableBackgroundItemKeyStoreAndSemanticClassification: bool *
        enablePartialTypeChecking: bool *
        parallelReferenceResolution: ParallelReferenceResolution *
        captureIdentifiersWhenParsing: bool *
        getSource: (string -> Async<ISourceText option>) option *
        useChangeNotifications: bool ->
            BackgroundCompiler

    static member ActualCheckFileCount: int

    static member ActualParseFileCount: int
