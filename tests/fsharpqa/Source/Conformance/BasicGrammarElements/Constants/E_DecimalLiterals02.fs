// #Regression #Conformance #BasicGrammarElements #Constants #NoMono #NETFX20Only 
#light

// Negative Regressiont test for FSharp1.0: 2543 - Decimal literals do not support exponents
//<Expects id="FS1154" span="(7,18-7,26)" status="error">This number is outside the allowable range for decimal literals</Expects>

let invalidDec = 1.0E-50M

exit 1
