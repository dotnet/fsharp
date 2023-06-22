// #Regression #Conformance #TypesAndModules #Unions 
// Parethesis are no longer ignificant in DU definitions
// See FSHARP1.0:4787 (Discriminated union constructors as first class values)
//<Expects status="success"></Expects>

type T = | C of int * int
         | D of (int * int)
         
let a = 1,2         
let x1 = C(a)     // ok!
let x2 = C(1,2)

if x1 = x2 then 0 else failwith "Failed: 1"
