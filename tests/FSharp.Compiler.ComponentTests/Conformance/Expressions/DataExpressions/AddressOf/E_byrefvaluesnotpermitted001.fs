// #Regression #Conformance #DataExpressions 
// AddressOf Operator (errors)
// Verify we can the compiler always issue warnings about using & and && operators
//<Expects id="FS0431" span="(9,7-9,8)" status="error">A byref typed value would be stored here\. Top-level let-bound byref values are not permitted</Expects>


module M =
  let mutable x = 10
  let w = &x                    // A byref typed value would be stored here. Top-level let-bound byref values are not permitted.

