module Foo

open System

type Int32 with
    member this.PlusPlus () = this + 1
    
let two = (1).PlusPlus()