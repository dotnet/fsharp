// #Conformance #TypeInference #ByRef 
open System

let createSpan (x: byref<int>) : Span<int> =
    Span<int>(&x)

let testFunction() =
    let mutable local = 42
    createSpan &local
