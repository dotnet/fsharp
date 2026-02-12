// #Regression #Conformance #LexicalAnalysis 
// Verify error when an identifier contains an '@' symbol
//<Expects id="FS1104" status="warning">Identifiers containing '@' are reserved for use in F# code generation</Expects>

let ``abc@def`` = 42

exit 0
