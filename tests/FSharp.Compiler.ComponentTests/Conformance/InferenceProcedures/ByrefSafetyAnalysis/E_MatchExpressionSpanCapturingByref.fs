// #Conformance #TypeInference #ByRef 
module E_MatchExpressionSpanCapturingByref
open System

// Scenario 3: Match expression returning Span from local byrefs
let testMatch(b: bool) : Span<int> =
    let mutable x = 1
    let mutable y = 2
    match b with
    | true -> Span<int>(&x) // Should error - cannot return Span capturing local byref
    | false -> Span<int>(&y) // Should error - cannot return Span capturing local byref
