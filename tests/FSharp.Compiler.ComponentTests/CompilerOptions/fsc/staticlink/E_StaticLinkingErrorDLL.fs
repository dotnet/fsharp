// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:5133
// --staticlink emits a reasonable error message when things go wrong (e.g. when F# compiler has optimized away the dependencies on the target DLL
// From Don's note: "This can only happen if the target code is pure (such as the "let a = 1" binding). "

module Test.Module1
let a = 1
