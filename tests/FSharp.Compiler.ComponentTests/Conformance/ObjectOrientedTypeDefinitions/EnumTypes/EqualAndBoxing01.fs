// #Regression #Conformance #ObjectOrientedTypes #Enums 
// Regression test for FSHARP1.0:5535
// Enum type
type E = | A = 1y
         | B = 2y

let e1 = E.A
let e2 = E.B

let t1 = e1.Equals(box e2)        // expect: false, no exception!
let t2 = e1.Equals(box e1)        // expect: true, no exception!

(if not t1 && t2 then 0 else 1) |> exit
