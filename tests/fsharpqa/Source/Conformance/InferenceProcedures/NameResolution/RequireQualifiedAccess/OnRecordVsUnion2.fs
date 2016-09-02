// #Conformance #TypeInference #Attributes 
// Verify the RequireQualifiedAccess attribute works on unions

module Module =
    type R = { a: int } with static member New = { a = 1 }
    type Choice = | R of R
open Module

let record1 = R.New
let choice1 v =
    match v with
    | R r -> r

let record2 = Module.R.New
let choice2 v =
    match v with
    | Module.R r -> r