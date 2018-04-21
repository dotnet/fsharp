// #Conformance #TypesAndModules #Records
#light

// Verify cloning and updating of fields using nested copy and update syntax

type AnotherNestedRecTy = { A : int; }

type NestdRecTy = { B : string; C : AnotherNestedRecTy; }

type RecTy = { D : NestdRecTy; E : string option; }

let t1 = { D = { B = "t1"; C = { A = 1; } }; E = None; }

let t2 = { t1 with D.B = "t2" }

let t3 = { t2 with D.C.A = 3 }

// Changed fields t1 to t2
if t1.D.B <> "t1" || t2.D.B <> "t2" then exit 1

// Fields cloned t1 to t2
if t1.E <> t2.E || t1.D.C <> t2.D.C then exit 1

// Changed fields t2 to t3
if t2.D.C.A <> 1 || t3.D.C.A <> 3 then exit 1

// Fields cloned t2 to t3
if t2.E <> t3.E || t2.D.B <> t3.D.B then exit 1

exit 0
