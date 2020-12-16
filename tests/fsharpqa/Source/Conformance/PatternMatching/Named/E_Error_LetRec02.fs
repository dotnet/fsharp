// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expects span="(4,9-4,34)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
let rec (|Foo2|Bar2|_|) (a:int) x = "BAD DOG!"
