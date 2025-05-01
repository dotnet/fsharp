open Prelude

module InRefParamOverload_ImplicitAddressOfAtCallSite2  = 
    type C() = 
         static member M(x: System.DateTime) = x.AddDays(1.0)
         static member M(x: inref<System.DateTime>) = x.AddDays(2.0)
         static member M2(x: System.DateTime, y: int) = x.AddDays(1.0)
         static member M2(x: inref<System.DateTime>, y: int) = x.AddDays(2.0)
    let Test() = 
        let res = System.DateTime.Now
        let v =  C.M(res)
        check "cweweoiwe51btw1" v (res.AddDays(1.0))
        let v2 =  C.M2(res, 4)
        check "cweweoiwe51btw2" v2 (res.AddDays(1.0))
    Test()