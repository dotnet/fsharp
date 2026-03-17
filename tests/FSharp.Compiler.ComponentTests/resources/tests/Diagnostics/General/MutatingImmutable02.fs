// #Regression #Diagnostics 
// Regression test for FSHARP1.0:6135

//<Expects status="error" span="(7,1-7,8)" id="FS0027">This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable x = expression'.</Expects>

let  x = null
x <- ""
