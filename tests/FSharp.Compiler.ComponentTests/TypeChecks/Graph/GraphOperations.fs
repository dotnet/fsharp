module FSharp.Compiler.ComponentTests.TypeChecks.Graph.GraphOperations

open Xunit
open FSharp.Compiler.GraphChecking


[<Fact>]
let ``Transform graph to layers of leaves`` () = 

    let g = Graph.make [
        'B', [|'A'|]
        'C', [|'A'|]
        'E', [|'A'; 'B'; 'C'|]
        'F', [|'C'; 'D'|]
    ]

    //let layers = g |> Graph.leaves |> Seq.toList

    let _expected = [
        [|'A'; 'D'|]
        [|'B'; 'C'|]
        [|'E'; 'F'|]
    ]

    let _x = Graph.reverse g

    ()

    //Assert.Equal<string array>(expected, layers)

