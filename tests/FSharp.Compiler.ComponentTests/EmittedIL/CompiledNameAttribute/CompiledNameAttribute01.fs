// #NoMono #NoMT #CodeGen #EmittedIL #Attributes 
// EXPECTED behavior: compile to an assembly where "f" has custom attribute Microsoft.FSharp.Core.CompilationSourceNameAttribute with value "f"

module Program

[<CompiledName("SomeCompiledName")>]
let f x = x
