// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4527
//<Expect status=success></Expect>

let p = [ (if true then 1 else 2) ]
(if p = [ 1 ] then 0 else 1) |> exit
