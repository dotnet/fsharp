// #Regression #Libraries #Operators 
// Regression test for FSHARP1.0:3470 - exception on abs of native integer
//<Expects status="error" span="(11,6-11,9)" id="FS0001">The type 'byte' does not support the operator 'Abs'$</Expects>
//<Expects status="error" span="(12,6-12,9)" id="FS0001">The type 'uint16' does not support the operator 'Abs'$</Expects>
//<Expects status="error" span="(13,6-13,9)" id="FS0001">The type 'uint32' does not support the operator 'Abs'$</Expects>
//<Expects status="error" span="(14,6-14,8)" id="FS0001">The type 'uint32' does not support the operator 'Abs'$</Expects>
//<Expects status="error" span="(15,6-15,9)" id="FS0001">The type 'unativeint' does not support the operator 'Abs'$</Expects>
//<Expects status="error" span="(16,6-16,9)" id="FS0001">The type 'uint64' does not support the operator 'Abs'$</Expects>
//<Expects status="error" span="(17,6-17,9)" id="FS0001">The type 'uint64' does not support the operator 'Abs'$</Expects>

abs -1uy   // byte
abs -1us   // uint16
abs -1ul   // uint32
abs -1u    // uint32
abs -1un   // unativeint
abs -1uL   // uint64
abs -1UL   // uint64
