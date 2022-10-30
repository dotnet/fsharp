module FSharp.Compiler.Service.Tests.ParallelTypeChecking

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
open FSharp.Compiler.Service.Tests.Graph
open FSharp.Compiler.Service.Tests.Types
open FSharp.Compiler.Service.Tests.Utils
open FSharp.Compiler.Service.Tests2
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open Internal.Utilities.Library
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
    let CheckMultipleInputsInParallelMy
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
        let graph = DepResolving.AutomatedDependencyResolving.detectFileDependencies sourceFiles
        
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
            ((input, logger) : ParsedInput * DiagnosticsLogger)
            ((currentTcState, currentPriorErrors) : State)
            : State -> PartialResult * State =
            cancellable {
                use _ = UseDiagnosticsLogger logger
                // Is it OK that we don't update 'priorErrors' after processing batches?
                let checkForErrors2 () = priorErrors || (logger.ErrorCount > 0)
        
                let tcSink = TcResultsSink.NoSink
        
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
                let parsedInput, logger = inputsWithLoggers[file.Idx]
                processFile (parsedInput, logger) state
                
            let folder: State -> SingleResult -> FinalFileResult * State = folder
            let state: State = tcState, priorErrors
            
            let partialResults, (tcState, _) =
                GraphProcessing.processGraph<File, State, SingleResult, FinalFileResult>
                    graph
                    processFile
                    folder
                    state
                    12
            
            partialResults |> Array.toList, tcState
        )
            

let typeCheckGraph (graph : FileGraph) : FinalFileResult[] * State =
    let parallelism = 4 // cpu count?
    GraphProcessing.processGraph
        graph
        typeCheckFile
        folder
        ""
        parallelism
        
let typeCheckGraph2 (graph : FileGraph) : FinalFileResult[] * State =
    let parallelism = 4 // cpu count?
    GraphProcessing.processGraph
        graph
        typeCheckFile
        folder
        ""
        parallelism
    
let typeCheck (files : SourceFiles) : FinalFileResult[] * State =
    let graph = calcFileGraph files
    let state = typeCheckGraph graph
    state