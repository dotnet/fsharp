open Prelude

module InRefReturn  = 
    type C() = 
         static member M(x: inref<int>) = &x
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwvw4" v 9

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreekerp" (minfo.GetParameters().[0].IsIn) true
    check "cwnoreekera" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreeker6f" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0 // modreq only placed on abstract/virtual
    check "cwnoreekers3" (minfo.ReturnParameter.IsIn) false // has modreq 'In' but reflection never returns true for ReturnParameter.IsIn
    check "cwnoreekers4" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 1
    check "cwnoreekerd" (minfo.ReturnParameter.IsOut) false