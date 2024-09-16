// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:1407 (Poor error message when arguments do not match signature of overloaded operator. Also incorrect span.)






let foo (arr : int[,]) = arr.[1,2] // ok

let a1 (arr : int[]) = arr.[1,2]
let a2 (arr : int[,]) = arr.[1]
let a3 (arr : int[,,]) = arr.[(1,2)]
let a4 (arr : int[,,,]) = arr.[(1,2)]
