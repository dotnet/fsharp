module FSharp.Compiler.Compilation.IncrementalChecker

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
        isScript: bool
    }

type internal CheckerOptions =
    {
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
        parsingOptions: CheckerParsingOptions
    }

[<NoEquality;NoComparison>]
type PreEmitState =
    {
        finalAcc: TcAccumulator
        tcEnvAtEndOfLastFile: TcEnv
        topAttrs: TopAttribs
        implFiles: TypedImplFile list
        tcState: TcState
    }

    member x.ImplFiles = x.implFiles

    member x.TypeCheckErrors = x.finalAcc.tcErrorsRev |> List.last

    member x.FinalTcAcc = x.finalAcc

type PartialCheckResult =
    | NotParsed of FSharpSyntaxTree
    | Parsed of FSharpSyntaxTree * ParseResult
    /// Is an impl file, but only checked its signature file (.fsi). This is a performance optimization.
   // | SignatureChecked of SyntaxTree * TcAccumulator
    | Checked of FSharpSyntaxTree * TcAccumulator

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
        indexLookup: ImmutableDictionary<FSharpSource, int>
    }

    // TODO: This should be moved out of the checker (possibly to Compilation), but we have a dependency on tcConfig; which gets built as part of the checker.
    //       The checker should not be thinking about sources and only be thinking about consuming syntax trees.
    /// Create a syntax tree.
    static member private CreateSyntaxTree (tcConfig, parsingOptions, isLastFileOrScript, src: FSharpSource) =
        let pConfig =
            {
                tcConfig = tcConfig
                isLastFileOrScript = isLastFileOrScript
                isExecutable = parsingOptions.isExecutable
                conditionalCompilationDefines = []
                filePath = src.FilePath
                supportsFeature = tcConfig.langVersion.SupportsFeature
            }

        FSharpSyntaxTree.Create (pConfig, src)

    static member Create (tcConfig, tcGlobals, tcImports, initialTcAcc, options, orderedSources: ImmutableArray<FSharpSource>) =
        cancellable {
            let isScript = options.parsingOptions.isScript
            let length = orderedSources.Length

            let orderedResultsBuilder = ImmutableArray.CreateBuilder length
            let indexLookup = Array.zeroCreate length

            orderedResultsBuilder.Count <- length

            orderedSources
            |> ImmutableArray.iteri (fun i src ->
                let isLastFile = (orderedSources.Length - 1) = i
                let syntaxTree = IncrementalCheckerState.CreateSyntaxTree (tcConfig, options.parsingOptions, isScript || isLastFile, src)
                orderedResultsBuilder.[i] <- NotParsed syntaxTree
                indexLookup.[i] <- KeyValuePair (src, i)
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

    member private this.GetIndex src =
        match this.indexLookup.TryGetValue src with
        | false, _ -> failwith "source does not exist in incremental checker"
        | true, index -> index

    member private this.GetPartialCheckResultByIndex index =
        this.orderedResults.[index]

    member private this.SetPartialCheckResultByIndex (index, result) =
        this.orderedResults.[index] <- result

    member private this.GetPriorTcAccumulatorAsync (src: FSharpSource) =
        async {
            match this.GetIndex src with
            | 0 -> return (this.initialTcAcc, 0) // first file

            | cacheIndex ->
                let priorCacheResult = this.GetPartialCheckResultByIndex (cacheIndex - 1)
                match priorCacheResult with
                | PartialCheckResult.NotParsed syntaxTree
                | PartialCheckResult.Parsed (syntaxTree, _) ->
                    // We set no checker flags as we don't want to ask for extra information when checking a dependent file.
                    let! tcAcc, _, _ = this.CheckAsync syntaxTree.Source
                    return (tcAcc, cacheIndex)
                | PartialCheckResult.Checked (_, tcAcc) ->
                    return (tcAcc, cacheIndex)
        }

    member this.ReplaceSource (oldSrc, newSrc: FSharpSource) =
        let index = this.GetIndex oldSrc
        let orderedResults =
            this.orderedResults
            |> Array.mapi (fun i result ->
                // invalidate compilation results of the source and all sources below it.
                if i = index then
                    let syntaxTree = result.SyntaxTree.WithChangedSource newSrc
                    PartialCheckResult.NotParsed syntaxTree
                elif i > index then
                    PartialCheckResult.NotParsed result.SyntaxTree
                else
                    result
            )
        { this with orderedResults = orderedResults }

    member this.SubmitSource (src: FSharpSource, preEmitState) =
        let initialTcAcc =
            { this.initialTcAcc with
                tcState = preEmitState.tcState }
        IncrementalCheckerState.Create (this.tcConfig, this.tcGlobals, this.tcImports, initialTcAcc, this.options, ImmutableArray.Create src)
        |> Cancellable.runWithoutCancellation

    member this.GetSyntaxTree filePath =
        match this.indexLookup.TryGetValue filePath with
        | true, i -> this.orderedResults.[i].SyntaxTree
        | _ -> failwith "file for syntax tree does not exist in incremental checker"

    member this.CheckAsync (src: FSharpSource) : Async<(TcAccumulator * TcResultsSinkImpl * SymbolEnv)> =
        let tcConfig = this.tcConfig
        let tcGlobals = this.tcGlobals
        let tcImports = this.tcImports
        let options = this.options

        async {
            let! ct = Async.CancellationToken

            let! (tcAcc, cacheIndex) = this.GetPriorTcAccumulatorAsync src
            let syntaxTree = (this.GetPartialCheckResultByIndex cacheIndex).SyntaxTree
            let (inputOpt, parseErrors) = syntaxTree.GetParseResult ct
            match inputOpt with
            | Some input ->
                let capturingErrorLogger = CompilationErrorLogger("CheckAsync", tcConfig.errorSeverityOptions)
                let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput input, capturingErrorLogger)

                let fullComputation = 
                    eventually {                 
                        if not (String.IsNullOrWhiteSpace src.FilePath) then
                            ApplyMetaCommandsFromInputToTcConfig (tcConfig, input, Path.GetDirectoryName src.FilePath) |> ignore

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
                                
                        let newErrors = capturingErrorLogger.GetErrorInfos ()

                        return {tcAcc with  tcState=tcState 
                                            tcEnvAtEndOfFile=tcEnvAtEndOfFile
                                            topAttribs=Some topAttribs
                                            latestImplFile=implFile
                                            latestCcuSigForFile=Some ccuSigForFile
                                            tcErrorsRev = newErrors :: tcAcc.tcErrorsRev 
                                            tcModuleNamesDict = moduleNamesDict }, sink, symbolEnv
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
                | Some (tcAcc, sink, senv) ->
                    this.SetPartialCheckResultByIndex (cacheIndex, PartialCheckResult.Checked (syntaxTree, tcAcc))
                    return (tcAcc, sink, senv)
                | _ -> return raise (OperationCanceledException ())
            | _ ->
                let senv = SymbolEnv (tcGlobals, tcAcc.tcState.Ccu, tcAcc.latestCcuSigForFile, tcImports)
                return (tcAcc, TcResultsSinkImpl tcGlobals, senv)
        }

    member this.SpeculativeCheckAsync (src, tcState: TcState, synExpr: SynExpr) =
        let tcConfig = this.tcConfig
        let tcGlobals = this.tcGlobals
        let tcImports = this.tcImports

        async {
            let! ct = Async.CancellationToken

            let syntaxTree = this.GetSyntaxTree src
            let (inputOpt, _) = syntaxTree.GetParseResult ct
            match inputOpt with
            | Some input ->
                let fullComputation =           
                    CompilationWorker.EnqueueAndAwaitAsync (fun ctok ->                        
                        if not (String.IsNullOrWhiteSpace src.FilePath) then
                            ApplyMetaCommandsFromInputToTcConfig (tcConfig, input, Path.GetDirectoryName src.FilePath) |> ignore

                        let sink = TcResultsSinkImpl tcGlobals
                        TryTypeCheckOneInputSynExpr (ctok, tcConfig, tcImports, tcGlobals, TcResultsSink.WithSink sink, tcState, input, synExpr)
                        |> Option.map (fun ty ->
                            (ty, sink)
                        )
                    )

                return! fullComputation
            | _ ->
                return None
        }

let getTcAccs state =
    state.orderedResults 
    |> Array.map (function
        | PartialCheckResult.Checked (_, tcAcc) -> tcAcc
        | _ -> failwith "should not happen, missing a checked file"
    )

let getPreEmitState state =
    let tcAccs = getTcAccs state

    // Get the state at the end of the type-checking of the last file
    let finalAcc = tcAccs.[tcAccs.Length-1]

    // Finish the checking
    let (tcEnvAtEndOfLastFile, topAttrs, mimpls, _), tcState = 
        let results = tcAccs |> List.ofArray |> List.map (fun acc-> acc.tcEnvAtEndOfFile, defaultArg acc.topAttribs EmptyTopAttrs, acc.latestImplFile, acc.latestCcuSigForFile)
        TypeCheckMultipleInputsFinish (results, finalAcc.tcState)

    {
        finalAcc = finalAcc
        tcEnvAtEndOfLastFile = tcEnvAtEndOfLastFile
        topAttrs = topAttrs
        implFiles = mimpls
        tcState = tcState
    }

[<Sealed>]
type IncrementalChecker (tcInitial: TcInitial, state: IncrementalCheckerState) =

    member __.ReplaceSource (oldSrc, newSrc) =
        IncrementalChecker (tcInitial, state.ReplaceSource (oldSrc, newSrc))

    member __.CheckAsync src =
        state.CheckAsync src

    member __.SpeculativeCheckAsync (src, tcState, synExpr) =
        state.SpeculativeCheckAsync (src, tcState, synExpr)

    member __.GetSyntaxTree src =
        state.GetSyntaxTree src

    member __.TcInitial = tcInitial

    member __.TcGlobals = state.tcGlobals

    member __.TcImports = state.tcImports

    member this.FinishAsync () =
        match state.orderedResults.[state.orderedResults.Length - 1] with
        | PartialCheckResult.Checked _ ->
            async { return getPreEmitState state }
        | result -> 
            async {
                let! _ = this.CheckAsync result.SyntaxTree.Source
                return getPreEmitState state
            }

    member this.SubmitSource (src: FSharpSource, ct) =
        let preEmitState = Async.RunSynchronously(this.FinishAsync (), cancellationToken = ct)
        IncrementalChecker (tcInitial, state.SubmitSource (src, preEmitState))

    static member Create(tcInitial: TcInitial, tcGlobals, tcImports, tcAcc, checkerOptions: CheckerOptions, srcs) =
        cancellable {
            let! state = IncrementalCheckerState.Create (tcInitial.tcConfig, tcGlobals, tcImports, tcAcc, checkerOptions, srcs)
            return IncrementalChecker (tcInitial, state)
        }
