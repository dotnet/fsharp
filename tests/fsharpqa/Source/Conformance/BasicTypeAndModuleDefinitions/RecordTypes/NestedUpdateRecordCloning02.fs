// #Conformance #TypesAndModules #Records
#light

// Verify cloning and updating of fields accessed through Module or TypeName using nested copy and update syntax
    module M =
        type AnotherNestedRecTy = { A : int; }

        type NestdRecTy = { B : string; C : AnotherNestedRecTy; }

        type RecTy = { D : NestdRecTy; E : string option; }



let t1 = { M.D = { M.B = "t1"; M.C = { M.A = 1; } }; M.E = None; }

// Module.FieldName access
let t2 = { t1 with M.D.B = "t2" }

// Module.TypeName.FieldName access
let t3 = { t2 with M.RecTy.D.B = "t3" }

open M

// TypeName.FieldName access
let t4 = { t3 with RecTy.D.B = "t4" }

// Changed Fields
if t1.D.B <> "t1" || t2.D.B <> "t2" || t3.D.B <> "t3" || t4.D.B <> "t4" then printfn "AAA"

// Fields Cloned t1 to t2
if t1.D.C <> t2.D.C || t1.E <> t2.E then printfn "AAA"

// Fields Cloned t2 to t3
if t2.D.C <> t3.D.C || t2.E <> t3.E then printfn "AAA"

// Fields Cloned t3 to t4
if t3.D.C <> t4.D.C || t3.E <> t4.E then printfn "AAA"

exit 0