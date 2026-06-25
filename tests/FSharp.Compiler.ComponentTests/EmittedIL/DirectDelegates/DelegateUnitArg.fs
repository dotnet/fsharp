module DelegateUnitArg

open System

let handler () : unit = ()

// 46. non-eta unit-argument delegate
let caseUnitNonEta () = Action(handler)

// 47. eta unit-argument delegate
let caseUnitEta () = Action(fun () -> handler ())
