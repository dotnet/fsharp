// #Regression #Conformance #LexicalAnalysis 
#light

// Verify error when trying to access U+D800 in string

//<Expects id="FS1245" status="error">This is not a valid UTF-16 literal</Expects>

let _ = '\uD7FF' // Ok
let _ = "\uE000" // Ok

let _ = "\uD800"

exit 1
