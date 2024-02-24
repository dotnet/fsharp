open Prelude

module InRefParam  = 
    type C() = 
         static member M(x: inref<System.DateTime>) = x
    let Test() = 
        let res = System.DateTime.Now
        let v =  C.M(&res)
        check "cweweoiwe51btw" v res

        let minfo = typeof<C>.GetMethod("M")
        check "cwnoreekerf" (minfo.GetParameters().[0].IsIn) true
        check "cwnoreekerg" (minfo.GetParameters().[0].IsOut) false
    Test()