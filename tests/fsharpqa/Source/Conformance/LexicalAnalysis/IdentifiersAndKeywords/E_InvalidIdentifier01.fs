// #Regression #Conformance #LexicalAnalysis 
#light

// Test invalid identifiers
//<Expects id="FS1156" status="error">This is not a valid numeric literal. Sample formats include 4, 0x4, 0b0100, 4L, 4UL, 4u, 4s</Expects>

let 0leaderingzero = false

exit 1
