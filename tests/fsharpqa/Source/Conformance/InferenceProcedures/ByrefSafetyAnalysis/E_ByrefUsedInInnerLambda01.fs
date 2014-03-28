// #Regression #Conformance #TypeInference #ByRef 
// Verify that byref values may not escape into inner lambdas.
// (Disallowed by the CLR.)

//<Expects id="FS0406" span="(12,34-12,48)" status="error">The byref-typed variable 'byrefValue' is used in an invalid way\. Byrefs cannot be captured by closures or passed to inner functions\.$</Expects>

let testFunction() =
    let mutable x = 0
    let byrefValue = &x
    
    // If you simply use a byrev value, F# automatically dereferences it
    let nestedLambda (x : int) = byrefValue + x
    
    nestedLambda 42

// Code shouldn't compile    
exit 1
