// #Conformance #TypesAndModules #Unions 
// DU may include members
// Test both static and instance members
//<Expects status="success"></Expects>
#light

// Instance member
type T1 = | C of int * int
          | D of (int * int)
          member x.M(a) = match a with
                          | C(_,_) -> false
                          | D(_) -> true

// Static member                           
type T2 = | C of int * int
          | E of (int * int)
          static member M(b) = match b with
                                 | C(_,_) -> false
                                 | E(_) -> true

let e = E(1,2)
let d = D(1,2)

if T2.M(e) && d.M(d) then 0 else failwith "Failed: 1"
