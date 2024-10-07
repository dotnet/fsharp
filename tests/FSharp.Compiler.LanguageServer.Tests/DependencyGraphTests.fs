module DependencyGraphTests


open FSharp.Compiler.LanguageServer.Common.DependencyGraph
open Xunit


[<Fact>]
let ``Can add a node to the graph`` () =
    let graph = DependencyGraph()
    graph.AddOrUpdateNode(1, 1) |> ignore
    Assert.Equal(1, graph.GetValue(1))


[<Fact>]
let ``Can add a node with dependencies to the graph`` () =
    let graph = DependencyGraph()
    graph.AddOrUpdateNode(1, 1) 
        .AddDependentNode(2, fun deps -> deps |> Seq.sum |> (+) 1)
        .AddDependentNode(3, fun deps -> deps |> Seq.sum |> (+) 1) |> ignore
    graph.AddOrUpdateNode(4, [1; 3], fun deps -> deps |> Seq.sum |> (+) 1) |> ignore
    Assert.Equal(2, graph.GetValue(2))
    Assert.Equal(3, graph.GetValue(3))
    Assert.Equal(5, graph.GetValue(4))


[<Fact>]
let ``Can update a value`` () =
    let graph = DependencyGraph()
    graph.AddOrUpdateNode(1, 1)
        .AddDependentNode(2, fun deps -> deps |> Seq.sum |> (+) 1)
        .AddDependentNode(3, fun deps -> deps |> Seq.sum |> (+) 1) |> ignore
    graph.AddOrUpdateNode(4, [1; 3], fun deps -> deps |> Seq.sum |> (+) 1) |> ignore
    graph.AddOrUpdateNode(1, 2) |> ignore

    // Values were invalidated
    Assert.Equal(None, graph.Debug.Nodes[2].Value)
    Assert.Equal(None, graph.Debug.Nodes[3].Value)
    Assert.Equal(None, graph.Debug.Nodes[4].Value)

    Assert.Equal(7, graph.GetValue(4))
    Assert.Equal(Some 3, graph.Debug.Nodes[2].Value)
    Assert.Equal(Some 4, graph.Debug.Nodes[3].Value)
    Assert.Equal(Some 7, graph.Debug.Nodes[4].Value)


[<Fact>]
let ``Dependencies are ordered`` () =
    
    let graph = DependencyGraph()
    
    let input = [1..100]
    
    graph.AddList(seq { for x in input -> (x, [x]) }).AddDependentNode(101, fun deps -> deps |> Seq.collect id |> Seq.toList) |> ignore 

    Assert.Equal<int list>(input, graph.GetValue(101))

    graph.AddOrUpdateNode(35, [42]) |> ignore

    let expectedResult = input |> List.map (fun x -> if x = 35 then 42 else x)
    Assert.Equal<int list>(expectedResult, graph.GetValue(101))


