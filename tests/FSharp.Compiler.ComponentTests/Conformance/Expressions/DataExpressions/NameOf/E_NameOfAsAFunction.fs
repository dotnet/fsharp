// #Regression #Conformance #DataExpressions 
// Verify that nameof can't be used as a function.
//<Expects id="FS3251" span="(5,9)" status="error">Using the 'nameof' operator as a first-class function value is not permitted</Expects>

let f = nameof

exit 0
