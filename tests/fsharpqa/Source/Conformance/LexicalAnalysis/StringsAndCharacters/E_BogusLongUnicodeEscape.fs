// #Regression #Conformance #LexicalAnalysis 
#light

// Verify error with malformed long unicode escape sequences

let tooBigForChar  = '\U00024B62'
let bogusString01 = "\U00110000"
let bogusString02 = "\UFFFF0000"

exit 1

//<Expects id="FS1159" span="(6,22-6,34)" status="error">This Unicode encoding is only valid in string literals</Expects>
//<Expects id="FS1245" span="(7,22-7,32)" status="error">\\U00110000 is not a valid Unicode character escape sequence</Expects>
//<Expects id="FS1245" span="(8,22-8,32)" status="error">\\UFFFF0000 is not a valid Unicode character escape sequence</Expects>
