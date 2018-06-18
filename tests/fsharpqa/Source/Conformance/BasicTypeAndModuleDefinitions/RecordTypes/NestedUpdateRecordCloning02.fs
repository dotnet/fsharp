// #Conformance #TypesAndModules #Records
#light

// Verify cloning and updating of fields accessed through TypeName using nested copy and update syntax

type AnotherNestedRecTy = { A : int; }

type NestdRecTy = { B : string; C : AnotherNestedRecTy; }

type RecTy = { D : NestdRecTy; E : string option; }

let t1 = { D = { B = "t1"; C = { A = 1; } }; E = None; }

// TypeName.FieldName access
let t2 = { t1 with RecTy.D.B = "t2"; }

let t3 = { t2 with RecTy.D.B = "t3"; RecTy.D.C.A = 3; }

// Changed Fields t1 to t2
if t1.D.B <> "t1" || t2.D.B <> "t2" then exit 1

// Fields Cloned t1 to t2
if t2.D.C.A <> t1.D.C.A || t2.E <> t1.E then exit 1

// Changed Fields t2 to t3
if t3.D.B <> "t3" || t2.D.C.A <> 1 || t3.D.C.A <> 3 then exit 1

// Fields Cloned t2 to t3
if t3.E <> t2.E then exit 1

exit 0