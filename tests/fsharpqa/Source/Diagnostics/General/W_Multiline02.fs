// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3596
// Make sure that error spans correctly across multiple lines
//<Expects status="warning" id="FS0020" span="(8,1-10,18)">The result of this expression has type 'int' and is implicitly ignored</Expects>
#nowarn "988"
let f g x = g x

f (fun x ->

             x) 3
