namespace FSharp.Compiler

open System
open System.Threading
open System.Collections.Immutable
open Internal.Utilities.Collections
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.CompileOptions

[<AutoOpen>]
module Helpers =

     let toAsync e = 
         async { 
           let! ct = Async.CancellationToken
           return! 
              Async.FromContinuations(fun (cont, econt, ccont) -> 
                // Run the computation synchronously using the given cancellation token
                let res = try Choice1Of2 (Cancellable.run ct e) with err -> Choice2Of2 err
                match res with 
                | Choice1Of2 (ValueOrCancelled.Value v) -> cont v
                | Choice1Of2 (ValueOrCancelled.Cancelled err) -> ccont err
                | Choice2Of2 err -> econt err) 
         }

     [<Sealed>]
     type AsyncLazy<'T> (computation: Async<'T>) =

        let syncLock = obj ()
        let mutable cachedResult = ValueNone
        let mutable currentThreadId = 0

        let isLocked () = currentThreadId <> 0

        member __.GetValueAsync () =
            async {
                if currentThreadId = Environment.CurrentManagedThreadId then
                    failwith "AsyncLazy tried to re-enter computation recursively."

                match cachedResult with
                | ValueSome result -> return result
                | _ ->
                    let! cancellationToken = Async.CancellationToken

                    use _cancellationTokenRegistration = cancellationToken.Register((fun o -> lock o (fun () -> Monitor.PulseAll o |> ignore)), syncLock, useSynchronizationContext = false)

                    let spin = SpinWait ()
                    while isLocked () && not spin.NextSpinWillYield do
                        spin.SpinOnce ()

                    lock syncLock <| fun () ->
                        while isLocked () do
                            cancellationToken.ThrowIfCancellationRequested ()
                            Monitor.Wait syncLock |> ignore
                        currentThreadId <- Environment.CurrentManagedThreadId

                    try
                        cancellationToken.ThrowIfCancellationRequested ()

                        match cachedResult with
                        | ValueSome result -> return result
                        | _ ->
                            let! result = computation
                            cachedResult <- ValueSome result
                            return result
                    finally
                        lock syncLock <| fun() ->
                            currentThreadId <- 0
                            Monitor.Pulse syncLock |> ignore
                    
            }

/// Global service state
type FrameworkImportsCacheKey = (*resolvedpath*)string list * string * (*TargetFrameworkDirectories*)string list* (*fsharpBinaries*)string

/// Represents a cache of 'framework' references that can be shared betweeen multiple incremental builds
type FrameworkImportsCache(keepStrongly) = 

    // Mutable collection protected via CompilationThreadToken 
    let frameworkTcImportsCache = AgedLookup<CompilationThreadToken, FrameworkImportsCacheKey, (TcGlobals * TcImports)>(keepStrongly, areSimilar=(fun (x, y) -> x = y)) 

    /// Reduce the size of the cache in low-memory scenarios
    member __.Downsize ctok = frameworkTcImportsCache.Resize(ctok, keepStrongly=0)

    /// Clear the cache
    member __.Clear ctok = frameworkTcImportsCache.Clear ctok

    /// This function strips the "System" assemblies from the tcConfig and returns a age-cached TcImports for them.
    member __.Get(ctok, tcConfig: TcConfig) =
      cancellable {
        // Split into installed and not installed.
        let frameworkDLLs, nonFrameworkResolutions, unresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(ctok, tcConfig)
        let frameworkDLLsKey = 
            frameworkDLLs 
            |> List.map (fun ar->ar.resolvedPath) // The cache key. Just the minimal data.
            |> List.sort  // Sort to promote cache hits.

        let! tcGlobals, frameworkTcImports = 
          cancellable {
            // Prepare the frameworkTcImportsCache
            //
            // The data elements in this key are very important. There should be nothing else in the TcConfig that logically affects
            // the import of a set of framework DLLs into F# CCUs. That is, the F# CCUs that result from a set of DLLs (including
            // FSharp.Core.dll and mscorlib.dll) must be logically invariant of all the other compiler configuration parameters.
            let key = (frameworkDLLsKey, 
                        tcConfig.primaryAssembly.Name, 
                        tcConfig.GetTargetFrameworkDirectories(), 
                        tcConfig.fsharpBinariesDir)

            match frameworkTcImportsCache.TryGet (ctok, key) with 
            | Some res -> return res
            | None -> 
                let tcConfigP = TcConfigProvider.Constant tcConfig
                let! ((tcGlobals, tcImports) as res) = TcImports.BuildFrameworkTcImports (ctok, tcConfigP, frameworkDLLs, nonFrameworkResolutions)
                frameworkTcImportsCache.Put(ctok, key, res)
                return tcGlobals, tcImports
          }
        return tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolved
      }

