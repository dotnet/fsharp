// #Conformance #TypesAndModules #Records 
#light

// Verify the ability to clone records using a simplified syntax

type RecType = { A : int; B : string; C : float; D : RecType option }

let t1 = { A = 1; B = "t1"; C = 3.14; D = None }

let t2 = { t1 with A = 2 }
let t3 = { t2 with B = "t3" }
let t4 = { t3 with C = 4.4; D = Some(t1) }

// Changed field
if t1.A <> 1 || t2.A <> 2 then failwith "Failed"
// The rest were cloned from t1
if t1.B <> t2.B || t1.C <> t2.C || t1.D <> t2.D then failwith "Failed"

if t3.B <> "t3" then failwith "Failed"
// The rest were cloned from t2
if t3.A <> t2.A || t3.C <> t2.C || t3.D <> t2.D then failwith "Failed"

if t4.C <> 4.4 || t4.D <> Some(t1) then failwith "Failed"
if t4.A <> t3.A || t4.B <> t3.B then failwith "Failed"
