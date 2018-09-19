// #Regression #Conformance #TypesAndModules #Records
// Verify same field cannot be declared twice in a nested field update
//<Expects id="FS0668" status="error">The field 'A' appears twice in this record expression or pattern</Expects>
#light

type AnotherNestedRecTy = { A : int; }

type NestdRecTy = { B : string; C : AnotherNestedRecTy; }

type RecTy = { D : NestdRecTy; E : string option; }

let t1 = { D = { B = "t1"; C = { A = 1; } }; E = None; }

let t2 = { t1 with D.C.A = 3; D.C.A = 2}