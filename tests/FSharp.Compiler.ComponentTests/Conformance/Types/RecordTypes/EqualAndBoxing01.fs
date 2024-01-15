// #Regression #Conformance #TypesAndModules #Records 
// Regression test for FSHARP1.0:5535
// Record type
type R = { A : int; B : char }

let r1 = { A = 1; B = 'a' }
let r2 = { A = 2; B = 'b' }

let t1 = r1.Equals(box r2)        // expect: false, no exception!
let t2 = r1.Equals(box r1)        // expect: true, no exception!

(if t1 && t2 then failwith "Failed")
