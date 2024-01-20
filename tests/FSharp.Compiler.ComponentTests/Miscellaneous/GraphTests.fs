module Miscellaneous.GraphTests

open Xunit
open FSharp.Compiler.GraphChecking

[<Fact>]
let ``Create graph from sequence`` () =
    let graph =
        Graph.make (
            seq {
                yield (0, Array.empty)
                yield (1, [| 0 |])
                yield (2, [| 0; 1 |])
            }
        )

    Assert.Equal(3, graph.Count)

[<Fact>]
let ``Map graph`` () =
    let graph = Graph.make [| 0, Array.empty; 1, [| 0 |]; 2, [| 0; 1 |] |]
    let mapped = Graph.map string graph
    Assert.True(mapped.ContainsKey("0"))
    let value = mapped["1"]
    Assert.Equal<string array>([| "0" |], value)

[<Fact>]
let ``Calculate transitive graph`` () =
    let graph = Graph.make [ "a", Array.empty; "b", [| "a" |]; "c", [| "b" |] ]
    let transitiveGraph = Graph.transitive graph
    let values = transitiveGraph["c"] |> Set.ofArray
    Assert.Equal<Set<string>>(set [ "a"; "b" ], values)

[<Fact>]
let ``Reverse graph`` () =
    let graph = Graph.make [ "a", Array.empty; "b", [| "a" |] ]
    let reversed = Graph.reverse graph
    let valueA = reversed["a"]
    Assert.Equal<string array>([| "b" |], valueA)
    let valueB = reversed["b"]
    Assert.Equal<string array>(Array.empty, valueB)
