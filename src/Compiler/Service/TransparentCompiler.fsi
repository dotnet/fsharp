namespace FSharp.Compiler.CodeAnalysis.TransparentCompiler

open Internal.Utilities.Collections

open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.BuildGraph
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.Symbols
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.GraphChecking
open FSharp.Compiler.Syntax
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypedTree
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.EditorServices

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

[<NoEquality; NoComparison>]
type internal TcIntermediate =
    {
        finisher: Finisher<NodeToTypeCheck, TcState, PartialResult>

        /// Disambiguation table for module names
        moduleNamesDict: ModuleNamesDict

        /// Accumulated diagnostics, last file first
        tcDiagnosticsRev: (PhasedDiagnostic * FSharpDiagnosticSeverity) array list
        tcDependencyFiles: string list
        sink: TcResultsSinkImpl
    }

/// Things we need to start parsing and checking files for a given project snapshot
type internal BootstrapInfo =
    { Id: int
      AssemblyName: string
      OutFile: string
      TcConfig: TcConfig
      TcImports: TcImports
      TcGlobals: TcGlobals
      InitialTcInfo: TcInfo
      LoadedSources: (range * ProjectSnapshot.FSharpFileSnapshot) list
      LoadClosure: LoadClosure option
      LastFileName: string
      ImportsInvalidatedByTypeProvider: Event<unit> }

type internal TcIntermediateResult = TcInfo * TcResultsSinkImpl * CheckedImplFile option * string

[<RequireQualifiedAccess>]
type internal DependencyGraphType =

    /// A dependency graph for a single file - it will be missing files which this file does not depend on
    | File

    /// A dependency graph for a project - it will contain all files in the project
    | Project

[<System.Runtime.CompilerServices.Extension; Class>]
type internal Extensions =

    [<System.Runtime.CompilerServices.Extension>]
    static member Key:
        fileSnapshots: #ProjectSnapshot.IFileSnapshot list * ?extraKeyFlag: DependencyGraphType ->
            ICacheKey<(DependencyGraphType option * byte array), string>

type internal CompilerCaches =

    new: sizeFactor: int -> CompilerCaches

    member AssemblyData: AsyncMemoize<(string * string), (string * string), ProjectAssemblyDataResult>

    member BootstrapInfo: AsyncMemoize<(string * string), string, (BootstrapInfo option * FSharpDiagnostic array)>

    member BootstrapInfoStatic:
        AsyncMemoize<(string * string), (string * string), (int * TcImports * TcGlobals * TcInfo * Event<unit>)>

    member DependencyGraph:
        AsyncMemoize<(DependencyGraphType option * byte array), string, (Graph<NodeToTypeCheck> * Graph<FileIndex>)>

    member FrameworkImports: AsyncMemoize<string, FrameworkImportsCacheKey, (TcGlobals * TcImports)>

    member ItemKeyStore: AsyncMemoize<(string * (string * string)), string, ItemKeyStore option>

    member ParseAndCheckAllFilesInProject: AsyncMemoizeDisabled<obj, obj, obj>

    member ParseAndCheckFileInProject:
        AsyncMemoize<(string * (string * string)), string * string, (FSharpParseFileResults * FSharpCheckFileAnswer)>

    member ParseAndCheckProject: AsyncMemoize<(string * string), string, FSharpCheckProjectResults>

    member ParseFile:
        AsyncMemoize<((string * string) * string), (string * string * bool), ProjectSnapshot.FSharpParsedFile>

    member ParseFileWithoutProject: AsyncMemoize<string, string, FSharpParseFileResults>

    member ProjectExtras: AsyncMemoizeDisabled<obj, obj, obj>

    member SemanticClassification: AsyncMemoize<(string * (string * string)), string, SemanticClassificationView option>

    member SizeFactor: int

    member TcIntermediate: AsyncMemoize<(string * (string * string)), (string * int), TcIntermediate>

    member ScriptClosure: AsyncMemoize<(string * (string * string)), string, LoadClosure>

    member TcLastFile: AsyncMemoizeDisabled<obj, obj, obj>

type internal TransparentCompiler =
    interface IBackgroundCompiler

    new:
        legacyReferenceResolver: LegacyReferenceResolver *
        projectCacheSize: int *
        keepAssemblyContents: bool *
        keepAllBackgroundResolutions: bool *
        tryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot *
        suggestNamesForErrors: bool *
        keepAllBackgroundSymbolUses: bool *
        enableBackgroundItemKeyStoreAndSemanticClassification: bool *
        enablePartialTypeChecking: bool *
        parallelReferenceResolution: ParallelReferenceResolution *
        captureIdentifiersWhenParsing: bool *
        getSource: (string -> Async<ISourceText option>) option *
        useChangeNotifications: bool *
        useSyntaxTreeCache: bool ->
            TransparentCompiler

    member FindReferencesInFile:
        fileName: string * projectSnapshot: ProjectSnapshot.ProjectSnapshot * symbol: FSharpSymbol * userOpName: string ->
            NodeCode<range seq>

    member GetAssemblyData:
        projectSnapshot: ProjectSnapshot.ProjectSnapshot * fileName: string * _userOpName: string ->
            NodeCode<ProjectAssemblyDataResult>

    member ParseAndCheckFileInProject:
        fileName: string * projectSnapshot: ProjectSnapshot.ProjectSnapshot * userOpName: string ->
            NodeCode<FSharpParseFileResults * FSharpCheckFileAnswer>

    member ParseFile:
        fileName: string * projectSnapshot: ProjectSnapshot.ProjectSnapshot * _userOpName: 'a ->
            NodeCode<FSharpParseFileResults>

    member SetCacheSizeFactor: sizeFactor: int -> unit

    member Caches: CompilerCaches
