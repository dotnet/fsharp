// #Conformance #TypesAndModules #Records 
// Record types may include members
// Test both static and instance members
//<Expects status="success"></Expects>
#light

// Instance member
type T1 = { A : int }
          member t.M(a) = t.A

// Static member                           
type T2 = { B : int }
          static member M(b) = b.B

let a = { A = 1 }
let b = { B = 2 }

(if T2.M(b)=2 && a.M(a)=1 then 0 else failwith "Failed")
