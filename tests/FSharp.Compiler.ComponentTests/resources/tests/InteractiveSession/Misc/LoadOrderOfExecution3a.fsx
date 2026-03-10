// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5427
//<Expects status="success"></Expects>

//<<Output
//let f x = x \+ 1
//let y z = f z
//let w = y 10
//Output

#load "LoadOrderOfExecution2.fsx"
printfn "let w = y 10"
