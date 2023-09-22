open Prelude

module InRefParam_DateTime_ImplicitAddressOfAtCallSite3  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let mutable w = System.DateTime.Now
    let v =  C.M(w)
    check "cweweoiwe51btw" v w