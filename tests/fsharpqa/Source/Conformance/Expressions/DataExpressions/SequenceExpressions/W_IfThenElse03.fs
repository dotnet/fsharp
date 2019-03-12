// #Regression #Conformance #DataExpressions #Sequences 
// Note, implicit yield is enabled because no 'yield' is used







let p = [ if true then printfn "hello"; () ];;
(if p = [ ] then 0 else 1) |> exit
