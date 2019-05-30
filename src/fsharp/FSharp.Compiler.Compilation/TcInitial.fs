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

type TcInitialOptions =
    {
        frameworkTcImportsCache: FrameworkImportsCache
        legacyReferenceResolver: ReferenceResolver.Resolver
        defaultFSharpBinariesDir: string
        tryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
        suggestNamesForErrors: bool
        sourceFiles: string list
        commandLineArgs: string list
        projectDirectory: string
        projectReferences: IProjectReference list
        useScriptResolutionRules: bool
        assemblyPath: string
        isExecutable: bool
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
    }

type internal TcInitial =
    {
        tcConfig: TcConfig
        tcConfigP: TcConfigProvider
        tcGlobals: TcGlobals
        frameworkTcImports: TcImports
        nonFrameworkResolutions: AssemblyResolution list
        unresolvedReferences: UnresolvedAssemblyReference list
        importsInvalidated: Event<string>
        assemblyName: string
        outfile: string
        niceNameGen: NiceNameGenerator
        loadClosureOpt: LoadClosure option
        projectDirectory: string
    }

module TcInitial =

    let create options ctok =
        let frameworkTcImportsCache = options.frameworkTcImportsCache
        let sourceFiles = options.sourceFiles
        let commandLineArgs = options.commandLineArgs
        let legacyReferenceResolver = options.legacyReferenceResolver
        let defaultFSharpBinariesDir = options.defaultFSharpBinariesDir
        let projectDirectory = options.projectDirectory
        let projectReferences = options.projectReferences
        let tryGetMetadataSnapshot = options.tryGetMetadataSnapshot
        let useScriptResolutionRules = options.useScriptResolutionRules
        let loadClosureOpt : LoadClosure option = None // TODO:

        // Trap and report warnings and errors from creation.
        let delayedLogger = CapturingErrorLogger("IncrementalBuilderCreation")
        use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
        use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter

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
        let outfile, _, assemblyName = tcConfigB.DecideNames sourceFilesNew

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

        {
            tcConfig = tcConfig
            tcConfigP = TcConfigProvider.Constant tcConfig
            tcGlobals = tcGlobals
            frameworkTcImports = frameworkTcImports
            nonFrameworkResolutions = nonFrameworkResolutions
            unresolvedReferences = unresolvedReferences
            importsInvalidated = Event<string> () // TODO:
            assemblyName = assemblyName
            outfile = outfile
            niceNameGen = niceNameGen
            loadClosureOpt = loadClosureOpt
            projectDirectory = projectDirectory
        }
