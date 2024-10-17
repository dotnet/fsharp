// #Regression #Conformance #TypesAndModules #Unions 
// Union types NO LONGER implement IStructuralHash
// They used to, but now they don't anymore.
// 




 
type I = | A
         | B

let p = A
let q = B

let mutable ix = 18
let ip = p :> IStructuralHash              // err: The type 'IStructuralHash' is not defined.
ip.GetStructuralHashCode(&ix)

let mutable iy = 18
let iq = q :> IStructuralHash              // err: The type 'IStructuralHash' is not defined.
iq.GetStructuralHashCode(&iy)
