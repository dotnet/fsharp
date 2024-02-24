open Prelude

module Slot_OutRefParam  = 
    type I = 
         abstract M : x: outref<int> -> unit
    type C() = 
         interface I with 
             member __.M(x: outref<int>) = x <- 5
    let mutable res = 9
    (C() :> I).M(&res)
    check "cweweoiwek28989" res 5