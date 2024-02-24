open Prelude

module InRefParam_DateTime_ImplicitAddressOfAtCallSite  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let v =  C.M(System.DateTime.Now)
    check "cweweoiwe51btw" v.Date System.DateTime.Now.Date