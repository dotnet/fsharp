// #Conformance #TypesAndModules #Records
#light

// Verify cloning and updating of fields with ambiguities between TypeName and FieldName using nested copy and update syntax

type AnotherNestedRecTy = { A : int; }

type NestdRecTy = { B : string; AnotherNestedRecTy : AnotherNestedRecTy; }

type RecTy = { NestdRecTy : NestdRecTy; E : string option; }


let t1 = { RecTy.NestdRecTy = { B = "t1"; AnotherNestedRecTy = { A = 1; } }; E = None; }

// Ambiguous access
let t2 = { t1 with NestdRecTy.B = "t2" }

let t3 = { t2 with NestdRecTy.AnotherNestedRecTy.A = 3 }

// Changed Fields t1 to t2
if t1.NestdRecTy.B <> "t1" || t2.NestdRecTy.B <> "t2" then exit 1

// Fields Cloned t1 to t2
if t2.E <> t1.E || t2.NestdRecTy.AnotherNestedRecTy.A <> t1.NestdRecTy.AnotherNestedRecTy.A then exit 1

// Changed Fields t2 to t3
if t2.NestdRecTy.AnotherNestedRecTy.A <> 3 then exit 1

// Fields Cloned t2 to t3
if t3.E <> t2.E || t3.NestdRecTy.B <> t2.NestdRecTy.B then exit 1

exit 0