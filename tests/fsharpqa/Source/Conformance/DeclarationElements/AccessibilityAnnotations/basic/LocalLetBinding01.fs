// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSHARP1.0:4679
//<Expects status="success"></Expects>
module M
type internal A() = class end

type public B() =
 let foo (x:A) = ()        // this is always private, so this is NOT more accessible than A
 do ()
