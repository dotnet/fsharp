// #Regression #Conformance #TypeInference #ByRef 
// Verify that byref values may not escape into inner lambdas.
// (Disallowed by the CLR.)



let testFunction() =
    let mutable x = 0
    let byrefValue = &x
    
    let nestedLambda = fun (x : int) -> x + byrefValue
    
    nestedLambda 42

// Code shouldn't compile    
exit 1
