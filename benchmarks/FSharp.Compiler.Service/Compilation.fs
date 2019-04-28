namespace FSharp.Compiler.Service

open System
open System.IO
open System.Threading
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
open FSharp.Compiler.Service.Utilities

type private CompilationErrorLogger = FSharp.Compiler.SourceCodeServices.CompilationErrorLogger
type private CompilationGlobalsScope = FSharp.Compiler.SourceCodeServices.CompilationGlobalsScope

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

/// Accumulated results of type checking.
[<NoEquality; NoComparison>]
type TcAccumulator =
    { tcState: TcState
      tcImports: TcImports
      tcGlobals: TcGlobals
      tcConfig: TcConfig
      tcEnvAtEndOfFile: TcEnv

      /// Accumulated resolutions, last file first
      tcResolutionsRev: TcResolutions list

      /// Accumulated symbol uses, last file first
      tcSymbolUsesRev: TcSymbolUses list

      /// Accumulated 'open' declarations, last file first
      tcOpenDeclarationsRev: OpenDeclaration[] list

      topAttribs: TopAttribs option

      /// Result of checking most recent file, if any
      latestImplFile: TypedImplFile option

      latestCcuSigForFile: ModuleOrNamespaceType option

      tcDependencyFiles: string list

      /// Disambiguation table for module names
      tcModuleNamesDict: ModuleNamesDict

      /// Accumulated errors, last file first
      tcErrorsRev:(PhasedDiagnostic * FSharpErrorSeverity)[] list }

[<RequireQualifiedAccess>]
type CompilationResult =
    | Parsed of SyntaxTree
    | SignatureChecked of SyntaxTree * TcAccumulator // is an impl file, but only checked its signature file (.fsi)
    | Checked of TcAccumulator

