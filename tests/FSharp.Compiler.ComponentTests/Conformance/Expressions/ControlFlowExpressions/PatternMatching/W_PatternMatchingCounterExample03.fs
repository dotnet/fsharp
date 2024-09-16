// #Regression #Conformance #ControlFlow 
// Regression test for FSHARP1.0:1986 (counter example in complex pattern matching)


let h = function
  | [||] -> 0
  | [|_|] -> 1
  | [|_;_|] -> 3

()
