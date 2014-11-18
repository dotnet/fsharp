// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on dictionary lookup
//<Expects id="FS3182" span="(5,9)" status="error">This expression does not have a name.</Expects>

let f = nameof

exit 0