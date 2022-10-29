module FSharp.Compiler.Service.Tests.code

open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Service.Tests.Graph
open FSharp.Compiler.Service.Tests.Types

type FileGraph = Graph<File>

let calcFileGraph (files : SourceFiles) : FileGraph =
    failwith ""

type State = string
type SingleResult = int

let typeCheckFile (file : File) (state : State) : SingleResult
    =
    file.Idx.Idx

let folder (state : string) (result : int) =
    $"{state}+{result}" 

let typeCheckGraph (graph : FileGraph) : TcState =
    let parallelism = 4 // cpu count?
    let state =
        GraphProcessing.processGraph
            graph
            typeCheckFile
            folder
            parallelism
     state
    
let typeCheck (files : SourceFiles) : TcState =
    let graph = calcFileGraph files
    let state = typeCheckGraph graph
    state