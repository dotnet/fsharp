// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on applied functions
//<Expects id="FS3199" span="(6,9)" status="error">This expression does not have a name.</Expects>

let f() = 1
let x = nameof(f())

exit 0