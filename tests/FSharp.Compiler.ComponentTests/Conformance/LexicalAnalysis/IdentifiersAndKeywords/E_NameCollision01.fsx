// #Regression #Conformance #LexicalAnalysis 
#light

// Verify error when names collide. Typically you just outscope the
// other value, but when the generated names collide we emit an error.

//<Expects id="FS0037" status="error">Duplicate definition of value 'x'</Expects>

let get_x = false

let x = 10

exit 1
