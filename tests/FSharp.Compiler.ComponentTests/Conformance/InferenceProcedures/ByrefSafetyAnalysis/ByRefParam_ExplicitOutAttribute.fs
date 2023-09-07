open Prelude

module ByRefParam_ExplicitOutAttribute  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.Out>] x: byref<int>) = x <- 5
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwekl4" res 5

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreeker5" (minfo.GetParameters().[0].IsIn) false
    check "cwnoreeker6a" (minfo.GetParameters().[0].IsOut) true
    check "cwnoreeker6b" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
    check "cwnoreekers1" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 0
    check "cwnoreeker7" (minfo.ReturnParameter.IsIn) false
    check "cwnoreeker8" (minfo.ReturnParameter.IsOut) false