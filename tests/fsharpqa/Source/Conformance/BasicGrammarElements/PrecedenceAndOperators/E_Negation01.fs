// #Regression #Conformance #BasicGrammarElements #Operators 
module M

// Verify errors when trying to negate unsigned values
//<Expects status="error" span="(10,31-10,32)" id="FS0001">The type 'byte' does not support the operator '~-'$</Expects>
//<Expects status="error" span="(11,31-11,32)" id="FS0001">The type 'uint16' does not support the operator '~-'$</Expects>
//<Expects status="error" span="(12,31-12,32)" id="FS0001">The type 'uint32' does not support the operator '~-'$</Expects>
//<Expects status="error" span="(13,31-13,32)" id="FS0001">The type 'uint64' does not support the operator '~-'$</Expects>

let negUByte  (x : byte)   = -x
let negUInt16 (x : uint16) = -x
let negUInt32 (x : uint32) = -x
let negUInt64 (x : uint64) = -x

exit 1
