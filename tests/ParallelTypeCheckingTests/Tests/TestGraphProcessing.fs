module ParallelTypeCheckingTests.TestGraphProcessing
#nowarn "1182"
open ParallelTypeCheckingTests
open ParallelTypeCheckingTests.Graph
open ParallelTypeCheckingTests.ParallelTypeChecking
open ParallelTypeCheckingTests.Types
open ParallelTypeCheckingTests.Utils
open NUnit.Framework

/// Used for testing the graph processing code
module FakeGraphProcessing =
    type State = string
    type FinalFileResult = string
    type SingleResult = State -> FinalFileResult * State

    let typeCheckFile<'Item> (item : 'Item) (_state : State) : SingleResult
        =
        fun (state : State) ->
            let res = item.ToString()
            res.ToString(), $"{state}+{res}"

    let folder (state : State) (result : SingleResult): FinalFileResult * State =
        result state            

    let processFileGraph<'Item when 'Item : comparison> (graph : Graph<'Item>) : FinalFileResult[] * State =
        let parallelism = 4 // cpu count?
        GraphProcessing.processGraph
            graph
            typeCheckFile
            folder
            ""
            (fun _ -> true)
            parallelism

let deps : Graph<int> =
    [|
        0, [||]  // A
        1, [|0|] // B1 -> A
        2, [|1|] // B2 -> B1
        3, [|0|] // C1 -> A
        4, [|3|] // C2 -> C1
        5, [|2; 4|] // D -> B2, C2
    |]
    |> readOnlyDict
    
[<Test>]
let ``Process a diamond graph of numbers`` () =
    let results, state = FakeGraphProcessing.processFileGraph deps
    printfn $"End state: {state}"
    printfn $"Results: %+A{results}"

[<Test>]
let ``Dummy type-check of a simple a-b graph`` () =
    let graph : FileGraph =
        let code ="""
module X
let a = 3
"""
        let a =
            {
                Idx = FileIdx.make 1
                Code = code 
                AST = ASTOrX.AST <| parseSourceCode ("A.fs", code)
                FsiBacked = false
            }
            
        let code = """
module Y
let b = 3
"""
        let b =
            {
                Idx = FileIdx.make 2
                Code = code
                AST = ASTOrX.AST <| parseSourceCode ("B.fs", code)
                FsiBacked = false
            }
        [|
            a, [||]
            b, [|a|]
        |]
        |> readOnlyDict
        
    let results, state = FakeGraphProcessing.processFileGraph graph
    printfn $"End state: {state}"
    printfn $"Results: %+A{results}"
    