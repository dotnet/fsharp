module FSharp.Compiler.Service.Tests2.RunCompiler

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Service.Tests.Graph
open NUnit.Framework

[<Test>]
let runCompiler () =
    let args =
        System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
    FSharp.Compiler.CommandLineMain.main args |> ignore
    
[<Test>]
let runGrapher () =
    // let args =
    //     System.IO.File.ReadAllLines(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.Service.Tests2\args.txt") |> Array.skip 1
    // FSharp.Compiler.CommandLineMain.main args |> ignore
    
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
    
    let dependants = deps |> Graph.reverse
    let transitiveDeps = deps |> Graph.transitive
    let transitiveDependants = transitiveDeps |> Graph.reverse
    
    let nodes =
        deps.Keys
        |> Seq.map (fun idx -> idx, {Idx = idx; Deps = [||]; Dependants = [||]; TransitiveDeps = [||]; Result = None; UnprocessedDepsCount = 0; _lock = Object()})
        |> readOnlyDict
    
    let processs deps = deps |> Array.map (fun d -> nodes[d])
    
    let graph =
        nodes
        |> Seq.iter (fun (KeyValue(idx, node)) ->
            node.Deps <- processs deps[idx]
            node.TransitiveDeps <- processs transitiveDeps[idx]
            node.Dependants <- processs dependants[idx]
            node.UnprocessedDepsCount <- node.Deps.Length
        )
        nodes.Values
        |> Seq.toArray
    
    GraphProcessing.processGraph graph