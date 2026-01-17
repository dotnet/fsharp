// #Regression #Conformance #LexicalAnalysis #Constants  
// Verify BigInt uses op_Implicit conversions
// Regression test for FSHARP1.0:4498
// Unsupported conversions - on VS2008/NetFx2.0

//<Expects status="error" span="(17,15-17,18)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'sbyte'$</Expects>
//<Expects status="error" span="(18,15-18,18)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'int16'$</Expects>
//<Expects status="error" span="(23,16-23,19)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'byte'$</Expects>
//<Expects status="error" span="(24,16-24,19)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'uint16'$</Expects>
//<Expects status="error" span="(25,16-25,19)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'uint32'$</Expects>
//<Expects status="error" span="(26,16-26,19)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'uint64'$</Expects>
//<Expects status="error" span="(29,14-29,17)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'char'$</Expects>
//<Expects status="error" span="(34,17-34,20)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'decimal'$</Expects>
//<Expects status="error" span="(35,17-35,20)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'float32'$</Expects>

// Signed
let _ = int8  10I
let _ = int16 10I
//let _ = int32 10I
//let _ = int64 10I

// Unsigned
let _ = uint8  10I
let _ = uint16 10I
let _ = uint32 10I
let _ = uint64 10I

// Char and string
let _ = char 10I
//let _ = string 10I

//float and decimal
//let _ = float 10I
let _ = decimal 10I
let _ = float32 10I
