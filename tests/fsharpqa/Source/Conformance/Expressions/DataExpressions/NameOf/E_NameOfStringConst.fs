// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on const string
//<Expects id="FS3250" span="(5,16)" status="error">Expression does not have a name.</Expects>

let x = nameof "string"

exit 0
