// #Conformance #TypesAndModules #Records
#light

// Verify cloning and updating of fields accessed through Module using nested copy and update syntax

    module M =
        type AnotherNestedRecTy = { A : int; }

        type NestdRecTy = { B : string; C : AnotherNestedRecTy; }

        type RecTy = { D : NestdRecTy; E : string option; }



let t1 = { M.RecTy.D = { M.B = "t1"; M.C = { M.A = 1; } }; M.E = None; }

// Module.FieldName access
let t2 = { t1 with M.D.B = "t2"; M.D.C.A = 2; }

// Module.TypeName.FieldName access
let t3 = { t1 with M.RecTy.E = Some "t3"; M.RecTy.D.B = "t3"; }

// Changed Fields
if t1.D.B <> "t1" || t2.D.B <> "t2" || t3.D.B <> "t3" || t4.D.B <> "t4" then exit 1

if t2.D.C.A <> 2 then exit 1

if t2
// Fields Cloned t1 to t2
if t1.D.C <> t2.D.C || t1.E <> t2.E then exit 1

// Fields Cloned t2 to t3
if t2.D.C <> t3.D.C || t2.E <> t3.E then exit 1