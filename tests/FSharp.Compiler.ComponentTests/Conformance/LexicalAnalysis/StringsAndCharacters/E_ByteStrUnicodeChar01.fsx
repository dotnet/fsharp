// #Regression #Conformance #LexicalAnalysis 
#light

// Verify error when trying to take the byte string of 
// a string literal with unicode characters

//<Expects id="FS1140" status="error">This byte array literal contains 1 characters that do not encode as a single byte</Expects>

let _ = "\u2660"B

exit 1
