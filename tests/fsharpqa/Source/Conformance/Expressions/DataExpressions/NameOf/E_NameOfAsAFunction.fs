// #Regression #Conformance #DataExpressions 
// Verify that nameof can't be used as a function.
//<Expects id="FS3197" span="(5,9)" status="error">This expression does not have a name.</Expects>

let f = nameof

exit 0
