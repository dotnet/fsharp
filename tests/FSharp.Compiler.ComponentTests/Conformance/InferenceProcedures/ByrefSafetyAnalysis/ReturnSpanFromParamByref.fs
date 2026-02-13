// #Conformance #TypeInference #ByRef 
module ReturnSpanFromParamByref

open System

let testFunction(x: byref<int>) =
    let span = Span<int>(&x)
    span[0] <- 13
    span
