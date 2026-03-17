// #Regression #Conformance #LexicalAnalysis #Constants 


// Verify compile error for signed literals which are MaxSize + 1, MaxSize - 1
// All of these should cause compiler errors

//<Expects id="FS1142" status="error">This number is outside the allowable range for 8-bit signed integers</Expects>
//<Expects id="FS1145" status="error">This number is outside the allowable range for 16-bit signed integers</Expects>
//<Expects id="FS1147" status="error">This number is outside the allowable range for 32-bit signed integers</Expects>
//<Expects id="FS1149" status="error">This number is outside the allowable range for 64-bit signed integers</Expects>

//<Expects id="FS1142" status="error">This number is outside the allowable range for 8-bit signed integers</Expects>
//<Expects id="FS1145" status="error">This number is outside the allowable range for 16-bit signed integers</Expects>
//<Expects id="FS1147" status="error">This number is outside the allowable range for 32-bit signed integers</Expects>
//<Expects id="FS1149" status="error">This number is outside the allowable range for 64-bit signed integers</Expects>


let minByte = -129y
let maxByte =  128y

let minInt16 = -32769s
let maxInt16 =  32768s

let minInt32 = -2147483649
let maxInt32 =  2147483648

let minInt64 = -9223372036854775809L
let maxInt64 =  9223372036854775808L

