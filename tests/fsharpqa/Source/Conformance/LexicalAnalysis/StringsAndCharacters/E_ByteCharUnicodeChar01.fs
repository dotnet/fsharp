// #Regression #Conformance #LexicalAnalysis 
#light

// Verify error when trying to take the byte value of 
// a unicode character literal.

//<Expects id="FS1157" status="error">This is not a valid ASCII byte literal. Value must be < 128y.</Expects>

let _ = '\u2660'B

exit 1