[<NoEquality;NoComparison>]
type Compilation =
    {
        asyncLazyTryGetAssemblyData: AsyncLazy<IRawFSharpAssemblyData option>
        resultCache: ConcurrentDictionary<string, int * CompilationResult>

        lexResourceManager: Lexhelp.LexResourceManager
        initialTcAcc: TcAccumulator
        options: CompilationOptions
        filePaths: ImmutableArray<string>
        stamp: TimeStamp
    }

    member this.ParseFile (filePath: string) =
        if not (this.resultCache.ContainsKey filePath) then
            failwith "file does not exist in compilation"

        let tcConfig = this.initialTcAcc.tcConfig
        let isLastFile = String.Equals (this.filePaths.[this.filePaths.Length - 1], filePath, StringComparison.OrdinalIgnoreCase)

        let errorLogger = CompilationErrorLogger("ParseFile", tcConfig.errorSeverityOptions)
        let input = ParseOneInputFile (tcConfig, this.lexResourceManager, [], filePath, (isLastFile, this.options.IsExecutable), errorLogger, (*retrylocked*) true)

        {
            FilePath = filePath
            ParseResult = (input, errorLogger.GetErrors ())
        }

    member this.RunEventually input capturingErrorLogger computation =
        let maxTimeShareMilliseconds = 100L
        let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput input, capturingErrorLogger)
        // Run part of the Eventually<_> computation until a timeout is reached. If not complete, 
        // return a new Eventually<_> computation which recursively runs more of the computation.
        //   - When the whole thing is finished commit the error results sent through the errorLogger.
        //   - Each time we do real work we reinstall the CompilationGlobalsScope
        computation |> 
            Eventually.repeatedlyProgressUntilDoneOrTimeShareOverOrCanceled 
                maxTimeShareMilliseconds
                CancellationToken.None
                (fun ctok f -> 
                    // Reinstall the compilation globals each time we start or restart
                    use unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck) 
                    f ctok)

    member this.GetTcAcc (parseResult: SyntaxTree) capturingErrorLogger =
        match this.resultCache.TryGetValue parseResult.FilePath with
        | true, (0, _) -> Eventually.Done this.initialTcAcc
        | true, (i, _) ->
            match this.resultCache.TryGetValue this.filePaths.[i - 1] with
            | true, (_, CompilationResult.Parsed parseResult) -> 
                match parseResult.ParseResult with
                | Some input, _ ->
                    this.Check parseResult |> this.RunEventually input capturingErrorLogger 
                | _ ->
                    this.GetTcAcc parseResult capturingErrorLogger
            | true, (_, CompilationResult.Checked tcAcc) -> Eventually.Done tcAcc
            | _ -> failwith "file does not exist in compilation"
        | _ -> failwith "file does not exist in compilation"

    member this.Check (parseResult: SyntaxTree) =

        let inputOpt, parseErrors = parseResult.ParseResult
        let filePath = parseResult.FilePath

        let tcConfig = this.initialTcAcc.tcConfig
        let tcGlobals = this.initialTcAcc.tcGlobals
        let capturingErrorLogger = CompilationErrorLogger("Check", tcConfig.errorSeverityOptions)

        eventually {
            let! tcAcc = this.GetTcAcc parseResult capturingErrorLogger
            match inputOpt with
            | Some input ->
                let fullComputation = 
                    eventually {
                        let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput input, capturingErrorLogger)
                    
                        ApplyMetaCommandsFromInputToTcConfig (tcConfig, input, Path.GetDirectoryName filePath) |> ignore
                        let sink = TcResultsSinkImpl(tcGlobals)
                        let hadParseErrors = not (Array.isEmpty parseErrors)

                        let input, moduleNamesDict = DeduplicateParsedInputModuleName tcAcc.tcModuleNamesDict input

                        let! (tcEnvAtEndOfFile, topAttribs, implFile, ccuSigForFile), tcState = 
                            TypeCheckOneInputEventually 
                                ((fun () -> hadParseErrors || errorLogger.ErrorCount > 0), 
                                    tcConfig, tcAcc.tcImports, 
                                    tcGlobals, 
                                    None, 
                                    TcResultsSink.WithSink sink, 
                                    tcAcc.tcState, input)
                
                        /// Only keep the typed interface files when doing a "full" build for fsc.exe, otherwise just throw them away
                        let implFile = if this.Options.KeepAssemblyContents then implFile else None
                        let tcResolutions = if this.Options.KeepAllBackgroundResolutions then sink.GetResolutions() else TcResolutions.Empty
                        let tcEnvAtEndOfFile = (if this.Options.KeepAllBackgroundResolutions then tcEnvAtEndOfFile else tcState.TcEnvFromImpls)
                        let tcSymbolUses = sink.GetSymbolUses()
                    
                        let newErrors = Array.append parseErrors (capturingErrorLogger.GetErrors())
                        return {tcAcc with  tcState=tcState 
                                            tcEnvAtEndOfFile=tcEnvAtEndOfFile
                                            topAttribs=Some topAttribs
                                            latestImplFile=implFile
                                            latestCcuSigForFile=Some ccuSigForFile
                                            tcResolutionsRev=tcResolutions :: tcAcc.tcResolutionsRev
                                            tcSymbolUsesRev=tcSymbolUses :: tcAcc.tcSymbolUsesRev
                                            tcOpenDeclarationsRev = sink.GetOpenDeclarations() :: tcAcc.tcOpenDeclarationsRev
                                            tcErrorsRev = newErrors :: tcAcc.tcErrorsRev 
                                            tcModuleNamesDict = moduleNamesDict
                                            tcDependencyFiles = filePath :: tcAcc.tcDependencyFiles }
                    }

                let! newTcAcc = this.RunEventually input capturingErrorLogger fullComputation
                let i, _ = this.resultCache.[filePath]
                this.resultCache.[filePath] <- (i, CompilationResult.Checked newTcAcc)
                return newTcAcc
                                       
            | _ ->
                return tcAcc
        }


    member this.Options = this.options

    member this.TryGetAssemblyDataAsync () = this.asyncLazyTryGetAssemblyData.GetValueAsync ()

    member this.Stamp = this.stamp

    static member Create (lexResourceManager, initialTcAcc, options, parseResults, asyncLazyTryGetAssemblyData) =
        let filePaths =
            parseResults
            |> Seq.map (fun x -> x.FilePath)
            |> ImmutableArray.CreateRange

        let parseResultPairs =
            parseResults
            |> Seq.mapi (fun i parseResult -> KeyValuePair (parseResult.FilePath, (i, CompilationResult.Parsed parseResult)))

        {
            asyncLazyTryGetAssemblyData = asyncLazyTryGetAssemblyData
            resultCache = ConcurrentDictionary (parseResultPairs, StringComparer.OrdinalIgnoreCase)
            lexResourceManager = lexResourceManager
            initialTcAcc = initialTcAcc
            options = options
            filePaths = filePaths
            stamp = TimeStamp.Create ()
        }
    
type [<NoEquality;NoComparison>] CompilationInfo =
    {
        Options: CompilationOptions
        ParseResults: SyntaxTree seq
        CompilationReferences: Compilation seq
    }

type InitialInfo =
    {
        tcConfig: TcConfig
        tcConfigP: TcConfigProvider
        tcGlobals: TcGlobals
        frameworkTcImports: TcImports
        nonFrameworkResolutions: AssemblyResolution list
        unresolvedReferences: UnresolvedAssemblyReference list
        importsInvalidated: Event<string>
        assemblyName: string
        niceNameGen: NiceNameGenerator
        loadClosureOpt: LoadClosure option
        projectDirectory: string
    }

