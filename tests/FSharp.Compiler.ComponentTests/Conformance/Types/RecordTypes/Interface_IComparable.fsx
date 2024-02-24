// #Conformance #TypesAndModules #Records 
// By default, record types implement IComparable
//<Expects status="success"></Expects>
 
#light

[<Measure>] type Kg

type I = { A : float<Kg> ; B : decimal<Kg> }

let p = { A = 10.0<Kg>; B = 11.0M<Kg> }
let q = { A = 11.0<Kg>; B = 12.0M<Kg> }

let _ = match box p with
        | :? System.IComparable -> true
        | _ -> false
        

// This is OK
let i1 = (box p)

// This is OK too
let i2 = p :> System.IComparable

let t1 = match i1 with
         | :? System.IComparable -> true
         | _ -> false

let t2 = i2.CompareTo(p) = 0        // true

let t3 = i2.CompareTo(q) <> 0       // true

(if t1 && t2 && t3 then 0 else failwith "Failed")
