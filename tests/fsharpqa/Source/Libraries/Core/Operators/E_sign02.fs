// #Regression #Libraries #Operators 
// Test sign function on unsigned primitives, should get error.

//<Expects status="error" span="(9,9-9,12)" id="FS0001">The type 'byte' does not support the operator 'get_Sign'$</Expects>
//<Expects status="error" span="(10,9-10,12)" id="FS0001">The type 'uint16' does not support the operator 'get_Sign'$</Expects>
//<Expects status="error" span="(11,9-11,11)" id="FS0001">The type 'uint32' does not support the operator 'get_Sign'$</Expects>
//<Expects status="error" span="(12,9-12,12)" id="FS0001">The type 'uint64' does not support the operator 'get_Sign'$</Expects>

if sign 0uy   <> 0 then exit 1  // byte
if sign 0us   <> 0 then exit 1  // int16
if sign 0u    <> 0 then exit 1  // int32
if sign 0uL   <> 0 then exit 1  // int64
