// #Regression #Diagnostics 
// Regression test for FSHARP1.0:5449
// Make sure that error spans correctly across multiple lines
//<Expects status="error" id="FS0003" span="(8,1-9,4)">This value is of type 'int', which is not a function type. A value is being passed to it as an argument as if it were a function.$</Expects>

let f x = 1

f
  1
  3
