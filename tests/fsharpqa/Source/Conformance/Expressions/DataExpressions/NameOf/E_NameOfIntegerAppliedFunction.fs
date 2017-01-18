// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on applied functions
//<Expects id="FS3197" span="(6,9)" status="error">This expression does not have a name.</Expects>

let f x = 1 * x
let x = nameof(f 2)

exit 0
