// #Regression #NoMT #CompilerOptions #NoMono 
// Regression test for FSharp1.0:5189
// Title: Make missing transitive references with --standalone a warning instead of an error
// Note: A reference to PresentationFramework.dll is needed for compilation.
//<Expects status="warning" id="FS2011">Assembly 'PresentationUI' was referenced transitively and the assembly could not be resolved automatically\. Static linking will assume this DLL has no dependencies on the F# library or other statically linked DLLs\. Consider adding an explicit reference to this DLL\.$</Expects>

module M
let a : System.Windows.Controls.TextBox = null


