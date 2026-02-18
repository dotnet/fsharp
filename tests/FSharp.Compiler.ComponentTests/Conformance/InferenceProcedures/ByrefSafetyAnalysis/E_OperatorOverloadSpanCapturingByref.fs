// #Conformance #TypeInference #ByRef 
open System

// Scenario 1: Operator overloading returning Span capturing byref
type OpTest() =
    static member (+) (x: byref<int>, y: byref<int>) = Span<int>(&x)

let testOp() =
    let mutable x = 1
    let mutable y = 2
    let _s = &x + &y // Should error - cannot escape local byref to Span
    ()
