// #Conformance #TypeInference #ByRef 
module E_OperatorOverloadSpanCapturingByref
open System

// Scenario 1: Static method returning Span capturing byref
type OpTest() =
    static member Combine(x: byref<int>, y: byref<int>) : Span<int> = Span<int>(&x)

let testOp() : Span<int> =
    let mutable x = 1
    let mutable y = 2
    OpTest.Combine(&x, &y) // Should error - cannot return Span capturing local byref
