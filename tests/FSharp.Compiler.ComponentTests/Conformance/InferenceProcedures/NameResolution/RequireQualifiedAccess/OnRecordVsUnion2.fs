// #Conformance #TypeInference #Attributes

module Module =
    type R = { a: int } with static member New = { a = 1 }
    type Choice = | R of R
open Module

let record1 = R.New
let choice1 v =
    match v with
    | R r -> r

let newChoice = R { a = 1}

let choice2 v =
    match v with
    | Module.R r -> r
    
let newChoice2 = Module.R { a = 1}