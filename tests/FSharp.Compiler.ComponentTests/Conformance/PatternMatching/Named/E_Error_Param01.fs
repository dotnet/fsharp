// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expects span="(4,5-4,26)" status="error" id="FS0001">This expression was expected to have type.    'Choice<'a,'b>'    .but here has type.    'string'</Expects>
let (|Foo|Bar|) (a:int) x = "BAD DOG!"

