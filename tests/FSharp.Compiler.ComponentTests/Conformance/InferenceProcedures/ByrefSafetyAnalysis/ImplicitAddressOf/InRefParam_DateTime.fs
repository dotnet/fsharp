open Prelude

module InRefParam_DateTime   = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let w = System.DateTime.Now
    let v =  C.M(w)
    check "cweweoiwe51btw" v w