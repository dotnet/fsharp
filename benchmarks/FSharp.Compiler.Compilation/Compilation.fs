namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution
open Internal.Utilities
open FSharp.Compiler.Compilation.Utilities

type CompilationOptions =
    {
        LegacyReferenceResolver: ReferenceResolver.Resolver
        DefaultFSharpBinariesDir: string
        TryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
        AssemblyPath: string
        IsExecutable: bool
        KeepAssemblyContents: bool
        KeepAllBackgroundResolutions: bool
        SourceSnapshots: ImmutableArray<SourceSnapshot>
        CompilationReferences: ImmutableArray<Compilation>
    }

    static member Create (assemblyPath, projectDirectory, sourceSnapshots, compilationReferences) =
        let legacyReferenceResolver = SimulatedMSBuildReferenceResolver.GetBestAvailableResolver()

        // TcState is arbitrary, doesn't matter as long as the type is inside FSharp.Compiler.Private, also this is yuck.
        let defaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(Some(typeof<TcState>.Assembly.Location)).Value

        let tryGetMetadataSnapshot = (fun _ -> None)
        let suggestNamesForErrors = false
        let useScriptResolutionRules = false
        {
            LegacyReferenceResolver = legacyReferenceResolver
            DefaultFSharpBinariesDir = defaultFSharpBinariesDir
            TryGetMetadataSnapshot = tryGetMetadataSnapshot
            SuggestNamesForErrors = suggestNamesForErrors
            CommandLineArgs = []
            ProjectDirectory = projectDirectory
            UseScriptResolutionRules = useScriptResolutionRules
            AssemblyPath = assemblyPath
            IsExecutable = false
            KeepAssemblyContents = false
            KeepAllBackgroundResolutions = false
            SourceSnapshots = sourceSnapshots
            CompilationReferences = compilationReferences
        }

    member options.CreateTcInitial frameworkTcImportsCache ctok =
        let tcInitialOptions =
            {
                frameworkTcImportsCache = frameworkTcImportsCache
                legacyReferenceResolver = options.LegacyReferenceResolver
                defaultFSharpBinariesDir = options.DefaultFSharpBinariesDir
                tryGetMetadataSnapshot = options.TryGetMetadataSnapshot
                suggestNamesForErrors = options.SuggestNamesForErrors
                sourceFiles = options.SourceSnapshots |> Seq.map (fun x -> x.FilePath) |> List.ofSeq
                commandLineArgs = options.CommandLineArgs
                projectDirectory = options.ProjectDirectory
                projectReferences = [] // TODO:
                useScriptResolutionRules = options.UseScriptResolutionRules
                assemblyPath = options.AssemblyPath
                isExecutable = options.IsExecutable
                keepAssemblyContents = options.KeepAssemblyContents
                keepAllBackgroundResolutions = options.KeepAllBackgroundResolutions
            }
        TcInitial.create tcInitialOptions ctok

    member options.CreateIncrementalChecker frameworkTcImportsCache ctok =
        let tcInitial = options.CreateTcInitial frameworkTcImportsCache ctok
        let tcImports, tcAcc = TcAccumulator.createInitial tcInitial ctok |> Cancellable.runWithoutCancellation
        let checkerOptions =
            {
                keepAssemblyContents = options.KeepAssemblyContents
                keepAllBackgroundResolutions = options.KeepAllBackgroundResolutions
                parsingOptions = { isExecutable = options.IsExecutable }
            }
        IncrementalChecker.create tcInitial.tcConfig tcInitial.tcGlobals tcImports tcAcc checkerOptions options.SourceSnapshots
        |> Cancellable.runWithoutCancellation

and [<Sealed>] Compilation (options: CompilationOptions, asyncLazyGetChecker: AsyncLazy<IncrementalChecker>, version: VersionStamp) =

    member this.Version = version

    member this.Options = options

    member this.CheckAsync (filePath) =
        async {
            let! checker = asyncLazyGetChecker.GetValueAsync ()
            let! tcAcc, tcResolutionsOpt = checker.CheckAsync (filePath)
            ()
        }

    static member Create (options: CompilationOptions, frameworkTcImportsCache) =
        let asyncLazyGetChecker =
            AsyncLazy (CompilationWorker.EnqueueAndAwaitAsync (fun ctok -> options.CreateIncrementalChecker frameworkTcImportsCache ctok))
        Compilation (options, asyncLazyGetChecker, VersionStamp.Create ())

[<Sealed>]
type CompilationService (_compilationCacheSize: int, frameworkTcImportsCacheStrongSize, workspace: Microsoft.CodeAnalysis.Workspace) =
    // Caches
    let frameworkTcImportsCache = FrameworkImportsCache frameworkTcImportsCacheStrongSize
    let temporaryStorageService = workspace.Services.TemporaryStorage

    member __.CreateSourceSnapshot (filePath, sourceText) =
        let storage = temporaryStorageService.CreateTemporaryTextStorage ()
        storage.WriteText sourceText

        match
            temporaryStorageService.CreateSourceSnapshot (filePath, sourceText)
            |> Cancellable.run CancellationToken.None with
        | ValueOrCancelled.Value result -> result
        | ValueOrCancelled.Cancelled ex -> raise ex

    member __.CreateCompilation options = Compilation.Create (options, frameworkTcImportsCache)