module TypeChecks.GraphProcessingTests

open System.Threading
open FSharp.Compiler.GraphChecking.GraphProcessing
open Xunit

[<Fact>]
let ``When processing a node throws an exception, an exception is raised with the original exception included`` () =
    let graph = [1, [|2|]; 2, [||]] |> readOnlyDict
    let work (_processor : int -> ProcessedNode<int, string>) (_node : NodeInfo<int>) : string = failwith "Work exception"
    
    let exn =
        Assert.Throws<GraphProcessingException>(
            fun () ->
                processGraph
                    graph
                    work
                    CancellationToken.None
                |> ignore
        )
    Assert.Equal(exn.Message, "Encountered exception when processing item '2'")
    Assert.NotNull(exn.InnerException)
    Assert.Equal(exn.InnerException.Message, "Work exception")
