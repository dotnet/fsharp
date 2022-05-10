// #Regression #Conformance #TypesAndModules #Unions 
// Regression test for FSHARP1.0:4787 ("Discriminated union constructors as first class values")
//<Expects status="success"></Expects>

type A =
    | A1 of int * int
    | A2 of int * int * int

let x2 = (1, 1)
let x3 = (1, 1, 1)

let y1 = A1 x2    // OK
let z1 = (A1) x2  // OK - same as above

let y2 = A2 x3    // OK
let z2 = (A2) x3  // OK - same as above

if (y1 = z1) && (y2 = z2) then 0 else failwith "Failed: 1"