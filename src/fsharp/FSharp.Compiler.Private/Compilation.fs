namespace FSharp.Compiler

open System
open System.Threading
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
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
open Internal.Utilities

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
     type NonReentrantLock () =

        let syncLock = obj ()
        let mutable currentThreadId = 0

        let isLocked () = currentThreadId <> 0

        member this.Wait (cancellationToken: CancellationToken) =

            if currentThreadId = Environment.CurrentManagedThreadId then
                failwith "AsyncLazy tried to re-enter computation recursively."
            
            use _cancellationTokenRegistration = cancellationToken.Register((fun o -> lock o (fun () -> Monitor.PulseAll o |> ignore)), syncLock, useSynchronizationContext = false)

            let spin = SpinWait ()
            while isLocked () && not spin.NextSpinWillYield do
                spin.SpinOnce ()

            lock syncLock <| fun () ->
                while isLocked () do
                    cancellationToken.ThrowIfCancellationRequested ()
                    Monitor.Wait syncLock |> ignore
                currentThreadId <- Environment.CurrentManagedThreadId

            new SemaphoreDisposer (this)

        member this.Release () =
            lock syncLock <| fun() ->
                currentThreadId <- 0
                Monitor.Pulse syncLock |> ignore

     and [<Struct>] SemaphoreDisposer (semaphore: NonReentrantLock) =

        interface IDisposable with

            member __.Dispose () = semaphore.Release ()

     [<Sealed>]
     type AsyncLazy<'T> (computation: Async<'T>) =

        let gate = NonReentrantLock ()
        let mutable cachedResult = ValueNone

        member __.GetValueAsync () =
            async {
                match cachedResult with
                | ValueSome result -> return result
                | _ ->
                    let! cancellationToken = Async.CancellationToken
                    use _semaphoreDisposer = gate.Wait cancellationToken

                    cancellationToken.ThrowIfCancellationRequested ()

                    match cachedResult with
                    | ValueSome result -> return result
                    | _ ->
                        let! result = computation
                        cachedResult <- ValueSome result
                        return result
                    
            }

/// Global service state
type FrameworkImportCacheKey = (*resolvedpath*)string list * string * (*TargetFrameworkDirectories*)string list* (*fsharpBinaries*)string

/// Represents a cache of 'framework' references that can be shared betweeen multiple incremental builds
type FrameworkImportCache(keepStrongly) = 

    // Mutable collection protected via CompilationThreadToken 
    let frameworkTcImportCache = AgedLookup<CompilationThreadToken, FrameworkImportCacheKey, (TcGlobals * TcImports)>(keepStrongly, areSimilar=(fun (x, y) -> x = y)) 

    /// Reduce the size of the cache in low-memory scenarios
    member __.Downsize ctok = frameworkTcImportCache.Resize(ctok, keepStrongly=0)

    /// Clear the cache
    member __.Clear ctok = frameworkTcImportCache.Clear ctok

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

            match frameworkTcImportCache.TryGet (ctok, key) with 
            | Some res -> return res
            | None -> 
                let tcConfigP = TcConfigProvider.Constant tcConfig
                let! ((tcGlobals, tcImports) as res) = TcImports.BuildFrameworkTcImports (ctok, tcConfigP, frameworkDLLs, nonFrameworkResolutions)
                frameworkTcImportCache.Put(ctok, key, res)
                return tcGlobals, tcImports
          }
        return tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolved
      }


[<Struct>]
type Stamp (dateTime: DateTime) =

    member __.DateTime = dateTime

    static member Create () = Stamp DateTime.UtcNow

[<Struct>]
type SourceId (guid: Guid) =

    member __.Guid = guid

    static member Create () = SourceId (Guid.NewGuid ())

[<NoEquality;NoComparison>]
type Source =
    {
        id: SourceId
        filePath: string
        sourceText: FSharp.Compiler.Text.ISourceText option
    }

    member this.Id = this.id
        
    member this.FilePath = this.filePath

    // TODO: Make this async?
    member this.TryGetSourceText () = this.sourceText

