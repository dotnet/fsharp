open Prelude

module InRefParam_ExplicitInAttributeDateTime = 
    type C() = 
         static member M([<System.Runtime.InteropServices.In>] x: inref<System.DateTime>) = x
    let Test() = 
        let res = System.DateTime.Now
        let v = C.M(&res)
        check "cweweoiwe519cw" v res
    Test()