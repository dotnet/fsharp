// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4527
//<Expects status="success"></Expects>

let p = [ (if true then 1 else 2) ]
(if p = [ 1 ] then 0 else 1) |> exit
