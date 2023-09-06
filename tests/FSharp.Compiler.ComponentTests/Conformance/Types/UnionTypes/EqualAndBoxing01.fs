// #Regression #Conformance #TypesAndModules #Unions 
// Regression test for FSHARP1.0:5535
// Union type
type U = A | B of int

let r1 = A.Equals(box 1)        // expect: false, no exception!

let r2 = (B 1).Equals(box 1)    // expect: false, no exception!

if not r1 && not r2 then 0 else failwith "Failed: 1"
