// #Regression #Conformance #DataExpressions #Sequences 
// Note, implicit yield is enabled because no 'yield' is used







let p : unit list = [ if true then printfn "hello"; () ];; // note, both unit-typed expressions interpreted as side-effecting operations
(match p with [ ] -> 0 | _ -> 1) |> exit
