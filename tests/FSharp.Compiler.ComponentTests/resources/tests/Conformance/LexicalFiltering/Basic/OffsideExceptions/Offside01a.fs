// #Regression #Conformance #LexFilter #Exceptions 
// Regression test for FSHARP1.0:5205
// Indentation rules
//<Expects status="success"></Expects>

let faaa xx = List.iter (fun z -> 
     match 1 with
     | _ -> ()) [1;2]
