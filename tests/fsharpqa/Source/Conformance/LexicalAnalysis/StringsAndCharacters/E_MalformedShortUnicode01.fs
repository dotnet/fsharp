// #Regression #Conformance #LexicalAnalysis 
#light

// Verify error with malformed short unicode character

// NOTE: I've jazzed up the error messages since they will be interprited as RegExs...
//<Expects id="FS0010" span="(11,18-11,19)" status="error">Unexpected character '\\' in expression. Expected identifier or other token</Expects>
//<Expects id="FS0010" span="(13,18-13,19)" status="error">Unexpected character '\\' in expression. Expected identifier or other token</Expects>
//<Expects id="FS0010" span="(15,17-15,18)" status="error">Unexpected character '\\' in binding</Expects>

let tooShort  = '\u266'

let tooLong   = '\u26600'

let notInChar = \u2660

exit 1
