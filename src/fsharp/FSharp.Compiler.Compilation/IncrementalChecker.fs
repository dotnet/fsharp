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

type CheckFlags =
    | None = 0x00

type PartialCheckResult =
    | NotParsed of SyntaxTree
    | Parsed of SyntaxTree * ParseResult
    /// Is an impl file, but only checked its signature file (.fsi). This is a performance optimization.
   // | SignatureChecked of SyntaxTree * TcAccumulator
    | Checked of SyntaxTree * TcAccumulator

    member this.SyntaxTree =
        match this with
        | PartialCheckResult.NotParsed syntaxTree -> syntaxTree
        | PartialCheckResult.Parsed (syntaxTree, _) -> syntaxTree
        | PartialCheckResult.Checked (syntaxTree, _) -> syntaxTree

[<NoEquality;NoComparison>]
type IncrementalCheckerState =
    {
        tcConfig: TcConfig
        tcGlobals: TcGlobals
        tcImports: TcImports
        initialTcAcc: TcAccumulator
        options: CheckerOptions
        /// Mutable item, used for caching results. Gets copied when new state gets built.
        orderedResults: PartialCheckResult []
        indexLookup: ImmutableDictionary<string, int>
    }

    // TODO: This should be moved out of the checker (possibly to Compilation), but we have a dependency on tcConfig; which gets built and passed to the checker.
    //       The checker should not be thinking about source snapshots and only be thinking about consuming syntax trees.
    /// Create a syntax tree.
    static member private CreateSyntaxTree (tcConfig, parsingOptions, isLastFileOrScript, sourceSnapshot: SourceSnapshot) =
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

    static member Create (tcConfig, tcGlobals, tcImports, initialTcAcc, options, orderedSourceSnapshots: ImmutableArray<SourceSnapshot>) =
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
                    let syntaxTree = IncrementalCheckerState.CreateSyntaxTree (tcConfig, options.parsingOptions, isLastFile, sourceSnapshot)
                    let sourceValue = Async.RunSynchronously (sourceSnapshot.GetSourceValueAsync (), cancellationToken = cancellationToken)
                    syntaxTrees.Add (syntaxTree, sourceValue)
                )

                // We parallelize parsing here because reading from source snapshots are not truly asynchronous; memory mapped files are blocking, non-asynchronous calls.
                orderedSourceSnapshots |> Seq.iteri (fun i _ ->
               // Parallel.For(0, orderedSourceSnapshots.Length, fun i ->
                    let syntaxTree, sourceValue = syntaxTrees.[i]
                    let parseResult = Parser.Parse syntaxTree.ParsingInfo sourceValue
                    let compilationResult = PartialCheckResult.Parsed (syntaxTree, parseResult)

                    orderedResultsBuilder.[i] <- compilationResult
                    indexLookup.[i] <- KeyValuePair (syntaxTree.FilePath, i)
                ) |> ignore

            finally
                syntaxTrees
                |> Seq.iter (fun (_, sourceValue) -> 
                    // TODO: We should moving disposing of a stream in the Parser.
                    (sourceValue :> IDisposable).Dispose ()
                )

                cancellationToken.ThrowIfCancellationRequested ()

            return {
                tcConfig = tcConfig
                tcGlobals = tcGlobals
                tcImports = tcImports
                initialTcAcc = initialTcAcc
                options = options
                orderedResults = orderedResultsBuilder.ToArray ()
                indexLookup = ImmutableDictionary.CreateRange indexLookup
            }
        }

    member private this.GetIndex (filePath: string) =
        match this.indexLookup.TryGetValue filePath with
        | false, _ -> failwith "source does not exist in incremental checker"
        | true, index -> index

    member private this.GetPartialCheckResultByIndex index =
        this.orderedResults.[index]

    member private this.SetPartialCheckResultByIndex (index, result) =
        this.orderedResults.[index] <- result

    member private this.GetPriorTcAccumulatorAsync (filePath: string) =
        async {
            match this.GetIndex filePath with
            | 0 -> return (this.initialTcAcc, 0) // first file

            | cacheIndex ->
                let priorCacheResult = this.GetPartialCheckResultByIndex (cacheIndex - 1)
                match priorCacheResult with
                | PartialCheckResult.NotParsed syntaxTree
                | PartialCheckResult.Parsed (syntaxTree, _) ->
                    // We set no checker flags as we don't want to ask for extra information when checking a dependent file.
                    let! tcAcc, _ = this.CheckAsync (syntaxTree.FilePath, CheckFlags.None)
                    return (tcAcc, cacheIndex)
                | PartialCheckResult.Checked (_, tcAcc) ->
                    return (tcAcc, cacheIndex)
        }

    // TODO: While we keep results above the source snapshot, we don't keep an inprocess *running* result. 
    //       This way we don't try to restart already computing results that would be valid on the next state.
    // TODO: The above TODO applies, but should be changed to a syntax tree.
    member this.ReplaceSourceSnapshot (sourceSnapshot: SourceSnapshot) =
        let index = this.GetIndex sourceSnapshot.FilePath
        let orderedResults =
            this.orderedResults
            |> Array.mapi (fun i result ->
                // invalidate compilation results of the source and all sources below it.
                if i = index then
                    let isLastFile = (this.orderedResults.Length - 1) = i
                    let syntaxTree = IncrementalCheckerState.CreateSyntaxTree (this.tcConfig, this.options.parsingOptions, isLastFile, sourceSnapshot)
                    PartialCheckResult.NotParsed syntaxTree
                elif i > index then
                    PartialCheckResult.NotParsed result.SyntaxTree
                else
                    result
            )
        { this with orderedResults = orderedResults }

    member this.CheckAsync (filePath: string, flags: CheckFlags) =
        let tcConfig = this.tcConfig
        let tcGlobals = this.tcGlobals
        let tcImports = this.tcImports
        let options = this.options

        async {
            let! cancellationToken = Async.CancellationToken

            let! (tcAcc, cacheIndex) = this.GetPriorTcAccumulatorAsync (filePath)
            let syntaxTree = (this.GetPartialCheckResultByIndex cacheIndex).SyntaxTree
            let! (inputOpt, parseErrors) = syntaxTree.GetParseResultAsync ()
            match inputOpt with
            | Some input ->
                let capturingErrorLogger = CompilationErrorLogger("CheckAsync", tcConfig.errorSeverityOptions)
                let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput input, capturingErrorLogger)

                let fullComputation = 
                    eventually {                    
                        ApplyMetaCommandsFromInputToTcConfig (tcConfig, input, Path.GetDirectoryName filePath) |> ignore
                        let sink = CheckerSink tcGlobals
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
                        let tcEnvAtEndOfFile = (if options.keepAllBackgroundResolutions then tcEnvAtEndOfFile else tcState.TcEnvFromImpls)

                        let symbolEnv = SymbolEnv (tcGlobals, tcState.Ccu, Some ccuSigForFile, tcImports)
                                
                        let newErrors = Array.append parseErrors (capturingErrorLogger.GetErrors())
                        return {tcAcc with  tcState=tcState 
                                            tcEnvAtEndOfFile=tcEnvAtEndOfFile
                                            topAttribs=Some topAttribs
                                            latestImplFile=implFile
                                            latestCcuSigForFile=Some ccuSigForFile
                                            tcErrorsRev = newErrors :: tcAcc.tcErrorsRev 
                                            tcModuleNamesDict = moduleNamesDict
                                            tcDependencyFiles = filePath :: tcAcc.tcDependencyFiles }, Some (sink, symbolEnv)
                    }

                // No one has ever changed this value, although a bit arbitrary.
                // At some point the `eventually { ... }` constructs will go away once we have a thread safe compiler.
                let maxTimeShareMilliseconds = 100L
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
                            eventuallyWork ctok
                        )
                    )

                match! timeSlicedComputationAsync with
                | Some (tcAcc, Some checkerSink) -> 
                    this.SetPartialCheckResultByIndex (cacheIndex, PartialCheckResult.Checked (syntaxTree, tcAcc))
                    return (tcAcc, Some checkerSink)
                | _ ->
                    return failwith "computation failed"
                                       
            | _ ->
                return (tcAcc, None)
        }

[<Sealed>]
type IncrementalChecker (state: IncrementalCheckerState) =

    // TODO: Should be a syntax tree.
    member __.ReplaceSourceSnapshot sourceSnapshot =
        IncrementalChecker (state.ReplaceSourceSnapshot sourceSnapshot)

    member __.CheckAsync filePath =
        state.CheckAsync (filePath, CheckFlags.None)

module IncrementalChecker =

    let create tcConfig tcGlobals tcImports tcAcc (checkerOptions: CheckerOptions) sourceSnapshots =
        cancellable {
            let! state = IncrementalCheckerState.Create (tcConfig, tcGlobals, tcImports, tcAcc, checkerOptions, sourceSnapshots)
            return IncrementalChecker state
        }
