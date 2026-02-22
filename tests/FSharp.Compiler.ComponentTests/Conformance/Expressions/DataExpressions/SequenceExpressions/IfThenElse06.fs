// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4527
//<Expects status="success"></Expects>
let y = 
    [ if true then yield! [1;2] else () ]
(if y = [1;2] then 0 else 1) |> exit
