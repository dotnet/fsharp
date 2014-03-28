// #Regression #Conformance #LexicalAnalysis #Constants 
#light

// Verify compile error for unsigned literals which are MaxSize + 1, MaxSize - 1
// All of these should cause compiler errors

//<Expects id="FS1144" status="error">This number is outside the allowable range for 8-bit unsigned integers</Expects>
//<Expects id="FS1146" status="error">This number is outside the allowable range for 16-bit unsigned integers</Expects>
//<Expects id="FS1148" status="error">This number is outside the allowable range for 32-bit unsigned integers</Expects>
//<Expects id="FS1150" status="error">This number is outside the allowable range for 64-bit unsigned integers</Expects>

let maxUByte = 256uy

let maxUInt16 = 65536us

let maxUInt32 = 4294967296u

let maxUInt64 = 18446744073709551616UL

exit 1
