module FSharp.Compiler.Service.Tests.ParallelTypeChecking

open FSharp.Compiler.Service.Tests.Graph
open FSharp.Compiler.Service.Tests.Types

type FileGraph = Graph<File>

let calcFileGraph (files : SourceFiles) : FileGraph =
    // TODO Use DepResolving.fs
    failwith ""

// TODO Use real things
type State = string
type SingleResult = int

// TODO Use the real thing
let typeCheckFile (file : File) (state : State) : SingleResult
    =
    file.Idx.Idx

// TODO Use the real thing
let folder (state : State) (result : SingleResult) =
    $"{state}+{result}"

// TODO We probably need to return partial results as well
let typeCheckGraph (graph : FileGraph) : State =
    let parallelism = 4 // cpu count?
    let state =
        GraphProcessing.processGraph
            graph
            typeCheckFile
            folder
            ""
            parallelism
    state
    
let typeCheck (files : SourceFiles) : State =
    let graph = calcFileGraph files
    let state = typeCheckGraph graph
    state