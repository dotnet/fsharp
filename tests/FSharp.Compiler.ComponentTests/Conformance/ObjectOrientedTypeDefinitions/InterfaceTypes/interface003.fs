// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:
//<Expects status="success"></Expects>
type I<'t> =
  abstract M : unit -> int -> 't

type C() = interface I<int> with
            member x.M () f = 99

let pack (i: I<'t>) = i.M () 3  // do not ICE at (9,23)

exit 0
