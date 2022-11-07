module internal ParallelTypeCheckingTests.SingleTcStateTypeChecking
#nowarn "1182"
open FSharp.Compiler
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open ParallelTypeCheckingTests
open ParallelTypeCheckingTests.Types
open ParallelTypeCheckingTests.Utils
open ParallelTypeCheckingTests.DepResolving
open FSharp.Compiler.Syntax
open FSharp.Compiler.TypedTree
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

type PartialResult = TcEnv * TopAttribs * CheckedImplFile option * ModuleOrNamespaceType

type SignaturePairResult =
    Import.ImportMap * string list option * ModuleOrNamespaceType * bool * ParsedImplFileInput * TcState * ModuleOrNamespaceType

[<NoEquality; NoComparison; RequireQualifiedAccess>]
type TypeCheckResponse =
    | ImplementationFile of topAttrs: TopAttribs * implFile: CheckedImplFile * tcEnvAtEnd: TcEnv * createsGeneratedProvidedTypes: bool
    | SignatureFile of tcEnv: TcEnv * sigFileType: ModuleOrNamespaceType * createsGeneratedProvidedTypes: bool * implIdx: int

type ParallelTypeCheckMsg =
    | TypeCheckCompleted of
        index: int *
        response: TypeCheckResponse *
        replyChannel: AsyncReplyChannel<Choice<PartialResult, SignaturePairResult> array * TcState>
    | StartTypeCheck of index: int * replyChannel: AsyncReplyChannel<Choice<PartialResult, SignaturePairResult> array * TcState>
    | Start of
        inputFiles: (ParsedInput * DiagnosticsLogger) array *
        replyChannel: AsyncReplyChannel<Choice<PartialResult, SignaturePairResult> array * TcState>

type ParallelTypeCheckModel =
    {
        CurrentTcState: TcState
        Free: Set<int>
        Processing: Set<int>
        Input: (ParsedInput * DiagnosticsLogger) array
        Results: Choice<PartialResult, SignaturePairResult> array
    }

