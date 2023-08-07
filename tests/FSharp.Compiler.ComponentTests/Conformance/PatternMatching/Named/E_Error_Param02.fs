// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expects span="(4,5-4,31)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
let (|Foo2|Bar2|_|) (a:int)  x = "BAD DOG!"   // expect: invalid name for an active pattern
