// #Regression #Conformance #LexicalAnalysis 
#light

// Verify error when trying to access U+DFFF in string

//<Expects id="FS1245" status="error">This is not a valid UTF-16 literal</Expects>

let _ = "\uDFFF"

exit 1
