// #Regression #Conformance #TypeInference #ByRef 
// Verify that byref values may not escape into inner lambdas.
// (Disallowed by the CLR.)

//<Expects id="FS0406" span="(11,24-11,55)" status="error">The byref-typed variable 'byrefValue' is used in an invalid way\. Byrefs cannot be captured by closures or passed to inner functions\.$</Expects>
//<Expects id="FS0406" span="(11,24-11,55)" status="error">The byref-typed variable 'byrefValue' is used in an invalid way\. Byrefs cannot be captured by closures or passed to inner functions\.$</Expects>
let testFunction() =
    let mutable x = 0
    let byrefValue = &x
    
    let nestedLambda = fun (x : int) -> x + byrefValue
    
    nestedLambda 42

// Code shouldn't compile    
exit 1
