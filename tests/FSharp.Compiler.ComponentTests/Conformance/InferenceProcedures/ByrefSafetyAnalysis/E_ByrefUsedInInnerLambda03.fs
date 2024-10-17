// #Regression #Conformance #TypeInference #ByRef 
// Verify that byref values may not escape into inner lambdas.
// (Disallowed by the CLR.)
// This test is currently emitting 2 copies of the same error message due to a known bug


let testFunction() =
    let mutable x = 0
    let byrefValue = &x
    
    let nestedLambda = function (x : int) -> x + byrefValue
    
    nestedLambda 42
