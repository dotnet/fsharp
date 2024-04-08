// #Conformance #TypesAndModules #Records 
#light

// By default, Union types implement dispatch slot Equals
// Also minimal test on the expected implementation.
//<Expects status="success"></Expects>

[<Measure>] type Kg

type T1 = { A1 : float<Kg> ; B1 : decimal<Kg> }
type T2 = { A2 : float<Kg> ; B2 : decimal<Kg>; C2 : decimal}

let p =  { A1 = 10.0<Kg>; B1 = 11.0M<Kg> }
let p' = { A1 = 10.0<Kg>; B1 = 12.0M<Kg> }
let q = { A2 = 10.0<Kg>; B2 = 11.0M<Kg>; C2 = 0M }

(if ((p.GetHashCode() <> p'.GetHashCode()) && 
     (p.GetHashCode() <> q.GetHashCode())) then 0 else failwith "Failed")
