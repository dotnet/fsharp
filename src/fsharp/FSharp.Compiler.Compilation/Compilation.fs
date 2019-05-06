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

[<Struct>]
type CompilationId private (_guid: Guid) =

    static member Create () = CompilationId (Guid.NewGuid ())

type CompilationCaches =
    {
        incrementalCheckerCache: MruWeakCache<struct (CompilationId * VersionStamp), IncrementalChecker>
        frameworkTcImportsCache: FrameworkImportsCache
    }

type CompilationGlobalOptions =
    {
        LegacyReferenceResolver: ReferenceResolver.Resolver
        DefaultFSharpBinariesDir: string
        TryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
    }

    static member Create () =
        let legacyReferenceResolver = SimulatedMSBuildReferenceResolver.GetBestAvailableResolver()
        
        // TcState is arbitrary, doesn't matter as long as the type is inside FSharp.Compiler.Private, also this is yuck.
        let defaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(Some(typeof<FSharp.Compiler.CompileOps.TcState>.Assembly.Location)).Value
        
        let tryGetMetadataSnapshot = (fun _ -> None)
        
        {
            LegacyReferenceResolver = legacyReferenceResolver
            DefaultFSharpBinariesDir = defaultFSharpBinariesDir
            TryGetMetadataSnapshot = tryGetMetadataSnapshot
        }

type CompilationOptions =
    {
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
        let suggestNamesForErrors = false
        let useScriptResolutionRules = false
        {
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

    member options.CreateTcInitial (frameworkTcImportsCache, globalOptions, ctok) =
        let tcInitialOptions =
            {
                frameworkTcImportsCache = frameworkTcImportsCache
                legacyReferenceResolver = globalOptions.LegacyReferenceResolver
                defaultFSharpBinariesDir = globalOptions.DefaultFSharpBinariesDir
                tryGetMetadataSnapshot = globalOptions.TryGetMetadataSnapshot
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

    member options.CreateIncrementalChecker (frameworkTcImportsCache, globalOptions, ctok) =
        let tcInitial = options.CreateTcInitial (frameworkTcImportsCache, globalOptions, ctok)
        let tcImports, tcAcc = TcAccumulator.createInitial tcInitial ctok |> Cancellable.runWithoutCancellation
        let checkerOptions =
            {
                keepAssemblyContents = options.KeepAssemblyContents
                keepAllBackgroundResolutions = options.KeepAllBackgroundResolutions
                parsingOptions = { isExecutable = options.IsExecutable }
            }
        IncrementalChecker.create tcInitial.tcConfig tcInitial.tcGlobals tcImports tcAcc checkerOptions options.SourceSnapshots
        |> Cancellable.runWithoutCancellation

and [<NoEquality; NoComparison>] CompilationState =
    {
        filePathIndexMap: ImmutableDictionary<string, int>
        caches: CompilationCaches
        options: CompilationOptions
        globalOptions: CompilationGlobalOptions
        asyncLazyGetChecker: AsyncLazy<IncrementalChecker>
    }

    static member Create (options, globalOptions, caches: CompilationCaches) =
        let sourceSnapshots = options.SourceSnapshots

        let filePathIndexMapBuilder = ImmutableDictionary.CreateBuilder (StringComparer.OrdinalIgnoreCase)
        for i = 0 to sourceSnapshots.Length - 1 do
            let sourceSnapshot = sourceSnapshots.[i]
            if filePathIndexMapBuilder.ContainsKey sourceSnapshot.FilePath then
                failwithf "Duplicate file path when creating a compilation. File path: %s" sourceSnapshot.FilePath
            else
                filePathIndexMapBuilder.Add (sourceSnapshot.FilePath, i)

        let asyncLazyGetChecker =
            AsyncLazy (async {
                return! CompilationWorker.EnqueueAndAwaitAsync (fun ctok -> 
                    options.CreateIncrementalChecker (caches.frameworkTcImportsCache, globalOptions, ctok)
                )
            })

        {
            filePathIndexMap = filePathIndexMapBuilder.ToImmutableDictionary ()
            caches = caches
            options = options
            globalOptions = globalOptions
            asyncLazyGetChecker = asyncLazyGetChecker
        }

    member this.SetOptions options =
        CompilationState.Create (options, this.globalOptions, this.caches)

    member this.ReplaceSourceSnapshot (sourceSnapshot: SourceSnapshot) =
        match this.filePathIndexMap.TryGetValue sourceSnapshot.FilePath with
        | false, _ -> failwith "source snapshot does not exist in compilation"
        | _, filePathIndex ->

            let sourceSnapshotsBuilder = this.options.SourceSnapshots.ToBuilder ()

            sourceSnapshotsBuilder.[filePathIndex] <- sourceSnapshot

            let options = { this.options with SourceSnapshots = sourceSnapshotsBuilder.MoveToImmutable () }

            let asyncLazyGetChecker =
                AsyncLazy (async {
                    let! checker = this.asyncLazyGetChecker.GetValueAsync ()
                    return checker.ReplaceSourceSnapshot sourceSnapshot
                })

            { this with options = options; asyncLazyGetChecker = asyncLazyGetChecker }

and [<Sealed>] Compilation (id: CompilationId, state: CompilationState, version: VersionStamp) =

    let asyncLazyGetChecker =
        AsyncLazy (async {
            match state.caches.incrementalCheckerCache.TryGetValue struct (id, version) with
            | ValueSome checker -> return checker
            | _ -> 
                let! checker = state.asyncLazyGetChecker.GetValueAsync ()
                state.caches.incrementalCheckerCache.Set (struct (id, version), checker)
                return checker
        })

    let checkFilePath filePath =
        if not (state.filePathIndexMap.ContainsKey filePath) then
            failwithf "File path does not exist in compilation. File path: %s" filePath

    member __.Id = id

    member __.Version = version

    member __.Options = state.options

    member __.SetOptions (options: CompilationOptions) =
        Compilation (id, state.SetOptions options, version.NewVersionStamp ())

    member __.ReplaceSourceSnapshot (sourceSnapshot: SourceSnapshot) =
        checkFilePath sourceSnapshot.FilePath
        Compilation (id, state.ReplaceSourceSnapshot sourceSnapshot, version.NewVersionStamp ())

    member __.GetSemanticModel filePath =
        checkFilePath filePath
        SemanticModel asyncLazyGetChecker

module Compilation =

    let create options globalOptions caches =
        Compilation (CompilationId.Create (), CompilationState.Create (options, globalOptions, caches), VersionStamp.Create ())
