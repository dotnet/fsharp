// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
//<Expects span="(5,9-5,30)" status="error" id="FS0001">This expression was expected to have type.    'Choice<'a,'b>'    .but here has type.    'string'</Expects>

let rec (|Foo|Bar|) (a:int) x = "BAD DOG!"

exit 1