[<NoEquality;NoComparison>]
type CompilationOptions =
    {
        CompilationThreadToken: CompilationThreadToken
        LegacyReferenceResolver: ReferenceResolver.Resolver
        DefaultFSharpBinariesDir: string
        TryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
        AssemblyPath: string
    }

    static member Create (assemblyPath, commandLineArgs, projectDirectory) =
        let compilationThreadToken = CompilationThreadToken ()
        let legacyReferenceResolver = SimulatedMSBuildReferenceResolver.GetBestAvailableResolver()
        let defaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(Some(typeof<FSharpCheckFileAnswer>.Assembly.Location)).Value
        let tryGetMetadataSnapshot = (fun _ -> None)
        let suggestNamesForErrors = false
        let useScriptResolutionRules = false
        {
            CompilationThreadToken = compilationThreadToken
            LegacyReferenceResolver = legacyReferenceResolver
            DefaultFSharpBinariesDir = defaultFSharpBinariesDir
            TryGetMetadataSnapshot = tryGetMetadataSnapshot
            SuggestNamesForErrors = suggestNamesForErrors
            CommandLineArgs = commandLineArgs
            ProjectDirectory = projectDirectory
            UseScriptResolutionRules = useScriptResolutionRules
            AssemblyPath = assemblyPath
        }

[<Struct>]
type CompilationId (guid: Guid) =

    member __.Guid = guid

    static member Create () = CompilationId (Guid.NewGuid ())

[<NoEquality;NoComparison>]
type CompilationState =
    {
        tcAssemblyData: IRawFSharpAssemblyData option
        sources: ImmutableArray<SourceId>
        sourceSet: ImmutableHashSet<SourceId>
        stamp: Stamp
    }

    member this.TryGetAssemblyData () = this.tcAssemblyData

    static member Create (sources: SourceId seq) =
        {
            tcAssemblyData = None
            sources = ImmutableArray.CreateRange sources
            sourceSet = ImmutableHashSet.CreateRange sources
            stamp = Stamp.Create ()
        }

[<Sealed;NoEquality;NoComparison>]
type Compilation (id: CompilationId, lexResourceManager: Lexhelp.LexResourceManager, tcConfig: TcConfig, options: CompilationOptions, state: CompilationState) =

    member this.Id = id

    member this.AssemblyPath = options.AssemblyPath

    member this.LexResourceManager = lexResourceManager

    member this.TcConfig = tcConfig

    member this.Options = options

    // TODO: Make this async?
    member this.TryGetAssemblyData () = state.tcAssemblyData

    member this.HasSource sourceId = state.sourceSet.Contains sourceId

    member this.Stamp = state.stamp

    static member Create (compilationId, lexResourceManager, options, sources, tcConfig) =
        Compilation (compilationId, lexResourceManager, tcConfig, options, CompilationState.Create sources)
    
type [<NoEquality;NoComparison>] CompilationInfo =
    {
        Options: CompilationOptions
        Sources: SourceId seq
        CompilationReferences: CompilationId seq
    }

type [<NoEquality;NoComparison>] CompilationManagerState =
    {
        compilationMap: ImmutableDictionary<CompilationId, Compilation>
        sourceMap: ImmutableDictionary<SourceId, Source>
    }

    member this.TryGetCompilation compilationId =
        match this.compilationMap.TryGetValue compilationId with
        | true, compilation -> Some compilation
        | _ -> None

    member this.TryGetSource sourceId =
        match this.sourceMap.TryGetValue sourceId with
        | true, source -> Some source
        | _ -> None

    member this.AddSources (sources: Source seq) =
        let sourceMap =
            (this.sourceMap, sources)
            ||> Seq.fold (fun sourceMap source ->
                sourceMap.Add (source.Id, source)
            )
        { this with sourceMap = sourceMap }

    member this.AddCompilation (compilation: Compilation) =
        { this with compilationMap = this.compilationMap.Add (compilation.Id, compilation) }

    static member Create () =
        {
            compilationMap = ImmutableDictionary.Empty
            sourceMap = ImmutableDictionary.Empty
        }

type ParseResult = (ParsedInput option * (PhasedDiagnostic * FSharpErrorSeverity) [])

