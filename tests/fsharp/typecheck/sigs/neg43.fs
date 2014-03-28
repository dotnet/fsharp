namespace Foo
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type DU = 
 | A
 | B of string
 with
   override x.ToString() = "x"

namespace Foo2
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type DU = 
 | A
 | B of string
 with
   override x.ToString() = "x"