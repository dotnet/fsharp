// #Regression #Conformance #DeclarationElements #ObjectConstructors 
// Regression test for dev10 bug 840657
// Field "Z" has a default value, and we should not allow it to be set via an object expression
//<Expects id="FS0765" span="(10,10-10,27)" status="error">Extraneous fields have been given values</Expects>
//<Expects id="FS0765" span="(16,11-16,36)" status="error">Extraneous fields have been given values</Expects>
module M

type T = { X: int; [<DefaultValue>] Z : int}
let t = {X = 10}
let t' = { t with Z = 10 }

// with recursive type
type T2 = { A : int; [<DefaultValue>] Z : string}
and U2 = { B : string; [<DefaultValue>] Y : T2}
let u2 = { B = "" }
let u2' = { u2 with Y = { A = 2 } }

exit 1