[<Sealed>]
type CompilerService (_compilationCacheSize: int, frameworkTcImportsCacheStrongSize) =
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

    let rangeStartup = Range.rangeN "startup" 1

    // Caches
    let frameworkTcImportCache = FrameworkImportsCache frameworkTcImportsCacheStrongSize   

    member __.CreateInitialState (info: InitialInfo) =
      let tcConfig = info.tcConfig
      let tcConfigP = info.tcConfigP
      let tcGlobals = info.tcGlobals
      let frameworkTcImports = info.frameworkTcImports
      let nonFrameworkResolutions = info.nonFrameworkResolutions
      let unresolvedReferences = info.unresolvedReferences
      let importsInvalidated = info.importsInvalidated
      let assemblyName = info.assemblyName
      let niceNameGen = info.niceNameGen
      let loadClosureOpt = info.loadClosureOpt
      let projectDirectory = info.projectDirectory

      cancellable {
        let errorLogger = CompilationErrorLogger("CombineImportedAssembliesTask", tcConfig.errorSeverityOptions)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter)

        let! tcImports = 
          cancellable {
            try
                let! tcImports = TcImports.BuildNonFrameworkTcImports(ctok, tcConfigP, tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolvedReferences)  
#if !NO_EXTENSIONTYPING
                tcImports.GetCcusExcludingBase() |> Seq.iter (fun ccu -> 
                    // When a CCU reports an invalidation, merge them together and just report a 
                    // general "imports invalidated". This triggers a rebuild.
                    //
                    // We are explicit about what the handler closure captures to help reason about the
                    // lifetime of captured objects, especially in case the type provider instance gets leaked
                    // or keeps itself alive mistakenly, e.g. via some global state in the type provider instance.
                    //
                    // The handler only captures
                    //    1. a weak reference to the importsInvalidated event.  
                    //
                    // The IncrementalBuilder holds the strong reference the importsInvalidated event.
                    //
                    // In the invalidation handler we use a weak reference to allow the IncrementalBuilder to 
                    // be collected if, for some reason, a TP instance is not disposed or not GC'd.
                    let capturedImportsInvalidated = WeakReference<_>(importsInvalidated)
                    ccu.Deref.InvalidateEvent.Add(fun msg -> 
                        match capturedImportsInvalidated.TryGetTarget() with 
                        | true, tg -> tg.Trigger msg
                        | _ -> ()))
#endif

                return tcImports
            with e -> 
                System.Diagnostics.Debug.Assert(false, sprintf "Could not BuildAllReferencedDllTcImports %A" e)
                errorLogger.Warning e
                return frameworkTcImports           
          }

        let tcInitial = GetInitialTcEnv (assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)
        let tcState = GetInitialTcState (rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, niceNameGen, tcInitial)
        let loadClosureErrors = 
           [ match loadClosureOpt with 
             | None -> ()
             | Some loadClosure -> 
                for inp in loadClosure.Inputs do
                    for (err, isError) in inp.MetaCommandDiagnostics do 
                        yield err, (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning) ]

        let initialErrors = Array.append (Array.ofList loadClosureErrors) (errorLogger.GetErrors())

        let basicDependencies = 
            [ for (UnresolvedAssemblyReference(referenceText, _))  in unresolvedReferences do
                // Exclude things that are definitely not a file name
                if not(FileSystem.IsInvalidPathShim referenceText) then 
                    let file = if FileSystem.IsPathRootedShim referenceText then referenceText else Path.Combine(projectDirectory, referenceText) 
                    yield file 

              for r in nonFrameworkResolutions do 
                    yield  r.resolvedPath  ]

        let tcAcc = 
            { tcGlobals=tcGlobals
              tcImports=tcImports
              tcState=tcState
              tcConfig=tcConfig
              tcEnvAtEndOfFile=tcInitial
              tcResolutionsRev=[]
              tcSymbolUsesRev=[]
              tcOpenDeclarationsRev=[]
              topAttribs=None
              latestImplFile=None
              latestCcuSigForFile=None
              tcDependencyFiles=basicDependencies
              tcErrorsRev = [ initialErrors ] 
              tcModuleNamesDict = Map.empty }
        return tcAcc
        }

    member this.TryCreateCompilationAsync info =
        cancellable {
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
                            return compilation.TryGetAssemblyDataAsync () |> Async.RunSynchronously
                          }

                      member __.FileName = compilation.Options.AssemblyPath

                      member __.TryGetLogicalTimeStamp (_, _) =
                          Some compilation.Stamp.DateTime                              
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

                  let sourceFiles =
                    info.ParseResults
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
              let! (tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolvedReferences) = frameworkTcImportCache.Get(ctok, tcConfig)

              // Note we are not calling errorLogger.GetErrors() anywhere for this task. 
              // This is ok because not much can actually go wrong here.
              let errorOptions = tcConfig.errorSeverityOptions
              let errorLogger = CompilationErrorLogger("nonFrameworkAssemblyInputs", errorOptions)
              // Return the disposable object that cleans up
              use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter)

              let initialInfo =
                {
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
                }

              let! initialTcAcc = this.CreateInitialState initialInfo
      
              
              return Some (Compilation.Create (lexResourceManager, initialTcAcc, info.Options, info.ParseResults, AsyncLazy (async { return None })))
            with e -> 
              errorRecoveryNoRange e
              return None
           }

          return builderOpt
        } |> Cancellable.toAsync
