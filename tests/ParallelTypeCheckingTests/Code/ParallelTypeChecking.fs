module FSharp.Compiler.Service.Tests.ParallelTypeChecking
#nowarn "1182"
open System.Collections.Generic
open System.Threading
open FSharp.Compiler
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Service.Tests.FileInfoGathering
open FSharp.Compiler.Service.Tests.Graph
open FSharp.Compiler.Service.Tests.Types
open FSharp.Compiler.Service.Tests.Utils
open FSharp.Compiler.Service.Tests2
open FSharp.Compiler.Service.Tests2.DepResolving
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Newtonsoft.Json

type FileGraph = Graph<File>

let calcFileGraph (files : SourceFiles) : FileGraph =
    // TODO Use DepResolving.fs
    failwith ""

// TODO Use real things
type State = string
type FinalFileResult = string
type SingleResult = State -> FinalFileResult * State

// TODO Use the real thing
let typeCheckFile (file : File) (state : State) : SingleResult
    =
    fun (state : State) ->
        let res = file.Idx.Idx
        res.ToString(), $"{state}+{res}"

// TODO Use the real thing
let folder (state : State) (result : SingleResult): FinalFileResult * State =
    result state
    
module internal Real =
    
    // Within a file, equip loggers to locally filter w.r.t. scope pragmas in each input
    let DiagnosticsLoggerForInput (tcConfig: TcConfig, input: ParsedInput, oldLogger) =
        CompilerDiagnostics.GetDiagnosticsLoggerFilteringByScopedPragmas(false, input.ScopedPragmas, tcConfig.diagnosticsOptions, oldLogger)
    
    type State = TcState * bool
    type FinalFileResult = TcEnv * TopAttribs * CheckedImplFile option * ModuleOrNamespaceType
    type SingleResult = State -> FinalFileResult * State
    type Item = File
    
    type PartialResult = TcEnv * TopAttribs * CheckedImplFile option * ModuleOrNamespaceType

    let folder (state : State) (result : SingleResult): FinalFileResult * State =
        result state
        
    /// Use parallel checking of implementation files that have signature files
    let CheckMultipleInputsInParallel
        ((ctok,
            checkForErrors,
            tcConfig: TcConfig,
            tcImports: TcImports,
            tcGlobals,
            prefixPathOpt,
            tcState,
            eagerFormat,
            inputs): 'a * (unit -> bool) * TcConfig * TcImports * TcGlobals * LongIdent option * TcState * (PhasedDiagnostic -> PhasedDiagnostic) * AST list)
        : FinalFileResult list * TcState
        =
        
        let sourceFiles =
            inputs
            |> List.toArray
            |> Array.mapi (fun i inp ->
                {
                    Idx = FileIdx.make i
                    AST = inp
                }
            )
        ParseAndCheckInputs.asts <-
            inputs
            |> List.map (fun ast -> ast.FileName, ast)
            |> readOnlyDict
            |> Dictionary<_,_>
        let graph = DepResolving.AutomatedDependencyResolving.detectFileDependencies sourceFiles
        
        let mutable nextIdx = (graph.Files |> Array.map (fun f -> f.File.Idx.Idx) |> Array.max) + 1
        let fakeX (idx : FileIdx) (fsi : string) : FileData =
            {
                File = File.FakeFs idx fsi
                Data =
                    {
                        Tops = [||]
                        ContainsModuleAbbreviations = false
                        ModuleRefs = [||]
                    }
            }
        let fsiXMap =
            graph.Files
            // fsi files
            |> Array.filter (fun f -> f.File.Name.EndsWith(".fsi"))
            // create fakes
            |> Array.map (fun fsi ->
                let idx = FileIdx.make nextIdx
                nextIdx <- nextIdx + 1
                fsi.File, fakeX idx fsi.File.Name
            )
            |> readOnlyDict
        let xFiles = fsiXMap.Values |> Seq.toArray
        let stuff =
            graph.Graph
            |> Seq.map (fun (KeyValue(node, deps)) ->
                let deps =
                    deps
                    |> Array.map (fun d ->
                        match (fsiXMap.TryGetValue d, (node.Name + "i" = d.Name)) with
                        | (true, xNode), false -> xNode.File
                        | (false, _), _
                        | _, true -> d
                    )
                node, deps
            )
            |> Seq.append (fsiXMap |> Seq.map (fun (KeyValue(fsi, x)) -> x.File, [|fsi|]))
            |> readOnlyDict
        let graph =
            {
                Files = Array.append graph.Files xFiles
                Graph = stuff |> Graph.fillEmptyNodes
            } : DepsResult
        
        
        let graphJson = graph.Graph |> Seq.map (fun (KeyValue(file, deps)) -> file.Name, deps |> Array.map (fun d -> d.Name)) |> dict
        let json = JsonConvert.SerializeObject(graphJson, Formatting.Indented)
        let path = $"c:/projekty/fsharp/heuristic/FCS.deps.json"
        System.IO.File.WriteAllText(path, json)
        
        let _ = ctok // TODO Use
        let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger

        // In the first linear part of parallel checking, we use a 'checkForErrors' that checks either for errors
        // somewhere in the files processed prior to each one, or in the processing of this particular file.
        let priorErrors = checkForErrors ()
        
        let processFile
            (file : File)
            ((input, logger) : ParsedInput * DiagnosticsLogger)
            ((currentTcState, currentPriorErrors) : State)
            : State -> PartialResult * State =
            cancellable {
                use _ = UseDiagnosticsLogger logger
                // Is it OK that we don't update 'priorErrors' after processing batches?
                let checkForErrors2 () = priorErrors || (logger.ErrorCount > 0)
                
                let tcSink = TcResultsSink.NoSink
                
                match file.AST with
                | ASTOrX.AST _ ->
                    printfn $"Processing AST {file.Name}"
                    let! f = CheckOneInput'(
                        checkForErrors2,
                        tcConfig,
                        tcImports,
                        tcGlobals,
                        prefixPathOpt,
                        tcSink,
                        currentTcState,
                        input,
                        false  // skipImpFiles...
                    )
            
                    printfn $"Finished Processing AST {file.Name}"
                    return
                        (fun (state : State) ->
                            let tcState, priorErrors = state
                            let (partialResult : PartialResult, tcState) = f tcState
            
                            let hasErrors = logger.ErrorCount > 0
                            // TODO Should we use local _priorErrors or global priorErrors? 
                            let priorOrCurrentErrors = priorErrors || hasErrors
                            let state : State = tcState, priorOrCurrentErrors
                            partialResult, state
                        )
                | ASTOrX.X fsi ->
                    printfn $"Processing X {file.Name}"

                    let hadSig = true
                    // Add dummy .fs results
                    // Adjust the TcState as if it has been checked, which makes the signature for the file available later
                    // in the compilation order.
                    let tcStateForImplFile = tcState
                    let fsName = fsi.TrimEnd('i')
                    let fsQualifiedName = asts[fsName].QualifiedName
                    let qualNameOfFile = fsQualifiedName
                    let priorErrors = checkForErrors ()
                    
                    // Add dummy TcState so that others can use this file through the .fsi stuff, without type-checking .fs
                    // Don't use it for this file's type-checking - it will cause duplicates
                    
                    let info = fsiBackedInfos[fsi]
                    match info with
                    // TODO Change
                    | amap, conditionalDefines, rootSig, priorErrors, filee, tcStateForImplFile, ccuSigForFile ->
                        printfn $"Finished Processing X {file.Name}"
                        return
                            (fun (state : State) ->
                                // (tcState.TcEnvFromImpls, EmptyTopAttrs, None, ccuSigForFile), state
                                printfn $"Applying X state {file.Name}"                        
                                let tcState, priorErrors = state
                                // (tcState.TcEnvFromImpls, EmptyTopAttrs, None, ccuSigForFile), state 
                                
                                let ccuSigForFile, tcState =
                                    AddCheckResultsToTcState
                                        (tcGlobals, tcImports.GetImportMap(), hadSig, prefixPathOpt, tcSink, tcState.TcEnvFromImpls, qualNameOfFile, ccuSigForFile)
                                        tcState
                                let partialResult = tcState.TcEnvFromImpls, EmptyTopAttrs, None, ccuSigForFile
                
                                let hasErrors = logger.ErrorCount > 0
                                // TODO Should we use local _priorErrors or global priorErrors? 
                                let priorOrCurrentErrors = priorErrors || hasErrors
                                let state : State = tcState, priorOrCurrentErrors
                                printfn $"Finished applying X state {file.Name}"
                                partialResult, state
                            )
            }
            |> Cancellable.runWithoutCancellation
            
        UseMultipleDiagnosticLoggers (inputs, diagnosticsLogger, Some eagerFormat) (fun inputsWithLoggers ->
            // Equip loggers to locally filter w.r.t. scope pragmas in each input
            let inputsWithLoggers: IReadOnlyDictionary<FileIdx,(ParsedInput * DiagnosticsLogger)> =
                inputsWithLoggers
                |> Seq.mapi (fun i (input, oldLogger) ->
                    let logger = DiagnosticsLoggerForInput(tcConfig, input, oldLogger)
                    FileIdx.make i, (input, logger))
                |> readOnlyDict
            

            let graph: Graph<File> = graph.Graph
            let processFile (file : File) (state : State) : State -> PartialResult * State =
                let parsedInput, logger =
                    match file.AST with
                    | ASTOrX.AST ast ->
                        ast, inputsWithLoggers[file.Idx] |> snd
                    | ASTOrX.X _ ->
                        inputs |> List.item 0, diagnosticsLogger
                processFile file (parsedInput, logger) state
                
            let folder: State -> SingleResult -> FinalFileResult * State = folder
            let qnof = QualifiedNameOfFile.QualifiedNameOfFile (Ident("", Range.Zero))
            let state: State = tcState, priorErrors
            
            let partialResults, (tcState, _) =
                GraphProcessing.processGraph<File, State, SingleResult, FinalFileResult>
                    graph
                    processFile
                    folder
                    state
                    (fun it -> (not it.FsiBacked) && it = it)
                    1
            
            partialResults |> Array.toList, tcState
        )





module internal Nojaf =
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
                    let logger = Real.DiagnosticsLoggerForInput(tcConfig, input, oldLogger)
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

                AutomatedDependencyResolving.detectFileDependencies sourceFiles

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
        



            

let typeCheckGraph (graph : FileGraph) : FinalFileResult[] * State =
    let parallelism = 4 // cpu count?
    GraphProcessing.processGraph
        graph
        typeCheckFile
        folder
        ""
        (fun _ -> true)
        parallelism
        
let typeCheckGraph2 (graph : FileGraph) : FinalFileResult[] * State =
    let parallelism = 4 // cpu count?
    GraphProcessing.processGraph
        graph
        typeCheckFile
        folder
        ""
        (fun _ -> true)
        parallelism
    
let typeCheck (files : SourceFiles) : FinalFileResult[] * State =
    let graph = calcFileGraph files
    let state = typeCheckGraph graph
    state