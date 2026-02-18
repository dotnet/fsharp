// #Conformance #TypeInference #ByRef 
open System

// Scenario 3: Match expression returning Span from local byrefs
let testMatch(b: bool) =
    let mutable x = 1
    let mutable y = 2
    let _s = 
        match b with
        | true -> Span<int>(&x) // Should error - cannot escape local byref to Span
        | false -> Span<int>(&y) // Should error - cannot escape local byref to Span
    ()

testMatch(true)
