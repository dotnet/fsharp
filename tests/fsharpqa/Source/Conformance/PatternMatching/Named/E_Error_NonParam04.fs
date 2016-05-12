// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expect span="(4,5-4,17)" status="error" id="FS0001">This expression was expected to have type.    ''a option'    .but here has type.    'string'</Expect>
let (|Foo3|_|) x = "BAD DOG!"   //  expect: type string doesn't match type 'option'

