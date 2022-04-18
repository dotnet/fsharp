// #Conformance #TypesAndModules #Unions 
// Paretheses are significant in DU definitions
//<Expects status="success"></Expects>
#light

type T = | C of int * int
         | D of (int * int)
         
let a = 1,2         
let x = C(1,2)
let y = D(1,2)
let y1 = D(a) // ok

match x with
| C(x,y) -> ()
| D(z) -> ()
