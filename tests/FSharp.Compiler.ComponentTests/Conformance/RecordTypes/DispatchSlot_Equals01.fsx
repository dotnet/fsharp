// #Conformance #TypesAndModules #Records 
// By default, Union types implement dispatch slot Equals
// Also minimal test on the expected implementation.
//<Expects status="success"></Expects>
#light

[<Measure>] type Kg

type T1 = { A1 : float<Kg> ; B1 : decimal<Kg> }
type T2 = { A2 : float<Kg> ; B2 : decimal<Kg>; C2 : decimal}

let p =  { A1 = 10.0<Kg>; B1 = 11.0M<Kg> }
let p' = { A1 = 10.0<Kg>; B1 = 12.0M<Kg> }
let q = { A2 = 10.0<Kg>; B2 = 11.0M<Kg>; C2 = 0M }

let r1 = p.Equals(p)
let r2 = not (p.Equals(q))
let r3 = not (p.Equals(p'))

let r4 = T1.Equals(p,p)
let r5 = not (T1.Equals(p,p'))
let r6 = not (T1.Equals(p,q))
let r7 = not (T1.Equals(p,null))

(if r1 && r2 && r3 && r4 && r5 && r6 && r7  then 0 else failwith "Failed")
