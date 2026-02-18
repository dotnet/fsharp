// #Conformance #TypeInference #ByRef 
open System

// Scenario 2: Indexer returning Span capturing byref arg
type IndexerTest() =
    member this.Item with get(x: byref<int>) = Span<int>(&x)

let testIndexer() =
    let ind = IndexerTest()
    let mutable x = 1
    let _s = ind[&x] // Should error - cannot escape local byref to Span
    ()

testIndexer()
