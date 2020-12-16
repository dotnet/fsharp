// #Regression #Conformance #ObjectOrientedTypes #Classes #ObjectConstructors 
// Regression test for FSHARP1.0:4212
// Attribute is placed on both the explicit and the implicit constructors
//<Expects id="FS0044" span="(11,12-11,18)" status="warning">Message2</Expects>
//<Expects id="FS0044" span="(13,9-13,14)" status="warning">Message3</Expects>
//<Expects id="FS0044" span="(14,9-14,15)" status="warning">Message2</Expects>
module M
// explicit syntax
type Foo [<System.Obsolete("Message2")>] (x:int) =
   [<System.Obsolete("Message3")>]
   new() = Foo(1)

let _ = Foo()
let _ = Foo(3)
