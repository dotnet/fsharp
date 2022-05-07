// #NoMono #NoMT #CodeGen #EmittedIL #Attributes 
// EXPECTED behavior: compile to an assembly where "Method" has custom attribute Microsoft.FSharp.Core.CompilationSourceNameAttribute with value "Method"
module Program

type T =
          [<CompiledName("SomeCompiledName")>]
          member a.Method(x,y) = x + y

