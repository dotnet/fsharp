// #Conformance #TypesAndModules #Unions 
// By default, Union types implement dispatch slot Equals
// Also minimal test on the expected implementation.
//<Expects status="success"></Expects>
#light

type T1 = | A
          | B 

type T2 = | A
          | C 

let r1 = not (T1.A.Equals(T1.B))
let r2 = T1.A.Equals(T1.A)
let r3 = not (T1.A.Equals(T2.A))

let t1 = T1.A
let t2 = T1.A
let t3 = T2.A
let t4 = T1.B

let r4 = not (t1.Equals(t4))
let r5 = t1.Equals(t1)
let r6 = not (t1.Equals(t3))

if r1 && r2 && r3 && r4 && r5 && r6  then 0 else failwith "Failed: 1"
