open Prelude

module ByRefParam_ExplicitInAttribute  = 
    type C() = 
         static member M([<System.Runtime.InteropServices.In>] x: byref<int>) = x <- 5
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwekl4" res 5

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreeker9" (minfo.GetParameters().[0].IsIn) true
    check "cwnoreekerq" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreeker6c" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
    check "cwnoreekers2" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 0
    check "cwnoreekerw" (minfo.ReturnParameter.IsIn) false
    check "cwnoreekere" (minfo.ReturnParameter.IsOut) false