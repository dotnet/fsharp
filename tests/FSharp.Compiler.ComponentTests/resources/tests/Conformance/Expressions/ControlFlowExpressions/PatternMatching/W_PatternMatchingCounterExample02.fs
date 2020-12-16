// #Regression #Conformance #ControlFlow 
// Regression test for FSHARP1.0:1986 (conter example in complex pattern matching)
//<Expects id="FS0025" span="(7,9-7,17)" status="warning">Incomplete pattern matches on this expression\. For example, the value '\[_;true\]' may indicate a case not covered by the pattern\(s\)</Expects>

#light

let g = function
  | [] -> 0
  | [_] -> 1
  | [_; false] -> 3
  | e1::e2::e3::_ -> 2
