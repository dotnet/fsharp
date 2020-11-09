// #Regression #Conformance #DataExpressions 
// Verify that passing unresolvable symbol name results with compilation error.
//<Expects id="FS0039" span="(5,43)" status="error">The value, constructor, namespace or type 'Unknown' is not defined.</Expects>

let b = nameof System.Collections.Generic.Unknown<int>
exit 0
