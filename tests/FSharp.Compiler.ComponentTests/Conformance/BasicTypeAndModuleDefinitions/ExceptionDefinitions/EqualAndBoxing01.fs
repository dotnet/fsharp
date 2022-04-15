// #Regression #Conformance #TypesAndModules #Exceptions 
// Regression test for FSHARP1.0:5535
// Exception type
exception E of int

let e1 = E(10)
let e2 = E(11)

let t1 = e1.Equals(box e2)        // expect: false, no exception!
let t2 = e1.Equals(box e1)        // expect: true, no exception!

if t1 && t2 then failwith "Failed: 1"