[<Struct>]
type Version =
    {
        dateTime: DateTime
    }

[<Struct>]
type SourceFileId =
    {
        guid: Guid
    }

type SourceFile =
    {
        id: SourceFileId
        filePath: string
    }

[<Struct>]
type ProjectId =
    {
        guid: Guid
    }

type Project =
    {
        [<DefaultValue>] mutable asyncLazyGetCompilation: AsyncLazy<Compilation option>

        // --
        id: ProjectId
        filePath: string
        directory: string
        sourceFiles: ImmutableArray<SourceFileId>
        projectReferences: ImmutableArray<ProjectId>
        options: string list
        useScriptResolutionsRules: bool
        loadClosureOpt: LoadClosure option
        [<DefaultValue>] mutable solution: Solution
        dependentVersion: Version
        version: Version
    }

    member this.TryGetCompilationAsync () : Async<Compilation option> =
        async {
            try
                return! this.asyncLazyGetCompilation.GetValueAsync ()
            with
            | _ -> return None
        }

    member this.TryCreateCompilationAsync () =
        let useSimpleResolutionSwitch = "--simpleresolution"
        let ctok = this.solution.compilationThreadToken
        let commandLineArgs = this.options
        let legacyReferenceResolver = this.solution.legacyReferenceResolver
        let defaultFSharpBinariesDir = this.solution.defaultFSharpBinariesDir
        let projectDirectory = this.directory
        let tryGetMetadataSnapshot = this.solution.tryGetMetadataSnapshot
        let useScriptResolutionRules = this.useScriptResolutionsRules
        let loadClosureOpt = this.loadClosureOpt
        let frameworkTcImportsCache = this.solution.frameworkImportsCache
        let sourceFiles =
            this.sourceFiles
            |> Seq.map (fun sourceFileId ->
                match this.solution.TryGetSourceFile sourceFileId with
                | Some sourceFile -> sourceFile.filePath
                | _ -> failwith "did not find source file"
            )
            |> List.ofSeq

        let projectReferences =
            this.projectReferences
            |> Seq.map (fun projectId ->
                match this.solution.TryGetProject projectId with
                | Some (project: Project) -> 
                    { new IProjectReference with

                        member __.EvaluateRawContents _ctok =
                            cancellable {
                                let! cancellationToken = Cancellable.token ()
                                let computation : Async<Compilation option> = project.TryGetCompilationAsync ()
                                let result = Async.RunSynchronously (computation, cancellationToken = cancellationToken)
                                match result with
                                | None -> return None
                                | Some compilation ->
                                    return compilation.tcAssemblyData
                            }

                        member __.FileName = project.filePath

                        member __.TryGetLogicalTimeStamp (_, _) = Some project.dependentVersion.dateTime
                            
                    }
                | _ -> failwith "did not find project"
            )
            |> List.ofSeq

        // TODO:
        let sourceFileToFilePathMap = ImmutableDictionary.Empty

        cancellable {

          // Trap and report warnings and errors from creation.
          let delayedLogger = CapturingErrorLogger("IncrementalBuilderCreation")
          use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
          use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter

          let! builderOpt =
           cancellable {
            try

              // Create the builder.         
              // Share intern'd strings across all lexing/parsing
              let resourceManager = new Lexhelp.LexResourceManager() 

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
              let _niceNameGen = NiceNameGenerator()
              let _outfile, _, _assemblyName = tcConfigB.DecideNames sourceFilesNew

              // Resolve assemblies and create the framework TcImports. This is done when constructing the
              // builder itself, rather than as an incremental task. This caches a level of "system" references. No type providers are 
              // included in these references. 
              let! (_tcGlobals, _frameworkTcImports, nonFrameworkResolutions, _unresolvedReferences) = frameworkTcImportsCache.Get(ctok, tcConfig)

              // Note we are not calling errorLogger.GetErrors() anywhere for this task. 
              // This is ok because not much can actually go wrong here.
              let errorOptions = tcConfig.errorSeverityOptions
              let errorLogger = CompilationErrorLogger("nonFrameworkAssemblyInputs", errorOptions)
              // Return the disposable object that cleans up
              use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter) 

              // Get the names and time stamps of all the non-framework referenced assemblies, which will act 
              // as inputs to one of the nodes in the build. 
              //
              // This operation is done when constructing the builder itself, rather than as an incremental task. 
              let _nonFrameworkAssemblyInputs = 
                  // Note we are not calling errorLogger.GetErrors() anywhere for this task. 
                  // This is ok because not much can actually go wrong here.
                  let errorLogger = CompilationErrorLogger("nonFrameworkAssemblyInputs", errorOptions)
                  // Return the disposable object that cleans up
                  use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter) 

                  [ for r in nonFrameworkResolutions do
                      let fileName = r.resolvedPath
                      yield (Choice1Of2 fileName, (fun (cache: TimeStampCache) _ctokk -> cache.GetFileTimeStamp fileName))  

                    for pr in projectReferences  do
                      yield Choice2Of2 pr, (fun (cache: TimeStampCache) ctok -> cache.GetProjectReferenceTimeStamp (pr, ctok)) ]
      
              let compilation = { project = this; tcAssemblyData = None; tcConfig = tcConfig; lexResourceManager = resourceManager; sourceFileToFilePathMap = sourceFileToFilePathMap }
              return Some compilation
            with e -> 
              errorRecoveryNoRange e
              return None
           }

          return builderOpt
        } |> toAsync

