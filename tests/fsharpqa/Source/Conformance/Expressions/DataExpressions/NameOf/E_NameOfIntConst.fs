// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on const int
//<Expects id="FS3199" span="(5,9)" status="error">This expression does not have a name.</Expects>

let x = nameof 1

exit 0
