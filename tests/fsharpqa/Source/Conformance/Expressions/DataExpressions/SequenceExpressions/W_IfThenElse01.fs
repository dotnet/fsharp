// #Regression #Conformance #DataExpressions #Sequences 
// Note, implicit yield is enabled because no 'yield' is used







let p = [ if true then 1 else 2 ]
(if p = [ 1 ] then 0 else 1) |> exit
