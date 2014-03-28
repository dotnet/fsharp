// #Regression #Diagnostics 
// Regression test for FSHARP1.0:6135

//<Expects status="error" span="(7,1-7,8)" id="FS0027">This value is not mutable$</Expects>

let  x = null
x <- ""