[<Sealed>]
type CompilationManager (_compilationCacheSize: int, frameworkTcImportsCacheStrongSize) =
    let gate = NonReentrantLock ()
    let mutable state = CompilationManagerState.Create ()

    let takeLock () =
        async {
            let! cancellationToken = Async.CancellationToken
            return gate.Wait cancellationToken
        }

    // Caches
    let frameworkTcImportCache = FrameworkImportsCache frameworkTcImportsCacheStrongSize

    member __.AddSourceAsync (filePath: string) =
        async {
            let source =
                {
                    id = SourceId.Create ()
                    filePath = filePath
                    sourceText = None
                }

            use! _semaphoreDisposer = takeLock ()

            state <- state.AddSources [ source ]
            return source
        }      

    member __.TryCreateCompilationAsync info =
        cancellable {
          let! cancellationToken = Cancellable.token ()
          use _semaphoreDisposer = gate.Wait cancellationToken

          let compilationId = CompilationId.Create ()
          let useSimpleResolutionSwitch = "--simpleresolution"
          let ctok = info.Options.CompilationThreadToken
          let commandLineArgs = info.Options.CommandLineArgs
          let legacyReferenceResolver = info.Options.LegacyReferenceResolver
          let defaultFSharpBinariesDir = info.Options.DefaultFSharpBinariesDir
          let projectDirectory = info.Options.ProjectDirectory
          let tryGetMetadataSnapshot = info.Options.TryGetMetadataSnapshot
          let useScriptResolutionRules = info.Options.UseScriptResolutionRules
          let loadClosureOpt : LoadClosure option = None
          let lexResourceManager = Lexhelp.LexResourceManager ()
          let sourceFiles =
              info.Sources
              |> Seq.map (fun sourceId -> 
                  match state.TryGetSource sourceId with
                  | Some source -> source.FilePath
                  | _ -> failwith "no source found"
              )
              |> List.ofSeq

          let projectReferences =
              info.CompilationReferences
              |> Seq.map (fun compilationId ->
                  let filePath =
                      match state.TryGetCompilation compilationId with
                      | Some compilation -> compilation.AssemblyPath 
                      | _ -> failwith "no compilation found"
                  { new IProjectReference with

                      member __.EvaluateRawContents _ctok =
                          cancellable {
                              let! cancellationToken = Cancellable.token ()
                              use _semaphoreDisposer = gate.Wait cancellationToken

                              match state.TryGetCompilation compilationId with
                              | Some compilation -> return compilation.TryGetAssemblyData ()
                              | _ -> return None
                          }

                      member __.FileName = filePath

                      member __.TryGetLogicalTimeStamp (_, _) =
                          use _semaphoreDisposer = gate.Wait cancellationToken

                          match state.TryGetCompilation compilationId with
                          | Some compilation -> Some compilation.Stamp.DateTime
                          | _ -> None
                              
                  }
              )
              |> List.ofSeq

          // Trap and report warnings and errors from creation.
          let delayedLogger = CapturingErrorLogger("IncrementalBuilderCreation")
          use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
          use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter

          let! builderOpt =
           cancellable {
            try

              //// Create the builder.         
              //// Share intern'd strings across all lexing/parsing
              //let resourceManager = new Lexhelp.LexResourceManager() 

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
              let! (_tcGlobals, _frameworkTcImports, nonFrameworkResolutions, _unresolvedReferences) = frameworkTcImportCache.Get(ctok, tcConfig)

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
      
              
              let compilation = Compilation.Create (compilationId, lexResourceManager, info.Options, info.Sources, tcConfig)
              state <- state.AddCompilation compilation
              return Some compilation
            with e -> 
              errorRecoveryNoRange e
              return None
           }

          return builderOpt
        } |> toAsync

    member this.GetSourceAndCompilation (sourceId, compilationId, cancellationToken) =
        use _semaphoreDisposer = gate.Wait cancellationToken
        let compilation =
            match state.TryGetCompilation compilationId with
            | Some compilation -> compilation
            | _ -> failwith "compilation not found"

        let source =
            if not (compilation.HasSource sourceId) then
                failwith "source not found in compilation"

            match state.TryGetSource sourceId with
            | Some source -> source
            | _ -> failwith "source not found"

        struct (source, compilation)

    member this.TryParseSourceFileAsync (sourceId, compilationId) =
        async {
            let! cancellationToken = Async.CancellationToken
            let struct (source, compilation) = this.GetSourceAndCompilation (sourceId, compilationId, cancellationToken)

            try                
                let errorLogger = CompilationErrorLogger("TryParseSourceFileAsync", compilation.TcConfig.errorSeverityOptions)
                let input = ParseOneInputFile (compilation.TcConfig, compilation.LexResourceManager, [], source.FilePath, (false, false), errorLogger, (*retrylocked*) true)
                return Some (input, errorLogger.GetErrors ())
            with
            | _ ->
                return None
        }

    member this.ParseSourceFilesAsync (sources, compilationId) =
        async {
            let! cancellationToken = Async.CancellationToken
            let pairs =
                sources
                |> Seq.map (fun x -> this.GetSourceAndCompilation (x, compilationId, cancellationToken))
                |> Array.ofSeq

            let results = ResizeArray ()

            pairs
            |> Array.iter (fun struct (source, compilation) ->             
                let errorLogger = CompilationErrorLogger("TryParseSourceFileAsync", compilation.TcConfig.errorSeverityOptions)
                let input = ParseOneInputFile (compilation.TcConfig, compilation.LexResourceManager, [], source.FilePath, (false, false), errorLogger, (*retrylocked*) true)
                results.Add (input, errorLogger.GetErrors ())
            )

            return results :> ParseResult seq
        }