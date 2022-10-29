module FSharp.Compiler.Service.Tests2.RunCompiler

open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Service.Tests.Graph
open FSharp.Compiler.Service.Tests.Types
open FSharp.Compiler.Service.Tests.Utils
open NUnit.Framework

[<Test>]
[<Explicit>]
let runCompiler () =
    let args =
        // System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
        System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\SimpleArgs.txt")
    let exit = FSharp.Compiler.CommandLineMain.main args
    ()
    
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
let runGrapher () =

    
    let state =
        GraphProcessing.processGraph
            deps
            (fun i state -> i)
            (fun state res -> res, $"{state}+{res}")
            ""
            8
    
    printfn $"End state: {state}"

open FSharp.Compiler.Service.Tests.ParallelTypeChecking
[<Test>]
let foo () =
    let graph : FileGraph =
        let a =
            {
                Idx = FileIdx.make 1
                Code = "module X = let a = 3"
                AST = parseSourceCode ("A.fs", "module X = let a = 3")
                FsiBacked = false
            }
            
        let b =
            {
                Idx = FileIdx.make 2
                Code = "module Y = let b = 3"
                AST = parseSourceCode ("B.fs", "module Y = let b = 3")
                FsiBacked = false
            }
        [|
            a, [||]
            b, [|a|]
        |]
        |> readOnlyDict
        
    let res = typeCheckGraph graph
    ()
    
[<Test>]
let runGrapher2 () =
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
    
    let addResult (state : string) (res : int) =
        $"{state}+{res}"
    
    let state =
        GraphProcessing.processGraph
            deps
            (fun i state -> fun (state : string) -> i, addResult state i)
            (fun state f ->
                let (partial, state) : int * string = f state
                partial, state
            )
            ""
            8
    
    printfn $"End state: {state}"