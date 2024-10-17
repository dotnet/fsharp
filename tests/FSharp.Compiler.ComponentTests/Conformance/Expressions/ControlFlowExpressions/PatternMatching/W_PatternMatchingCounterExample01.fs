// #Regression #Conformance #ControlFlow 
// Regression test for FSHARP1.0:1986 (conter example in complex pattern matching)


#light

let f = function
                            // Incomplete pattern matches on this expression. For example, the value 'Some ((_,true))' will not be matched
  | Some (true,false) -> 1
  | Some (false, _) -> 2
  | None -> 3
