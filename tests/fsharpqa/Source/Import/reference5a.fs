// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:3200
// Make sure we can import types defined in a module inside a namespace
module reference5a
open N.M
let foo : T = ""

exit 0
