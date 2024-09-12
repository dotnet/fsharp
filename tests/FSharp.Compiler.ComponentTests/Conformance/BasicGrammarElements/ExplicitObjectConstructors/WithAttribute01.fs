// #Regression #Conformance #ObjectOrientedTypes #Classes #ObjectConstructors 
// Regression test for FSHARP1.0:4212
// Attribute is placed on both the explicit and the implicit constructors



module M
// explicit syntax
type Foo [<System.Obsolete("Message2")>] (x:int) =
   [<System.Obsolete("Message3")>]
   new() = Foo(1)

let _ = Foo()
let _ = Foo(3)
