// #Regression #Conformance #ObjectOrientedTypes #Classes #ObjectConstructors 
// Regression test for FSHARP1.0:4212
//<Expects id="FS0044" span="(9,9-9,15)" status="warning">Message1</Expects>
module M
// implicit syntax
type Foo1 [<System.Obsolete("Message1")>] () =
  member x.Bar() = 1

let _ = Foo1()
