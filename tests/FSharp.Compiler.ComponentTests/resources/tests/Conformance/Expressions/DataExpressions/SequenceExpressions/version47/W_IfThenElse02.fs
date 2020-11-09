// #Regression #Conformance #DataExpressions #Sequences 
// Note, implicit yield is enabled because no 'yield' is used







let p = [ if false then 1 else printfn "hello"; 3 ]
(if p = [ 3 ] then 0 else 1) |> exit
