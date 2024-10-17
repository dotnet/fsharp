// #Regression #Conformance #PatternMatching 
module TestModule

// Regression testcase for FSharp 1.0: 2070
// Warn on incomplete match with when guard




let matchWarning s =
  match s with
  | 0 -> true
  | n when n>0 -> false

exit 0

