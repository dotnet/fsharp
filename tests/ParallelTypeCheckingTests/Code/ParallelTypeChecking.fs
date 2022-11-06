module internal ParallelTypeCheckingTests.ParallelTypeChecking
#nowarn "1182"
open System.Collections.Concurrent
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
open ParallelTypeCheckingTests.FileInfoGathering
open ParallelTypeCheckingTests.Graph
open ParallelTypeCheckingTests.Types
open ParallelTypeCheckingTests.Utils
open ParallelTypeCheckingTests
open ParallelTypeCheckingTests.DepResolving
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Newtonsoft.Json

type FileGraph = Graph<File>

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
        |> ConcurrentDictionary<_,_>
    let graph = DepResolving.AutomatedDependencyResolving.detectFileDependencies sourceFiles
    
    let mutable nextIdx = (graph.Files |> Array.map (fun f -> f.File.Idx.Idx) |> Array.max) + 1
    let fakeX (idx : FileIdx) (fsi : string) : FileData =
        {
            File = File.FakeFs idx fsi
            Data =
                {
                    Tops = [||]
                    Abbreviations = [||]
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
    
    let useXFiles = true
    let graph =
        if useXFiles then
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
            {
                Files =  Array.append graph.Files xFiles
                Graph = stuff |> Graph.fillEmptyNodes
            } : DepsResult
        else
            graph
    graph.Graph |> Graph.print
    
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
        ((currentTcState, _currentPriorErrors) : State)
        : State -> PartialResult * State =
        cancellable {
            use _ = UseDiagnosticsLogger logger
            // Is it OK that we don't update 'priorErrors' after processing batches?
            let checkForErrors2 () = priorErrors || (logger.ErrorCount > 0)
            
            let tcSink = TcResultsSink.NoSink
            
            match file.AST with
            | ASTOrX.AST _ ->
                printfn $"Processing AST {file.ToString()}"
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
        
                printfn $"Finished Processing AST {file.ToString()}"
                return
                    (fun (state : State) ->
                        printfn $"Applying {file.ToString()}"
                        let tcState, priorErrors = state
                        let (partialResult : PartialResult, tcState) = f tcState
        
                        let hasErrors = logger.ErrorCount > 0
                        // TODO Should we use local _priorErrors or global priorErrors? 
                        let priorOrCurrentErrors = priorErrors || hasErrors
                        let state : State = tcState, priorOrCurrentErrors
                        printfn $"Finished applying {file.ToString()}"
                        partialResult, state
                    )
            | ASTOrX.X fsi ->
                printfn $"Processing X {file.ToString()}"

                let hadSig = true
                // Add dummy .fs results
                // Adjust the TcState as if it has been checked, which makes the signature for the file available later
                // in the compilation order.
                let fsName = fsi.TrimEnd('i')
                let fsQualifiedName = asts[fsName].QualifiedName
                let qualNameOfFile = fsQualifiedName
                
                // Add dummy TcState so that others can use this file through the .fsi stuff, without type-checking .fs
                // Don't use it for this file's type-checking - it will cause duplicates
                
                let ccuSigForFile = fsiBackedInfos[fsi]
                printfn $"Finished Processing X {file}"
                return
                    (fun (state : State) ->
                        // (tcState.TcEnvFromImpls, EmptyTopAttrs, None, ccuSigForFile), state
                        printfn $"Applying X state {file}"                        
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
                        printfn $"Finished applying X state {file}"
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
        let _qnof = QualifiedNameOfFile.QualifiedNameOfFile (Ident("", Range.Zero))
        let state: State = tcState, priorErrors
        
        let partialResults, (tcState, _) =
            GraphProcessing.processGraph<File, State, SingleResult, FinalFileResult>
                graph
                processFile
                folder
                state
                (fun it -> not <| it.Name.EndsWith(".fsix"))
                8
        
        partialResults |> Array.toList, tcState
    )
