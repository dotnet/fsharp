// #Regression #Conformance #ObjectOrientedTypes #Classes #ObjectConstructors 
// Regression test for FSHARP1.0:4212

module M
// implicit syntax
type Foo1 [<System.Obsolete("Message1")>] () =
  member x.Bar() = 1

let _ = Foo1()
