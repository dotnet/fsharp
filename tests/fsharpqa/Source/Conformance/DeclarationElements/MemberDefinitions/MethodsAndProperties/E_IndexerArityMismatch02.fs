// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:1407 (Poor error message when arguments do not match signature of overloaded operator. Also incorrect span.)

//<Expects id="FS0001" span="(11,24-11,35)" status="error">This expression was expected to have type.    ''a\[,,\]'    .but here has type.    'int\[\]'</Expects>
//<Expects id="FS0001" span="(12,25-12,32)" status="error">This expression was expected to have type.    ''a\[\]'    .but here has type.    'int\[,\]'</Expects>
//<Expects id="FS0001" span="(13,27-13,38)" status="error">This expression was expected to have type.    ''a\[,,\]'    .but here has type.    'int\[,,,\]'</Expects>
//<Expects id="FS0001" span="(14,27-14,36)" status="error">This expression was expected to have type.    ''a\[,\]'    .but here has type.    'int\[,,,]'</Expects>

let foo (arr : int[,]) = arr.[1,2] // ok

let b1 (arr : int[]) = arr.[1,2,3]
let b2 (arr : int[,]) = arr.[1]
let b3 (arr : int[,,,]) = arr.[1,2,3]
let b4 (arr : int[,,,]) = arr.[1,2]





