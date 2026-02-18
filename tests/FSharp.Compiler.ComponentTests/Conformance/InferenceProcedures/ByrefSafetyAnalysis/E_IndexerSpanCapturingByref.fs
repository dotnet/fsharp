// #Conformance #TypeInference #ByRef 
module E_IndexerSpanCapturingByref
open System

// Scenario 2: Indexer returning Span capturing byref arg
type IndexerTest() =
    member this.Item with get(x: byref<int>) : Span<int> = Span<int>(&x)

let testIndexer() : Span<int> =
    let ind = IndexerTest()
    let mutable x = 1
    ind[&x] // Should error - cannot return Span capturing local byref
