module DelegateUnitArg

open System

let handler () : unit = ()

// unit-argument delegate, non-eta-expanded known target
let caseUnitNonEta () = Action(handler)

// unit-argument delegate, eta-expanded
let caseUnitEta () = Action(fun () -> handler ())
