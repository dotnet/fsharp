// #Regression #Conformance #ObjectOrientedTypes #Classes #ObjectConstructors 
// Regression test for FSHARP1.0:4212
// Attribute is placed on the explicit constructor only

module M
// explicit syntax
type Foo (x:int) =
   [<System.Obsolete("Message1")>]
   new() = Foo(1)

let _ = Foo()
