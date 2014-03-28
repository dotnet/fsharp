// #Regression #Conformance #LexicalAnalysis 
#light

// Verify warning when using a reserved identifier

//<Expects id="FS0046" status="warning">The identifier 'atomic' is reserved for future use by F#</Expects>

let atomic = 1

exit 0
