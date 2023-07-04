﻿module TypeChecks.GraphProcessingTests

open System.Threading
open FSharp.Compiler.GraphChecking.GraphProcessing
open NUnit.Framework

[<Test>]
let ``When processing a node throws an exception, an exception is raised with the original exception included`` () =
    let graph = [1, [|2|]; 2, [||]] |> readOnlyDict
    let work (_processor : int -> ProcessedNode<int, string>) (_node : NodeInfo<int>) : string = failwith "Work exception"
    
    let exn =
        Assert.Throws<System.Exception>(
            fun () ->
                processGraph
                    graph
                    work
                    CancellationToken.None
                |> ignore
        )
    Assert.That(exn.Message, Is.EqualTo("Encountered exception when processing item '2'"))
    Assert.That(exn.InnerException, Is.Not.Null)
    Assert.That(exn.InnerException.Message, Is.EqualTo("Work exception"))
