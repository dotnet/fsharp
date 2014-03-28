// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:5465
// Title: problem unifying types which implement generic interfaces at multiple instantiations
//<Expects status="success"></Expects>

type D =
    member __.M0(a:string, n:C)= ()
    member __.M1(a:string) (n:C)= ()

let M (d:D, c:C) = d.M0("aa",c)
let N (d:D, c:C) = d.M1 "aa" c           // Used to give error: Type constraint mismatch. The type   'a is not compatible with type  C The type 'string' does not match the type 'int'

