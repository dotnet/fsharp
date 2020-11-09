// #Regression #Conformance #PatternMatching 
module TestModule

// Regression testcase for FSharp 1.0: 2070
// Warn on incomplete match with when guard

//<Expects id="FS0025" span="(11,9-11,10)" status="warning">Incomplete pattern matches on this expression\. For example, the value '1' may indicate a case not covered by the pattern\(s\)\. However, a pattern rule with a 'when' clause might successfully match this value</Expects>


let matchWarning s =
  match s with
  | 0 -> true
  | n when n>0 -> false

exit 0

