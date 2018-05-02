// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2804
// Make sure we don't emit ?. (notice that the error message changed a bit since the bug was opened)
//<Expects status="error" span="(6,32-6,33)" id="FS0001">This expression was expected to have type.$</Expects>
//<Expects status="error" span="(6,35-6,37)" id="FS0001">This expression was expected to have type.$</Expects>

let f (x : int list)  = int32 (x>>32)


