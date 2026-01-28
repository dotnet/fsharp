// #Regression #Conformance #LexicalAnalysis #Constants 
// Verify error when you specify a sub zero floating point
// number without a zero prefix.
//<Expects id="FS0010" span="(6,9)" status="error">Unexpected symbol '\.' in binding</Expects>

let x = .1

exit 1
