// #Regression #Conformance #LexicalAnalysis #Constants 


// Verify compile error for unsigned literals which are MaxSize + 1, MaxSize - 1
// All of these should cause compiler errors

//<Expects status="error" span="(12,18)" id="FS0001">The type 'byte' does not support the operator '~-'$</Expects>
//<Expects status="error" span="(14,18)" id="FS0001">The type 'uint16' does not support the operator '~-'$</Expects>
//<Expects status="error" span="(16,18)" id="FS0001">The type 'uint32' does not support the operator '~-'$</Expects>
//<Expects status="error" span="(18,18)" id="FS0001">The type 'uint64' does not support the operator '~-'$</Expects>

let minUByte  = -1uy

let minUInt16 = -1us

let minUInt32 = -1u

let minUInt64 = -1UL

exit 1
