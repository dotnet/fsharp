module ParallelTypeCheckingTests.GraphResolvingAndProcessing
#nowarn "1182"
open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Service.Tests.Graph
open FSharp.Compiler.Service.Tests.Types
open FSharp.Compiler.Service.Tests.Utils
open NUnit.Framework

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
let ``Process a diamond graph`` () =
    let state =
        GraphProcessing.processGraph
            deps
            (fun i _state -> i)
            (fun state res -> res, $"{state}+{res}")
            ""
            (fun _ -> true)
            8
    
    printfn $"End state: {state}"

open FSharp.Compiler.Service.Tests.ParallelTypeChecking

[<Test>]
let ``Dummy type-check of a simple a-b graph`` () =
    let graph : FileGraph =
        let a =
            {
                Idx = FileIdx.make 1
                Code = "module X = let a = 3"
                AST = ASTOrX.AST <| parseSourceCode ("A.fs", "module X = let a = 3")
                FsiBacked = false
            }
            
        let b =
            {
                Idx = FileIdx.make 2
                Code = "module Y = let b = 3"
                AST = ASTOrX.AST <| parseSourceCode ("B.fs", "module Y = let b = 3")
                FsiBacked = false
            }
        [|
            a, [||]
            b, [|a|]
        |]
        |> readOnlyDict
        
    let _res = typeCheckGraph graph
    ()
    