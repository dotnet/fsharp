// #Regression #Conformance #TypeInference #ByRef 
// Verify that byref values may not escape into inner lambdas.
// (Disallowed by the CLR.)



let testFunction() =
    let mutable x = 0
    let byrefValue = &x
    
    // If you simply use a byrev value, F# automatically dereferences it
    let nestedLambda (x : int) = byrefValue + x
    
    nestedLambda 42

// Code shouldn't compile    
exit 1
