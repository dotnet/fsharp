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

[<NoEquality;NoComparison>]
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
    }

    static member Create (assemblyPath, commandLineArgs, projectDirectory, isExecutable) =
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
            CommandLineArgs = commandLineArgs
            ProjectDirectory = projectDirectory
            UseScriptResolutionRules = useScriptResolutionRules
            AssemblyPath = assemblyPath
            IsExecutable = isExecutable
            KeepAssemblyContents = false
            KeepAllBackgroundResolutions = false
        }

type [<NoEquality;NoComparison>] Compilation =
    {
        checker: IncrementalChecker
        options: CompilationOptions
    }

    member this.Version = this.checker.Version

    member this.Check (filePath, cancellationToken) =
        let tcAcc, tcResolutionsOpt = this.checker.Check (filePath, cancellationToken)
      //  printfn "%A" (tcAcc.tcDependencyFiles)
        ()

type [<NoEquality;NoComparison>] CompilationInfo =
    {
        Options: CompilationOptions
        SourceSnapshots: ImmutableArray<SourceSnapshot>
        CompilationReferences: ImmutableArray<Compilation>
    }

[<Sealed>]
type CompilationService (_compilationCacheSize: int, frameworkTcImportsCacheStrongSize, workspace: Microsoft.CodeAnalysis.Workspace) =
    let ctok = CompilationThreadToken ()
    let gate = NonReentrantLock ()

    let _takeLock () =
        async {
            let! cancellationToken = Async.CancellationToken
            return gate.Wait cancellationToken
        }

    let _takeLockCancellable () =
        cancellable {
            let! cancellationToken = Cancellable.token ()
            return gate.Wait cancellationToken
        }

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

    member this.CreateCompilation info =
        let useSimpleResolutionSwitch = "--simpleresolution"
        let commandLineArgs = info.Options.CommandLineArgs
        let legacyReferenceResolver = info.Options.LegacyReferenceResolver
        let defaultFSharpBinariesDir = info.Options.DefaultFSharpBinariesDir
        let projectDirectory = info.Options.ProjectDirectory
        let tryGetMetadataSnapshot = info.Options.TryGetMetadataSnapshot
        let useScriptResolutionRules = info.Options.UseScriptResolutionRules
        let loadClosureOpt : LoadClosure option = None // TODO:

        // Share intern'd strings across all lexing/parsing
        let lexResourceManager = Lexhelp.LexResourceManager ()

        let projectReferences =
            info.CompilationReferences
            |> Seq.map (fun compilation ->
                { new IProjectReference with

                    member __.EvaluateRawContents _ctok =
                        cancellable {
                        return None
                        }

                    member __.FileName = compilation.options.AssemblyPath

                    member __.TryGetLogicalTimeStamp (_, _) =
                        Some compilation.Version.DateTime                              
                }
            )
            |> List.ofSeq

        // Trap and report warnings and errors from creation.
        let delayedLogger = CapturingErrorLogger("IncrementalBuilderCreation")
        use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
        use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter

        let builderOpt =
            /// Create a type-check configuration
            let tcConfigB, sourceFilesNew = 

                let getSwitchValue switchstring =
                    match commandLineArgs |> Seq.tryFindIndex(fun s -> s.StartsWithOrdinal switchstring) with
                    | Some idx -> Some(commandLineArgs.[idx].Substring(switchstring.Length))
                    | _ -> None

                // see also fsc.fs: runFromCommandLineToImportingAssemblies(), as there are many similarities to where the PS creates a tcConfigB
                let tcConfigB = 
                    TcConfigBuilder.CreateNew(legacyReferenceResolver, 
                        defaultFSharpBinariesDir, 
                        implicitIncludeDir=projectDirectory, 
                        reduceMemoryUsage=ReduceMemoryFlag.Yes, 
                        isInteractive=false, 
                        isInvalidationSupported=true, 
                        defaultCopyFSharpCore=CopyFSharpCoreFlag.No, 
                        tryGetMetadataSnapshot=tryGetMetadataSnapshot) 

                tcConfigB.resolutionEnvironment <- (ReferenceResolver.ResolutionEnvironment.EditingOrCompilation true)

                tcConfigB.conditionalCompilationDefines <- 
                    let define = if useScriptResolutionRules then "INTERACTIVE" else "COMPILED"
                    define :: tcConfigB.conditionalCompilationDefines

                tcConfigB.projectReferences <- projectReferences

                tcConfigB.useSimpleResolution <- (getSwitchValue useSimpleResolutionSwitch) |> Option.isSome

                let sourceFiles =
                    info.SourceSnapshots
                    |> Seq.map (fun x -> x.FilePath)
                    |> List.ofSeq

                // Apply command-line arguments and collect more source files if they are in the arguments
                let sourceFilesNew = ApplyCommandLineArgs(tcConfigB, sourceFiles, commandLineArgs)

                // Never open PDB files for the language service, even if --standalone is specified
                tcConfigB.openDebugInformationForLaterStaticLinking <- false

                tcConfigB, sourceFilesNew

            match loadClosureOpt with
            | Some loadClosure ->
                let dllReferences =
                    [for reference in tcConfigB.referencedDLLs do
                        // If there's (one or more) resolutions of closure references then yield them all
                        match loadClosure.References  |> List.tryFind (fun (resolved, _)->resolved=reference.Text) with
                        | Some (resolved, closureReferences) -> 
                            for closureReference in closureReferences do
                                yield AssemblyReference(closureReference.originalReference.Range, resolved, None)
                        | None -> yield reference]
                tcConfigB.referencedDLLs <- []
                // Add one by one to remove duplicates
                dllReferences |> List.iter (fun dllReference ->
                    tcConfigB.AddReferencedAssemblyByPath(dllReference.Range, dllReference.Text))
                tcConfigB.knownUnresolvedReferences <- loadClosure.UnresolvedReferences
            | None -> ()

            let tcConfig = TcConfig.Create(tcConfigB, validate=true)
            let niceNameGen = NiceNameGenerator()
            let _outfile, _, assemblyName = tcConfigB.DecideNames sourceFilesNew

            // Resolve assemblies and create the framework TcImports. This is done when constructing the
            // builder itself, rather than as an incremental task. This caches a level of "system" references. No type providers are 
            // included in these references. 
            let (tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolvedReferences) = 
                match Cancellable.run CancellationToken.None (frameworkTcImportsCache.Get(ctok, tcConfig)) with
                | ValueOrCancelled.Value result -> result
                | ValueOrCancelled.Cancelled ex -> raise ex

            // Note we are not calling errorLogger.GetErrors() anywhere for this task. 
            // This is ok because not much can actually go wrong here.
            let errorOptions = tcConfig.errorSeverityOptions
            let errorLogger = CompilationErrorLogger("nonFrameworkAssemblyInputs", errorOptions)
            // Return the disposable object that cleans up
            use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter)

            let initialInfo =
                {
                    ctok = ctok
                    temporaryStorageService = temporaryStorageService
                    tcConfig = tcConfig
                    tcConfigP = TcConfigProvider.Constant tcConfig
                    tcGlobals = tcGlobals
                    frameworkTcImports = frameworkTcImports
                    nonFrameworkResolutions = nonFrameworkResolutions
                    unresolvedReferences = unresolvedReferences
                    importsInvalidated = Event<string> () // TODO:
                    assemblyName = assemblyName
                    niceNameGen = niceNameGen
                    loadClosureOpt = loadClosureOpt
                    projectDirectory = info.Options.ProjectDirectory
                    checkerOptions =
                        {
                            keepAssemblyContents = info.Options.KeepAssemblyContents
                            keepAllBackgroundResolutions = info.Options.KeepAllBackgroundResolutions
                            parsingOptions =
                                {
                                    isExecutable = info.Options.IsExecutable
                                    lexResourceManager = lexResourceManager
                                }
                        }
                    sourceSnapshots = info.SourceSnapshots
                }

            let checker = 
                match IncrementalChecker.Create initialInfo |> Cancellable.run CancellationToken.None with
                | ValueOrCancelled.Value result -> result
                | ValueOrCancelled.Cancelled ex -> raise ex
            {
                checker = checker
                options = info.Options
            }

        builderOpt
