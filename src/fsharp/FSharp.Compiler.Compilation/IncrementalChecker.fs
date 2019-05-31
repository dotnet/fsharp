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
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Tastops

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

    // TODO: This should be moved out of the checker (possibly to Compilation), but we have a dependency on tcConfig; which gets built as part of the checker.
    //       The checker should not be thinking about source snapshots and only be thinking about consuming syntax trees.
    /// Create a syntax tree.
    static member private CreateSyntaxTree (tcConfig, parsingOptions, isLastFileOrScript, sourceSnapshot: SourceSnapshot) =
        let filePath = sourceSnapshot.FilePath

        let pConfig =
            {
                tcConfig = tcConfig
                isLastFileOrScript = isLastFileOrScript
                isExecutable = parsingOptions.isExecutable
                conditionalCompilationDefines = []
                filePath = filePath
            }

        SyntaxTree (filePath, pConfig, sourceSnapshot)

    static member Create (tcConfig, tcGlobals, tcImports, initialTcAcc, options, orderedSourceSnapshots: ImmutableArray<SourceSnapshot>) =
        cancellable {
            let! cancellationToken = Cancellable.token ()
            let length = orderedSourceSnapshots.Length

            let orderedResultsBuilder = ImmutableArray.CreateBuilder length
            let indexLookup = Array.zeroCreate length

            orderedResultsBuilder.Count <- length

            orderedSourceSnapshots
            |> ImmutableArray.iteri (fun i sourceSnapshot ->
                let isLastFile = (orderedSourceSnapshots.Length - 1) = i
                let syntaxTree = IncrementalCheckerState.CreateSyntaxTree (tcConfig, options.parsingOptions, isLastFile, sourceSnapshot)
                let parseResult = Async.RunSynchronously (syntaxTree.GetParseResultAsync (), cancellationToken = cancellationToken)
                orderedResultsBuilder.[i] <- Parsed (syntaxTree, parseResult)
                indexLookup.[i] <- KeyValuePair (syntaxTree.FilePath, i)
            )

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

    member this.GetSyntaxTree filePath =
        match this.indexLookup.TryGetValue filePath with
        | true, i -> this.orderedResults.[i].SyntaxTree
        | _ -> failwith "file for syntax tree does not exist in incremental checker"

    member this.CheckAsync (filePath: string, flags: CheckFlags) =
        let tcConfig = this.tcConfig
        let tcGlobals = this.tcGlobals
        let tcImports = this.tcImports
        let options = this.options

        async {
            let! ct = Async.CancellationToken

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
                        let sink = TcResultsSinkImpl tcGlobals
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

                        this.SetPartialCheckResultByIndex (cacheIndex, PartialCheckResult.Checked (syntaxTree, tcAcc))

                        return {tcAcc with  tcState=tcState 
                                            tcEnvAtEndOfFile=tcEnvAtEndOfFile
                                            topAttribs=Some topAttribs
                                            latestImplFile=implFile
                                            latestCcuSigForFile=Some ccuSigForFile
                                            tcErrorsRev = newErrors :: tcAcc.tcErrorsRev 
                                            tcModuleNamesDict = moduleNamesDict }, (Some (sink, symbolEnv))
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
                                ct
                                (fun ctok f -> f ctok)

                let asyncTimeSlicedComputation =
                    timeSlicedComputation
                    |> Eventually.forceAsync (fun eventuallyWork ->
                        CompilationWorker.EnqueueAndAwaitAsync (fun ctok ->
                            // Reinstall the compilation globals each time we start or restart
                            use _unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck) 
                            eventuallyWork ctok
                        )
                    )
                    
                match! asyncTimeSlicedComputation with
                | Some result -> return result
                | _ -> return raise (OperationCanceledException ())
            | _ ->
                return (tcAcc, None)
        }

[<Sealed>]
type IncrementalChecker (tcInitial: TcInitial, state: IncrementalCheckerState) =

    let getTcAccs () =
        state.orderedResults 
        |> Array.map (function
            | PartialCheckResult.Checked (_, tcAcc) -> tcAcc
            | _ -> failwith "should not happen, missing a checked file"
        )

    member __.ReplaceSourceSnapshot sourceSnapshot =
        IncrementalChecker (tcInitial, state.ReplaceSourceSnapshot sourceSnapshot)

    member __.CheckAsync filePath =
        async {
            let! tcAcc, info = state.CheckAsync (filePath, CheckFlags.None)
            match info with
            | None -> return raise (InvalidOperationException ())
            | Some (sink, symbolEnv) -> return (tcAcc, sink, symbolEnv)
        }

    member __.GetSyntaxTree filePath =
        state.GetSyntaxTree filePath

    member __.TcInitial = tcInitial

    member __.TcGlobals = state.tcGlobals

    member __.TcImports = state.tcImports

    member this.FinishAsync () =
        match state.orderedResults.[state.orderedResults.Length - 1] with
        | PartialCheckResult.Checked _ ->
            async { return getTcAccs () }
        | result -> 
            async {
                let! _ = this.CheckAsync (result.SyntaxTree.FilePath)
                return getTcAccs ()
            }           

module IncrementalChecker =

    let create (tcInitial: TcInitial) tcGlobals tcImports tcAcc (checkerOptions: CheckerOptions) sourceSnapshots =
        cancellable {
            let! state = IncrementalCheckerState.Create (tcInitial.tcConfig, tcGlobals, tcImports, tcAcc, checkerOptions, sourceSnapshots)
            return IncrementalChecker (tcInitial, state)
        }
