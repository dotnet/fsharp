// #Regression #Conformance #ControlFlow 
// Regression test for FSHARP1.0:1986 (conter example in complex pattern matching)


#light

let g = function
  | [] -> 0
  | [_] -> 1
  | [_; false] -> 3
  | e1::e2::e3::_ -> 2