/// Use parallel checking of implementation files that have signature files
let CheckMultipleInputsInParallel
    (
        ctok,
        checkForErrors,
        tcConfig: TcConfig,
        tcImports: TcImports,
        tcGlobals,
        prefixPathOpt,
        tcState,
        eagerFormat,
        inputs
    ) =

    let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger

    // We create one CapturingDiagnosticLogger for each file we are processing and
    // ensure the diagnostics are presented in deterministic order.
    //
    // eagerFormat is used to format diagnostics as they are emitted, just as they would be in the command-line
    // compiler. This is necessary because some formatting of diagnostics is dependent on the
    // type inference state at precisely the time the diagnostic is emitted.
    UseMultipleDiagnosticLoggers (inputs, diagnosticsLogger, Some eagerFormat) (fun inputsWithLoggers ->

        // Equip loggers to locally filter w.r.t. scope pragmas in each input
        let inputsWithLoggers: (ParsedInput * DiagnosticsLogger)[] =
            inputsWithLoggers
            |> Seq.map (fun (input, oldLogger) ->
                let logger = ParallelTypeChecking.DiagnosticsLoggerForInput(tcConfig, input, oldLogger)
                input, logger)
            |> Seq.toArray

        // In the first linear part of parallel checking, we use a 'checkForErrors' that checks either for errors
        // somewhere in the files processed prior to each one, or in the processing of this particular file.
        let priorErrors = checkForErrors ()

        let graph: DepsResult =
            let sourceFiles =
                inputs
                |> List.toArray
                |> Array.mapi (fun i inp -> { Idx = FileIdx.make i; AST = inp }: SourceFile)

            DependencyResolution.detectFileDependencies sourceFiles

        do ()

        let partialResults, tcState =
            let amap = tcImports.GetImportMap()

            let conditionalDefines =
                if tcConfig.noConditionalErasure then
                    None
                else
                    Some tcConfig.conditionalDefines

            let agent =
                MailboxProcessor<ParallelTypeCheckMsg>.Start
                    (fun inbox ->
                        let rec loop (state: ParallelTypeCheckModel) =
                            async {
                                let! msg = inbox.Receive()

                                match msg with
                                | ParallelTypeCheckMsg.TypeCheckCompleted (index, response, channel) ->
                                    let input, _ = inputsWithLoggers.[index]

                                    let updateTcState =
                                        match response with
                                        | TypeCheckResponse.ImplementationFile (topAttrs,
                                                                                implFile,
                                                                                tcEnvAtEnd,
                                                                                createsGeneratedProvidedTypes) ->
                                            let x = state.CurrentTcState.CreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes
                                            let tcState = state.CurrentTcState.WithCreatesGeneratedProvidedTypes x

                                            let ccuSigForFile, updateTcState =
                                                AddCheckResultsToTcState
                                                    (tcGlobals,
                                                     amap,
                                                     false,
                                                     prefixPathOpt,
                                                     TcResultsSink.NoSink,
                                                     tcState.TcEnvFromImpls,
                                                     input.QualifiedName,
                                                     implFile.Signature)
                                                    tcState

                                            state.Results.[index] <- Choice1Of2(tcEnvAtEnd, topAttrs, Some implFile, ccuSigForFile)
                                            updateTcState
                                        | TypeCheckResponse.SignatureFile (tcEnv, sigFileType, createsGeneratedProvidedTypes, implIdx) ->
                                            let qualNameOfFile = input.QualifiedName
                                            let rootSigs = Zmap.add qualNameOfFile sigFileType state.CurrentTcState.TcsRootSigs

                                            // Add the signature to the signature env (unless it had an explicit signature)
                                            let ccuSigForFile =
                                                TypedTreeOps.CombineCcuContentFragments [ sigFileType; state.CurrentTcState.CcuSig ]

                                            let tcStateAfterSig =
                                                let creates = state.CurrentTcState.CreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes
                                                state.CurrentTcState.WithStuff tcEnv rootSigs creates

                                            state.Results.[index] <- Choice1Of2(tcEnv, EmptyTopAttrs, None, ccuSigForFile)

                                            let implFile =
                                                match fst inputsWithLoggers.[implIdx] with
                                                | ParsedInput.SigFile _ -> failwith "should be an implementation file"
                                                | ParsedInput.ImplFile file -> file

                                            let qualNameOfFile = input.QualifiedName
                                            let priorErrors = checkForErrors ()

                                            let ccuSigForFile, tcStateAfterImpl =
                                                AddCheckResultsToTcState
                                                    (tcGlobals,
                                                     amap,
                                                     true,
                                                     prefixPathOpt,
                                                     TcResultsSink.NoSink,
                                                     tcStateAfterSig.TcEnvFromImpls,
                                                     qualNameOfFile,
                                                     sigFileType)
                                                    tcStateAfterSig

                                            state.Results.[implIdx] <-
                                                Choice2Of2(
                                                    amap,
                                                    conditionalDefines,
                                                    sigFileType,
                                                    priorErrors,
                                                    implFile,
                                                    tcStateAfterImpl,
                                                    ccuSigForFile
                                                )

                                            tcStateAfterImpl

                                    let allFree =
                                        match response with
                                        | TypeCheckResponse.ImplementationFile _ -> Set.add index state.Free
                                        | TypeCheckResponse.SignatureFile (implIdx = implIdx) ->
                                            state.Free |> Set.add index |> Set.add implIdx

                                    if allFree.Count = state.Input.Length then
                                        channel.Reply(state.Results, updateTcState)
                                    else
                                        let nextFree =
                                            let alreadyFired = Set.union allFree state.Processing

                                            graph.Graph
                                            |> Seq.choose (fun (KeyValue (f, deps)) ->
                                                let idx = f.Idx.Idx

                                                if alreadyFired.Contains idx then
                                                    None
                                                elif Seq.forall (fun (dep: File) -> Set.contains dep.Idx.Idx allFree) deps then
                                                    Some idx
                                                else
                                                    None)
                                            |> Seq.toArray

                                        Array.iter
                                            (fun freeIndex -> inbox.Post(ParallelTypeCheckMsg.StartTypeCheck(freeIndex, channel)))
                                            nextFree

                                        return!
                                            loop
                                                { state with
                                                    CurrentTcState = updateTcState
                                                    Free = allFree
                                                    Processing = Set.unionMany [| state.Processing; Set.ofArray nextFree |]
                                                }

                                | ParallelTypeCheckMsg.StartTypeCheck (idx, channel) ->
                                    let input, logger = inputsWithLoggers.[idx]
                                    use _ = UseDiagnosticsLogger logger

                                    let checkForErrors2 () = priorErrors || (logger.ErrorCount > 0)

                                    match input with
                                    | ParsedInput.SigFile file ->
                                        let m = input.Range
                                        let qualNameOfFile = file.QualifiedName

                                        // Check if we've seen this top module signature before.
                                        if Zmap.mem qualNameOfFile state.CurrentTcState.TcsRootSigs then
                                            errorR (Error(FSComp.SR.buildSignatureAlreadySpecified qualNameOfFile.Text, m.StartRange))

                                        // Check if the implementation came first in compilation order
                                        if Zset.contains qualNameOfFile state.CurrentTcState.TcsRootImpls then
                                            errorR (Error(FSComp.SR.buildImplementationAlreadyGivenDetail (qualNameOfFile.Text), m))

                                        let conditionalDefines =
                                            if tcConfig.noConditionalErasure then
                                                None
                                            else
                                                Some tcConfig.conditionalDefines

                                        // Typecheck the signature file
                                        cancellable {
                                            let! tcEnv, sigFileType, createsGeneratedProvidedTypes =
                                                CheckOneSigFile
                                                    (tcGlobals,
                                                     amap,
                                                     state.CurrentTcState.Ccu,
                                                     checkForErrors,
                                                     conditionalDefines,
                                                     TcResultsSink.NoSink,
                                                     tcConfig.internalTestSpanStackReferring)
                                                    state.CurrentTcState.TcEnvFromSignatures
                                                    file

                                            let implIndex =
                                                [| idx + 1 .. inputsWithLoggers.Length - 1 |]
                                                |> Array.tryPick (fun idx ->
                                                    let f = fst inputsWithLoggers.[idx]

                                                    if f.QualifiedName.Text = qualNameOfFile.Text then
                                                        Some idx
                                                    else
                                                        None)
                                                |> function
                                                    | None -> failwith "No signature file"
                                                    | Some idx -> idx

                                            inbox.Post(
                                                ParallelTypeCheckMsg.TypeCheckCompleted(
                                                    idx,
                                                    TypeCheckResponse.SignatureFile(
                                                        tcEnv,
                                                        sigFileType,
                                                        createsGeneratedProvidedTypes,
                                                        implIndex
                                                    ),
                                                    channel
                                                )
                                            )
                                        }
                                        |> Cancellable.toAsync
                                        |> Async.Start

                                    | ParsedInput.ImplFile file ->
                                        let qualNameOfFile = file.QualifiedName

                                        // Check if we've already seen an implementation for this fragment
                                        if Zset.contains qualNameOfFile state.CurrentTcState.TcsRootImpls then
                                            errorR (Error(FSComp.SR.buildImplementationAlreadyGiven qualNameOfFile.Text, input.Range))

                                        let conditionalDefines =
                                            if tcConfig.noConditionalErasure then
                                                None
                                            else
                                                Some tcConfig.conditionalDefines

                                        // Typecheck the implementation file
                                        cancellable {
                                            let! topAttrs, implFile, tcEnvAtEnd, createsGeneratedProvidedTypes =
                                                CheckOneImplFile(
                                                    tcGlobals,
                                                    amap,
                                                    state.CurrentTcState.Ccu,
                                                    state.CurrentTcState.TcsImplicitOpenDeclarations,
                                                    checkForErrors2,
                                                    conditionalDefines,
                                                    TcResultsSink.NoSink,
                                                    tcConfig.internalTestSpanStackReferring,
                                                    state.CurrentTcState.TcEnvFromImpls,
                                                    None,
                                                    file
                                                )

                                            inbox.Post(
                                                ParallelTypeCheckMsg.TypeCheckCompleted(
                                                    idx,
                                                    TypeCheckResponse.ImplementationFile(
                                                        topAttrs,
                                                        implFile,
                                                        tcEnvAtEnd,
                                                        createsGeneratedProvidedTypes
                                                    ),
                                                    channel
                                                )
                                            )
                                        }
                                        |> Cancellable.toAsync
                                        |> Async.Start

                                    return! loop state

                                | ParallelTypeCheckMsg.Start (inputFiles, channel) ->
                                    let initialFreeIndexes =
                                        [|
                                            for KeyValue (file, deps) in graph.Graph do
                                                if Array.isEmpty deps then
                                                    yield file.Idx.Idx
                                        |]
                                        |> set

                                    Seq.iter
                                        (fun freeIndex -> inbox.Post(ParallelTypeCheckMsg.StartTypeCheck(freeIndex, channel)))
                                        initialFreeIndexes

                                    return!
                                        loop
                                            { state with
                                                Processing = initialFreeIndexes
                                                Input = inputFiles
                                            }
                            }

                        loop
                            {
                                CurrentTcState = tcState
                                Free = Set.empty
                                Processing = Set.empty
                                Input = Array.empty
                                Results = Array.zeroCreate graph.Graph.Count
                            })

            agent.PostAndReply(fun channel -> ParallelTypeCheckMsg.Start(inputsWithLoggers, channel))

        // Do the parallel phase, checking all implementation files that did have a signature, in parallel.
        let results, createsGeneratedProvidedTypesFlags =
            Array.zip partialResults inputsWithLoggers
            |> ArrayParallel.map (fun (partialResult, (_, logger)) ->
                use _ = UseDiagnosticsLogger logger
                use _ = UseBuildPhase BuildPhase.TypeCheck

                RequireCompilationThread ctok

                match partialResult with
                | Choice1Of2 result -> result, false
                | Choice2Of2 (amap, conditionalDefines, rootSig, priorErrors, file, tcStateForImplFile, ccuSigForFile) ->

                    // In the first linear part of parallel checking, we use a 'checkForErrors' that checks either for errors
                    // somewhere in the files processed prior to this one, including from the first phase, or in the processing
                    // of this particular file.
                    let checkForErrors2 () = priorErrors || (logger.ErrorCount > 0)

                    let topAttrs, implFile, tcEnvAtEnd, createsGeneratedProvidedTypes =
                        CheckOneImplFile(
                            tcGlobals,
                            amap,
                            tcStateForImplFile.Ccu,
                            tcStateForImplFile.TcsImplicitOpenDeclarations,
                            checkForErrors2,
                            conditionalDefines,
                            TcResultsSink.NoSink,
                            tcConfig.internalTestSpanStackReferring,
                            tcStateForImplFile.TcEnvFromImpls,
                            Some rootSig,
                            file
                        )
                        |> Cancellable.runWithoutCancellation

                    let result = (tcEnvAtEnd, topAttrs, Some implFile, ccuSigForFile)
                    result, createsGeneratedProvidedTypes)
            |> Array.toList
            |> List.unzip

        let x = tcState.CreatesGeneratedProvidedTypes || (createsGeneratedProvidedTypesFlags |> List.exists id)
        let tcState = tcState.WithCreatesGeneratedProvidedTypes x
        results, tcState)
    