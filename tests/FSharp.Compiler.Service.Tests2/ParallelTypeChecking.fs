module FSharp.Compiler.Service.Tests.ParallelTypeChecking

open System.Threading
open FSharp.Compiler
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Service.Tests.Graph
open FSharp.Compiler.Service.Tests.Types
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

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
        
    type State = TcState * bool
    type SingleResult = State -> FinalFileResult * State
    
    // TODO Use the real thing
    let typeCheckFile (file : File) (state : State) : SingleResult
        =
        fun (state : State) ->
            let res = file.Idx.Idx
            res.ToString(), $"{state}+{res}"
    
    type PartialResult = TcEnv * TopAttribs * CheckedImplFile option * ModuleOrNamespaceType

    /// Use parallel checking of implementation files that have signature files
    let CheckMultipleInputsInParallel2
        ((ctok,
            checkForErrors,
            tcConfig: TcConfig,
            tcImports: TcImports,
            tcGlobals,
            prefixPathOpt,
            tcState,
            eagerFormat,
            inputs): CancellationToken * (unit -> bool) * TcConfig * TcImports * TcGlobals * LongIdent option * TcState * (PhasedDiagnostic -> PhasedDiagnostic) * ParsedInput list) : PartialResult list * TcState =
        failwith ""
        
        

    
    let folder (state : State) (result : SingleResult): FinalFileResult * State =
        result state
            

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