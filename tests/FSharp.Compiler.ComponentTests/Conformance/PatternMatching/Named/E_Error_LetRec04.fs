// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expects span="(4,9-4,29)" status="error" id="FS0001">This expression was expected to have type.    ''a option'    .but here has type.    'string'</Expects>
let rec (|Foo3|_|) (a:int) x = "BAD DOG!"

