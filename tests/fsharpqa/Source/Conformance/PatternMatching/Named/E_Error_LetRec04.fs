// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expect span="(4,9-4,29)" status="error" id="FS0001">This expression was expected to have type.    ''a option'    .but here has type.    'string'</Expect>
let rec (|Foo3|_|) (a:int) x = "BAD DOG!"

