// #Regression #Conformance #ControlFlow 
// Regression test for FSHARP1.0:1986 (counter example in complex pattern matching)
//<Expects id="FS0025" span="(5,9-5,17)" status="warning">Incomplete pattern matches on this expression</Expects>

let h = function
  | [||] -> 0
  | [|_|] -> 1
  | [|_;_|] -> 3

()
