// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4527
//<Expects status="success"></Expects>
let x = 
    [ if true then
          yield! [1]
          if true then 
              yield 2 ]

(if x = [1;2] then 0 else 1) |> exit

