open Prelude

module InRefParam_Generic_ExplicitAddressOfAttCallSite1 = 
    type C() = 
         static member M(x: inref<'T>) = x
    let Test() = 
        let res = "abc"
        let v =  C.M(&res)
        check "lmvjvwo2" res "abc"
        check "lmvjvwo3" v "abc"
    Test()