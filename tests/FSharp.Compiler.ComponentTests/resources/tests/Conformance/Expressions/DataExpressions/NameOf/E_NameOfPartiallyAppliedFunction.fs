// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on partially applied functions
//<Expects id="FS3250" span="(6,16)" status="error">Expression does not have a name.</Expects>

let f x y = y * x
let x = nameof(f 2)

exit 0
