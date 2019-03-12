// #Regression #Conformance #DataExpressions 
// Verify that nameof can't be used as a function.
//<Expects id="FS3216" span="(5,9)" status="error">First-class uses of the 'nameof' operator is not permitted</Expects>

let f = nameof

exit 0
