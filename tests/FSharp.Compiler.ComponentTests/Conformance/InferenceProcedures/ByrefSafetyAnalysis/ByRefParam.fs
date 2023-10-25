open Prelude

module ByRefParam  = 
    type C() = 
         static member M(x: byref<int>) = x <- 5
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwekl4" res 5

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreeker1" (minfo.GetParameters().[0].IsIn) false
    check "cwnoreeker2" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreeker3" (minfo.ReturnParameter.IsIn) false
    check "cwnoreeker4" (minfo.ReturnParameter.IsOut) false