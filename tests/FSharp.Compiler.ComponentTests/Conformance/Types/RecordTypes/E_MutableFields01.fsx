// #Regression #Conformance #TypesAndModules #Records 
// Verify that record fields may be marked mutable
// Making one field mutable does not make _all_ the fields mutable


// This is also regression test for FSHARP1.0:3733
#light

[<Measure>] type Kg

type T1 = { mutable A : float<Kg> ; B : decimal<Kg> }
type T2 = { A : float ; mutable B : decimal }

let r1 : T1 =  { A = 1.0<Kg>; B = 1.0M<_> }
let r2 : T2 =  { A = 1.0<_>;  B = 1.0M<_> }

r1.B <- r1.A + 1.0<_>
r2.A <- r2.B + 1M<_>
