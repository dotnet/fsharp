open Prelude

module ByRefReturn = 
    type C() = 
         static member M(x: byref<int>) = x <- x + 1; &x
    let mutable res = 9
    let v =  C.M(&res)
    check "cwvereweoiwvw4" v 10

    let minfo = typeof<C>.GetMethod("M")
    check "cwnoreeker6d" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
    check "cwnoreekerr" (minfo.ReturnParameter.IsIn) false
    check "cwnoreekert" (minfo.ReturnParameter.IsOut) false