// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on partially applied functions
//<Expects id="FS3199" span="(6,9)" status="error">This expression does not have a name.</Expects>

let f x y = y * x
let x = nameof(f 2)

exit 0
