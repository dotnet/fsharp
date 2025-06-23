// #Regression #Conformance #LexicalAnalysis 
#light

// Verify error when trying to take the byte value of 
// a unicode character literal.

//<Expects id="FS1157" status="error">This is not a valid byte character literal. The value must be less than or equal to '\\127'B.</Expects>

let _ = '\u2660'B

exit 1
