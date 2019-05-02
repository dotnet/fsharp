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

type internal CheckerParsingOptions =
    {
        isExecutable: bool
    }

type internal CheckerOptions =
    {
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
        parsingOptions: CheckerParsingOptions
    }

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

type IncrementalCheckerState =
    {
        tcConfig: TcConfig
        parsingOptions: CheckerParsingOptions
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

type CheckerFlags =
    | None = 0x00
    | ReturnResolutions = 0x01 
    | Recheck = 0x1

[<Sealed>]
type IncrementalChecker (tcConfig: TcConfig, tcGlobals: TcGlobals, tcImports: TcImports, initialTcAcc: TcAccumulator, options: CheckerOptions, state: IncrementalCheckerState) =

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
                                CancellationToken.None
                                (fun ctok f -> f ctok)

                let timeSlicedComputationAsync =
                    timeSlicedComputation
                    |> Eventually.forceAsync (fun eventuallyWork ->
                        CompilationWorker.EnqueueAndAwaitAsync (fun ctok ->
                            // Reinstall the compilation globals each time we start or restart
                            use _unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck) 
                            eventuallyWork ctok
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

module IncrementalChecker =

    let create tcConfig tcGlobals tcImports tcAcc (checkerOptions: CheckerOptions) sourceSnapshots =
        cancellable {
            let! state = IncrementalCheckerState.Create (tcConfig, checkerOptions.parsingOptions, sourceSnapshots)
            return IncrementalChecker (tcConfig, tcGlobals, tcImports, tcAcc, checkerOptions, state)
        }
