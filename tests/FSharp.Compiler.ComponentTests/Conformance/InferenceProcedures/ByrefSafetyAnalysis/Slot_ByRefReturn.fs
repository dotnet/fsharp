open Prelude

module Slot_ByRefReturn  = 
    type I = 
         abstract M : x: byref<int> -> byref<int>
    type C() = 
         interface I with 
             member __.M(x: byref<int>) = x <- 5; &x
    let mutable res = 9
    let v =  (C() :> I).M(&res)
    check "cweweoiwek28989" res 5
    check "cweweoiwek28989" v 5

    let minfo = typeof<I>.GetMethod("M")
    check "cwnoreeker6e" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 0
    check "cwnoreekery" (minfo.GetParameters().[0].IsIn) false
    check "cwnoreekeru" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreekeri" (minfo.ReturnParameter.IsIn) false
    check "cwnoreekers" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 0
    check "cwnoreekero" (minfo.ReturnParameter.IsOut) false