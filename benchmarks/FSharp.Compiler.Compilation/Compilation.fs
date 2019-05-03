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
type CompilationId private (guid: Guid) =

    member private __.Guid = guid

    static member Create () = CompilationId (Guid.NewGuid ())

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

and [<Sealed>] Compilation (id: CompilationId, options: CompilationOptions, asyncLazyGetChecker: AsyncLazy<IncrementalChecker>, version: VersionStamp) =

    member __.Id = id

    member __.Version = version

    member __.Options = options

    member __.CheckAsync (filePath) =
        async {
            let! checker = asyncLazyGetChecker.GetValueAsync ()
            let! tcAcc, tcResolutionsOpt = checker.CheckAsync (filePath)
            printfn "%A" (tcAcc.tcErrorsRev)
            ()
        }

module Compilation =

    let create (options: CompilationOptions) frameworkTcImportsCache =
        let asyncLazyGetChecker =
            AsyncLazy (CompilationWorker.EnqueueAndAwaitAsync (fun ctok -> options.CreateIncrementalChecker frameworkTcImportsCache ctok))
        Compilation (CompilationId.Create (), options, asyncLazyGetChecker, VersionStamp.Create ())
