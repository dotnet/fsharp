// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2836
//<Expects status="error" span="(6,11-6,14)" id="FS0039">The value or constructor 'lsl' is not defined</Expects>
module M

let _ = 1 lsl 4




