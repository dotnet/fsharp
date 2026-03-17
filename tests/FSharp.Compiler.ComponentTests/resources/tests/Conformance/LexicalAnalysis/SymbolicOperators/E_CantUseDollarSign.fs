// #Regression #Conformance #LexicalAnalysis #Operators 
// Verify the use of the dollar sign in symbolic functions is deprecated
// Related to FSB 1684

//<Expects id="FS0035" status="error">This construct is deprecated: '\$' is not permitted as a character in operator names and is reserved for future use$</Expects>

let (<=$=>) x y = x + y

exit 0
