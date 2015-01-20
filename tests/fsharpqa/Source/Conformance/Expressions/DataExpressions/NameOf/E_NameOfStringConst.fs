// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on const string
//<Expects id="FS3182" span="(5,9)" status="error">This expression does not have a name.</Expects>

let x = nameof "string"

exit 0