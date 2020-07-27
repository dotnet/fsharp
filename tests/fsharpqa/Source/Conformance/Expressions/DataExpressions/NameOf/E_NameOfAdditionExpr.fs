// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on const string
//<Expects id="FS3250" span="(6,16)" status="error">Expression does not have a name.</Expects>
let x = nameof(1+2)

exit 0
