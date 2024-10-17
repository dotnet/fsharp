// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:4160
// ICE when trying to compile code with abstract properties
// See also FSHARP1.0:3661 and 4981

type T =
   abstract A : int
   override x.A with get() = 1
   member x.A with set(v) = ()
