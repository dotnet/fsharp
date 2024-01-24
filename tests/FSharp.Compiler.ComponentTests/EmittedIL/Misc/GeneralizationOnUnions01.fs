// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSharp1.0:6389 - Nullary union cases improperly constrain generic type parameters

type Weirdo = | C
let f C = 0 // parameter name is C
let g ()  = 
   let C = 1
   let f C = C // what is parameter name here? Do we even care?
   ()

exit 0