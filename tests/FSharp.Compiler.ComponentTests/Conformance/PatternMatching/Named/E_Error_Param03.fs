// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expects span="(4,5-4,38)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
let (|Foo2b|Bar2b|Baz2b|_|) (a:int) x = "BAD DOG!"   //  expect: type string doesn't match type 'choice'
