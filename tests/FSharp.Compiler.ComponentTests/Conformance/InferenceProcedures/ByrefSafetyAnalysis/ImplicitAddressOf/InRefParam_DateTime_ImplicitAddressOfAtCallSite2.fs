open Prelude

module InRefParam_DateTime_ImplicitAddressOfAtCallSite2   = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let v =  C.M(System.DateTime.Now.AddDays(1.0))
    check "cweweoiwe51btw" v.Date (System.DateTime.Now.AddDays(1.0).Date)