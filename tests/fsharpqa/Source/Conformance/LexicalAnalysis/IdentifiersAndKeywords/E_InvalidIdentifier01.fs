// #Regression #Conformance #LexicalAnalysis 
#light

// Test invalid identifiers
//<Expects id="FS1156" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>

let 0leaderingzero = false

exit 1
