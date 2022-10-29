module FSharp.Compiler.Service.Tests2.RunCompiler

open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Service.Tests.Graph
open NUnit.Framework

[<Test>]
[<Explicit>]
let runCompiler () =
    let args =
        System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
    FSharp.Compiler.CommandLineMain.main args |> ignore
    
[<Test>]
let runGrapher () =
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
    
    let state =
        GraphProcessing.processGraph
            deps
            (fun i state -> i)
            (fun state res -> $"{state}+{res}")
            ""
            8
    
    printfn $"End state: {state}"