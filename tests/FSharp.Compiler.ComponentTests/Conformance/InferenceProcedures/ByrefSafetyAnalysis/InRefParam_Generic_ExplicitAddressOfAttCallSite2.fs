open Prelude

module InRefParam_Generic_ExplicitAddressOfAttCallSite2  = 
    type C() = 
         static member M(x: inref<'T>) = x
    let Test() = 
        let res = "abc"
        let v =  C.M(&res)
        check "lmvjvwo4" v "abc"
    Test()