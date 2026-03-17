// #Regression #NoMT #CompilerOptions 
// Verify error when using --standalone to reference a file
// which doesn't exist.
// See also FSHARP1.0:5133

//<Expects id="FS2012" status="error">Assembly 'IDontExist\.dll' not found in dependency set of target binary\. Statically linked roots should be specified using an assembly name, without a DLL or EXE extension\. If this assembly was referenced explicitly then it is possible the assembly was not actually required by the generated binary, in which case it should not be statically linked</Expects>

namespace Foo

type Bar() =
   override this.ToString() = "Foo.Bar"
