module DelegateUnitArg

open System

let handler () : unit = ()

// 48. non-eta unit-argument delegate
let caseUnitNonEta () = Action(handler)

// 49. eta unit-argument delegate
let caseUnitEta () = Action(fun () -> handler ())