and Solution =
    {
        compilationThreadToken: CompilationThreadToken
        legacyReferenceResolver: ReferenceResolver.Resolver
        defaultFSharpBinariesDir: string
        frameworkImportsCache: FrameworkImportsCache
        tryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
        suggestNamesForErrors: bool

        // ---
        projects: ImmutableDictionary<ProjectId, Project>
        sourceFiles: ImmutableDictionary<SourceFileId, SourceFile>
        version: Version
    }

    member this.TryGetProject projectId =
        match this.projects.TryGetValue projectId with
        | true, project -> Some project
        | _ -> None

    member this.TryGetSourceFile sourceFileId : SourceFile option =
        match this.sourceFiles.TryGetValue sourceFileId with
        | true, sourceFile -> Some sourceFile
        | _ -> None

    member this.AddProject (filePath, directory, sourceFiles, projectReferences, options, useScriptResolutionsRules, loadClosureOpt) =
        let project =
            {
                id = { guid = Guid.NewGuid () }
                filePath = filePath
                directory = directory
                sourceFiles = sourceFiles
                projectReferences = projectReferences
                options = options
                useScriptResolutionsRules = useScriptResolutionsRules
                loadClosureOpt = loadClosureOpt
                dependentVersion = { dateTime = DateTime.UtcNow }
                version = { dateTime = DateTime.UtcNow }
            }

        project.solution <- { this with projects = this.projects.Add(project.id, project); version = { dateTime = DateTime.UtcNow } }
        project.asyncLazyGetCompilation <- AsyncLazy (project.TryCreateCompilationAsync ())
        project

and Compilation =
    {
        mutable tcAssemblyData: IRawFSharpAssemblyData option
        tcConfig: TcConfig
        sourceFileToFilePathMap: ImmutableDictionary<SourceFileId, string>
        lexResourceManager: Lexhelp.LexResourceManager

        // --
        project: Project
    }

    member this.TryParseSourceFileAsync sourceFileId =
        async {
            try
                let filename = this.sourceFileToFilePathMap.[sourceFileId]
                let errorLogger = CompilationErrorLogger("TryParseSourceFileAsync", this.tcConfig.errorSeverityOptions)
                let input = ParseOneInputFile (this.tcConfig, this.lexResourceManager, [], filename, (false, false), errorLogger, (*retrylocked*) true)
                return Some (input, errorLogger.GetErrors ())
            with
            | _ ->
                return None
        }

    member this.TryGetAssemblyData () =
        this.tcAssemblyData






        

