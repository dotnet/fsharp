// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expect span="(4,10-4,43)" status="error" id="FS0827">This is not a valid name for an active pattern</Expect>
let rec  (|Foo2b|Bar2b|Baz2b|_|) (a:int) x = "BAD DOG!" 
