// #Regression #Conformance #TypeInference #ByRef 
open System

let testFunction() =
    let mutable x = 0
    let span = Span<int>(&x)
    
    let nestedLambda (x : int) = span[0] + x
    
    nestedLambda 42

// Code shouldn't compile
exit 1