// #Conformance #TypesAndModules #Records 
// Verify that record fields may be marked mutable
// In this case, only one field is marked as mutable
//<Expects status="success"></Expects>
#light

[<Measure>] type Kg

type T1 = { mutable A : float<Kg> ; B : decimal<Kg> }
type T2 = { A : float ; mutable B : decimal }

let r1 : T1 =  { A = 1.0<Kg>; B = 1.0M<_> }
let r2 : T2 =  { A = 1.0<_>;  B = 1.0M<_> }

r1.A <- r1.A + 1.0<_>
r2.B <- r2.B + 1M<_>

(if not (r1.A = 2.0<_> && r2.B = 2.M) then failwith "Failed")
