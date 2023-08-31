open Prelude

module InRefParam_DateTime_ImplicitAddressOfAtCallSite4  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let date = System.DateTime.Now.Date
    let w = [| date |]
    let v =  C.M(w.[0])
    check "lmvjvwo1" v date