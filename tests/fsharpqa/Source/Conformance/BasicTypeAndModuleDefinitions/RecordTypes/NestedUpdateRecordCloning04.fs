// #Conformance #TypesAndModules #Records
#light

// Verify cloning and updating of fields accessed through ModuleName using nested copy and update syntax
module Test =
    module M =
        type AnotherNestedRecTy = { A : int; }

        type NestdRecTy = { B : string; C : AnotherNestedRecTy; }

        type RecTy = { D : NestdRecTy; E : string option; }


    let t1 = { M.RecTy.D = { M.B = "t1"; M.C = { M.A = 1; } }; M.E = None; }

    // Module.FieldName access
    let t2 = { t1 with M.D.B = "t2"; M.D.C.A = 2; }

    // Module.TypeName.FieldName access
    let t3 = { t1 with M.RecTy.E = Some "t3"; M.RecTy.D.B = "t3"; }

    // Changed Fields t1 to t2
    if t1.D.B <> "t1" || t2.D.B <> "t2" || t2.D.C.A <> 2 || t1.D.C.A <> 1 then exit 1

    // Fields Cloned t1 to t2
    if t2.E <> t1.E then exit 1

    // Changed Fields t2 to t3
    if t2.E <> None || t3.E <> Some "t3" || t2.D.B <> "t3" then exit 1

    // Fields Cloned t2 to t3
    if t3.D.C <> t2.D.C then exit 1

    exit 0