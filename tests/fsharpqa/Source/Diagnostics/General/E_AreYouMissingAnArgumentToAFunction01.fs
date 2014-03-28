// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2804
// Make sure we don't emit ?. (notice that the error message changed a bit since the bug was opened)
//<Expects status="error" span="(6,32-6,37)" id="FS0001">Expecting a type supporting the operator 'op_Explicit' but given a function type\. You may be missing an argument to a function\.$</Expects>

let f (x : int list)  = int32 (x>>32)


