// #Regression #Conformance #DataExpressions 
// Verify that passing unresolvable symbol name results with compilation error.
//<Expects id="FS0039" span="(5,9)" status="error">The value or constructor 'ff' is not defined.</Expects>

let b = nameof System.Collections.Generic.Listtt<int>
exit 0
