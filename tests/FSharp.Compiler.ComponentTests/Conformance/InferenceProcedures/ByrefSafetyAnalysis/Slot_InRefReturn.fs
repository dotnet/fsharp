open Prelude

module Slot_InRefReturn  = 
    type I = 
         abstract M : x: inref<int> -> inref<int>
    type C() = 
         interface I with 
             member __.M(x: inref<int>) = &x
    let mutable res = 9
    let v =  (C() :> I).M(&res)
    check "cweweoiwek28989" res 9
    check "cweweoiwek28989" v 9

    let minfo = typeof<I>.GetMethod("M")
    check "cwnoreekerp" (minfo.GetParameters().[0].IsIn) true
    check "cwnoreekera" (minfo.GetParameters().[0].IsOut) false
    check "cwnoreeker6g" (minfo.GetParameters().[0].GetRequiredCustomModifiers().Length) 1
    check "cwnoreekers5" (minfo.ReturnParameter.IsIn) false // has modreq 'In' but reflection never returns true for ReturnParameter.IsIn
    check "cwnoreekers6" (minfo.ReturnParameter.GetRequiredCustomModifiers().Length) 1
    check "cwnoreekerd" (minfo.ReturnParameter.IsOut) false