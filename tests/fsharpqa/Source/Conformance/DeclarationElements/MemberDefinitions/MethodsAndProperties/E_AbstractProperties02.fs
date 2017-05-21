// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:4160
// ICE when trying to compile code with abstract properties
// See also FSHARP1.0:3661 and 4981
//<Expects id="FS0435" span="(7,13-7,14)" status="error">The property 'A' of type 'T' has a getter and a setter that do not match\. If one is abstract then the other must be as well</Expects>
type T =
   abstract A : int
   override x.A with get() = 1
   member x.A with set(v) = ()
