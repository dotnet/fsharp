// #Regression #Conformance #TypeInference #ByRef 
// Verify that byref values may not escape into inner lambdas.
// (Disallowed by the CLR.)
// This test is currently emitting 2 copies of the same error message due to a known bug
//<Expects id="FS0406" span="(11,24-11,60)" status="error">The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions\.$</Expects>

let testFunction() =
    let mutable x = 0
    let byrefValue = &x
    
    let nestedLambda = function (x : int) -> x + byrefValue
    
    nestedLambda 42
