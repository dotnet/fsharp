// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expect span="(4,5-4,18)" status="error" id="FS0001">This expression was expected to have type.    'Choice<'a,'b>'    .but here has type.    'string'</Expect>
let (|Foo|Bar|) x = "BAD DOG!"

