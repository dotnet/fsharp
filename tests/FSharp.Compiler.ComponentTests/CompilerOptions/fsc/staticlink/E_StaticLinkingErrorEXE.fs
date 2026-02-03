// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:5133
// --staticlink emits a reasonable error message when things go wrong (e.g. when F# compiler has optimized away the dependencies on the target DLL
// From Don's note: "This can only happen if the target code is pure (such as the "let a = 1" binding). "
//<Expects id="FS2012" status="error">Assembly 'E_StaticLinkingErrorDLL' not found in dependency set of target binary\. Statically linked roots should be specified using an assembly name, without a DLL or EXE extension\. If this assembly was referenced explicitly then it is possible the assembly was not actually required by the generated binary, in which case it should not be statically linked\.$</Expects>
let b = Test.Module1.a
