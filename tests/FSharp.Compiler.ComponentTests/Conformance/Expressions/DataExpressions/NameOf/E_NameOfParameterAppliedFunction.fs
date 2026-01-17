// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on applied functions
//<Expects id="FS3250" span="(7,16)" status="error">Expression does not have a name.</Expects>

let f x y = x y
let z x = 1 * x
let b = nameof(f z 1)

exit 0
