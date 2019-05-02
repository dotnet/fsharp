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

/// Accumulated results of type checking.
[<NoEquality; NoComparison>]
type TcAccumulator =
    { tcState: TcState
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
   // | NotParsed of Source
    | Parsed of SyntaxTree * ParseResult
    | CheckingInProgress of SyntaxTree * CancellationTokenSource
   // | SignatureChecked of SyntaxTree * TcAccumulator // is an impl file, but only checked its signature file (.fsi)
    | Checked of SyntaxTree * TcAccumulator

    member this.SyntaxTree =
        match this with
        | CompilationResult.Parsed (syntaxTree, _) -> syntaxTree
        | CompilationResult.CheckingInProgress (syntaxTree, _) -> syntaxTree
        | CompilationResult.Checked (syntaxTree, _) -> syntaxTree

type ParsingOptions =
    {
        isExecutable: bool
        lexResourceManager: Lexhelp.LexResourceManager
    }

type IncrementalCheckerState =
    {
        tcConfig: TcConfig
        parsingOptions: ParsingOptions
        orderedResults: ImmutableArray<CompilationResult ref>
        indexLookup: ImmutableDictionary<string, int>
        version: VersionStamp
    }

    /// Create a syntax tree.
    static member CreateSyntaxTree (tcConfig, parsingOptions, isLastFileOrScript, sourceSnapshot: SourceSnapshot) =
        let filePath = sourceSnapshot.FilePath

        let parsingInfo =
            {
                tcConfig = tcConfig
                isLastFileOrScript = isLastFileOrScript
                isExecutable = parsingOptions.isExecutable
                conditionalCompilationDefines = []
                filePath = filePath
            }

        let asyncLazyWeakGetParseResult =
            AsyncLazyWeak (async {
                use! sourceValue = sourceSnapshot.GetSourceValueAsync ()
                return Parser.Parse parsingInfo sourceValue
            })

        SyntaxTree (filePath, parsingInfo, asyncLazyWeakGetParseResult)

    static member Create (tcConfig, parsingOptions, orderedSourceSnapshots: ImmutableArray<SourceSnapshot>) =
        cancellable {
            let! cancellationToken = Cancellable.token ()
            let length = orderedSourceSnapshots.Length

            let orderedResultsBuilder = ImmutableArray.CreateBuilder length
            let indexLookup = Array.zeroCreate length

            orderedResultsBuilder.Count <- length

            // Build syntax trees.
            let syntaxTrees = ResizeArray length

            try
                orderedSourceSnapshots
                |> ImmutableArray.iteri (fun i sourceSnapshot ->
                    let isLastFile = (orderedSourceSnapshots.Length - 1) = i
                    let syntaxTree = IncrementalCheckerState.CreateSyntaxTree (tcConfig, parsingOptions, isLastFile, sourceSnapshot)
                    let sourceValue = Async.RunSynchronously (sourceSnapshot.GetSourceValueAsync (), cancellationToken = cancellationToken)
                    syntaxTrees.Add (syntaxTree, sourceValue)
                )

                // We parallelize parsing here because reading from source snapshots are not truly asynchronous; memory mapped files are blocking, non-asynchronous calls.
                orderedSourceSnapshots |> Seq.iteri (fun i _ ->
               // Parallel.For(0, orderedSourceSnapshots.Length, fun i ->
                    let syntaxTree, sourceValue = syntaxTrees.[i]
                    let parseResult = Parser.Parse syntaxTree.ParsingInfo sourceValue
                    let compilationResult = CompilationResult.Parsed (syntaxTree, parseResult)

                    orderedResultsBuilder.[i] <- ref compilationResult
                    indexLookup.[i] <- KeyValuePair (syntaxTree.FilePath, i)
                ) |> ignore

            finally
                syntaxTrees
                |> Seq.iter (fun (_, sourceValue) -> 
                    (sourceValue :> IDisposable).Dispose ()
                )

                cancellationToken.ThrowIfCancellationRequested ()

            return {
                tcConfig = tcConfig
                parsingOptions = parsingOptions
                orderedResults = orderedResultsBuilder.ToImmutableArray ()
                indexLookup = ImmutableDictionary.CreateRange indexLookup
                version = VersionStamp.Create ()
            }
        }

    member this.GetIndex (filePath: string) =
        match this.indexLookup.TryGetValue filePath with
        | false, _ -> failwith "source does not exist in incremental checker"
        | true, index -> index

    member this.GetCompilationResult (filePath: string) =
        this.orderedResults.[this.GetIndex filePath]

    member this.GetIndexAndCompilationResult (filePath: string) =
        let index = this.GetIndex filePath
        (index, this.orderedResults.[index])

    member this.GetCompilationResultByIndex index =
        this.orderedResults.[index]

    member this.GetParseResult (filePath: string, cancellationToken) =
        match this.GetCompilationResult(filePath).contents with
        | CompilationResult.Parsed (_, parseResult) -> parseResult
        | CompilationResult.CheckingInProgress (syntaxTree, _) -> Async.RunSynchronously (syntaxTree.GetParseResultAsync (), cancellationToken = cancellationToken)
        | CompilationResult.Checked (syntaxTree, _) -> Async.RunSynchronously (syntaxTree.GetParseResultAsync (), cancellationToken = cancellationToken)

    //member this.ReplaceSourceSnapshot (sourceSnapshot: SourceSnapshot) =
    //    match this.indexLookup.TryGetValue sourceSnapshot.FilePath with
    //    | false, _ -> failwith "syntax tree does not exist in incremental checker"
    //    | true, (i) ->
    //        let orderedResultsBuilder = ImmutableArray.Crea
    //        let mutable resultCache = this.resultCache//this.resultCache.SetItem(source.FilePath, (i, ref (CompilationResult.Parsed source)))

    //        //for i = i + 1 to this.orderedFilePaths.Length - 1 do
    //        //    let filePath = this.orderedFilePaths.[i]
    //        //    match this.resultCache.TryGetValue filePath with
    //        //    | false, _ -> failwith "should not happen"
    //        //    | true, (i, refResult) ->
    //        //        let syntaxTree =
    //        //            match refResult.contents with
    //        //            | CompilationResult.Parsed syntaxTree -> syntaxTree
    //        //            | CompilationResult.Checked (syntaxTree, _) -> syntaxTree
    //        //        resultCache <- resultCache.SetItem(syntaxTree.FilePath, (i, ref (CompilationResult.Parsed syntaxTree)))

    //        { this with
    //            resultCache = resultCache
    //            version = this.version.NewVersionStamp ()
    //        }

type IncrementalCheckerOptions =
    {
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
        parsingOptions: ParsingOptions
    }

type CheckerFlags =
    | None = 0x00
    | ReturnResolutions = 0x01 
    | Recheck = 0x1

[<Sealed>]
type IncrementalChecker (tcConfig: TcConfig, tcGlobals: TcGlobals, tcImports: TcImports, initialTcAcc: TcAccumulator, options: IncrementalCheckerOptions, state: IncrementalCheckerState) =

    let gate = obj ()
    let maxTimeShareMilliseconds = 100L

    let cacheResults = Array.zeroCreate<(AsyncLazyWeak<TcAccumulator * TcResolutions option> * CheckerFlags) voption> state.orderedResults.Length

    member __.Version = state.version

    member __.ReplaceSourceSnapshot (_sourceSnapshot: SourceSnapshot) =
        let newState = state
        IncrementalChecker (tcConfig, tcGlobals, tcImports, initialTcAcc, options, newState)

    member this.GetTcAcc (filePath: string) =
        async {
            match state.GetIndexAndCompilationResult filePath with
            | 0, cacheResult -> 
                printfn "initial tcacc"
                return (initialTcAcc, cacheResult)

            | (i, cacheResult) ->
                let priorCacheResult = state.GetCompilationResultByIndex (i - 1)
                match priorCacheResult.contents with
                | CompilationResult.Parsed (syntaxTree, _) ->
                    // We set no checker flags as we don't want to ask for extra information when checking a dependent file.
                    let! tcAcc, _ = this.CheckAsyncLazy (syntaxTree.FilePath, CheckerFlags.None)
                    return (tcAcc, cacheResult)
                | CompilationResult.CheckingInProgress (_, _) -> return failwith "not yet"
                | CompilationResult.Checked (_, tcAcc) ->
                    return (tcAcc, cacheResult)
        }

    member this.CheckAsync (filePath: string, flags: CheckerFlags) =
        async {
            printfn "Checking %s" filePath
            let! cancellationToken = Async.CancellationToken

            let! (tcAcc, cacheResult) = this.GetTcAcc (filePath)
            let syntaxTree = cacheResult.contents.SyntaxTree
            let (inputOpt, parseErrors) = Async.RunSynchronously (syntaxTree.GetParseResultAsync (), cancellationToken = cancellationToken)
            match inputOpt with
            | Some input ->
                let capturingErrorLogger = CompilationErrorLogger("Check", tcConfig.errorSeverityOptions)
                let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput input, capturingErrorLogger)

                let fullComputation = 
                    eventually {                    
                        ApplyMetaCommandsFromInputToTcConfig (tcConfig, input, Path.GetDirectoryName filePath) |> ignore
                        let sink = TcResultsSinkImpl(tcGlobals)
                        let hadParseErrors = not (Array.isEmpty parseErrors)

                        let input, moduleNamesDict = DeduplicateParsedInputModuleName tcAcc.tcModuleNamesDict input

                        let! (tcEnvAtEndOfFile, topAttribs, implFile, ccuSigForFile), tcState = 
                            TypeCheckOneInputEventually 
                                ((fun () -> hadParseErrors || errorLogger.ErrorCount > 0), 
                                    tcConfig, tcImports, 
                                    tcGlobals, 
                                    None, 
                                    TcResultsSink.WithSink sink, 
                                    tcAcc.tcState, input)
                
                        /// Only keep the typed interface files when doing a "full" build for fsc.exe, otherwise just throw them away
                        let implFile = if options.keepAssemblyContents then implFile else None
                        let tcResolutions = if options.keepAllBackgroundResolutions then sink.GetResolutions() else TcResolutions.Empty
                        let tcEnvAtEndOfFile = (if options.keepAllBackgroundResolutions then tcEnvAtEndOfFile else tcState.TcEnvFromImpls)
                        let tcSymbolUses = sink.GetSymbolUses()

                        let tcResolutionsOpt =
                            if options.keepAllBackgroundResolutions then Some tcResolutions
                            elif (flags &&& CheckerFlags.ReturnResolutions = CheckerFlags.ReturnResolutions) then
                                Some (sink.GetResolutions ())
                            else
                                None
                                
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
                                            tcDependencyFiles = filePath :: tcAcc.tcDependencyFiles }, tcResolutionsOpt
                    }

                // Run part of the Eventually<_> computation until a timeout is reached. If not complete, 
                // return a new Eventually<_> computation which recursively runs more of the computation.
                //   - When the whole thing is finished commit the error results sent through the errorLogger.
                //   - Each time we do real work we reinstall the CompilationGlobalsScope
                let timeSlicedComputation = 
                        fullComputation |> 
                            Eventually.repeatedlyProgressUntilDoneOrTimeShareOverOrCanceled 
                                maxTimeShareMilliseconds
                                cancellationToken
                                (fun ctok f -> f ctok)

                let timeSlicedComputationAsync =
                    timeSlicedComputation
                    |> Eventually.forceAsync (fun eventuallyWork ->
                        CompilationWorker.EnqueueAndAwaitAsync (fun ctok ->
                            // Reinstall the compilation globals each time we start or restart
                            use _unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck) 
                            async { 
                                return (eventuallyWork ctok) 
                            }
                        )
                    )

                match! timeSlicedComputationAsync with
                | Some (tcAcc, tcResolutionsOpt) -> 
                    cacheResult := CompilationResult.Checked (syntaxTree, tcAcc)
                    return (tcAcc, tcResolutionsOpt)
                | _ ->
                    return failwith "computation failed"
                                       
            | _ ->
                return (tcAcc, None)
        }

        member this.CheckAsyncLazy (filePath: string, checkerFlags) =
            async {
                let i = state.GetIndex filePath

                lock gate <| fun () ->
                    match cacheResults.[i] with
                    | ValueSome (current, currentCheckerFlags) when (checkerFlags &&& CheckerFlags.Recheck = CheckerFlags.Recheck) && not (checkerFlags = currentCheckerFlags) -> 
                        current.CancelIfNotComplete ()
                        cacheResults.[i] <- ValueSome (AsyncLazyWeak (this.CheckAsync (filePath, checkerFlags)), checkerFlags)
                    | ValueNone ->
                        cacheResults.[i] <- ValueSome (AsyncLazyWeak (this.CheckAsync (filePath, checkerFlags)), checkerFlags)
                    | _ -> ()

                return! (fst cacheResults.[i].Value).GetValueAsync ()
            }

        member this.CheckAsync filePath =
            this.CheckAsyncLazy (filePath, CheckerFlags.ReturnResolutions ||| CheckerFlags.Recheck)

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
        checkerOptions: IncrementalCheckerOptions
        sourceSnapshots: ImmutableArray<SourceSnapshot>
    }

module IncrementalChecker =

    let rangeStartup = FSharp.Compiler.Range.rangeN "startup" 1

    let create (info: InitialInfo) ctok =
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

        let! checkerState = IncrementalCheckerState.Create (tcConfig, info.checkerOptions.parsingOptions, info.sourceSnapshots)

        let tcAcc = 
            { tcState=tcState
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
        return IncrementalChecker (tcConfig, tcGlobals, tcImports, tcAcc, info.checkerOptions, checkerState)
        }
