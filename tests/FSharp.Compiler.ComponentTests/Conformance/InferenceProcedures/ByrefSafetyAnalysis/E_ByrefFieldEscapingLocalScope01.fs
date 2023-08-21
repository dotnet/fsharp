// #Regression #Conformance #TypeInference #ByRef 
open System

let testFunction() =
    let mutable local = 42
    let span = Span<int>(&local)
    span

// Code shouldn't compile
exit 1
