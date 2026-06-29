module DelegateUnitArg

open System

[<NoCompilerInlining>]
let handler () : unit = ()

type C() =
    [<NoCompilerInlining>]
    member _.M () : unit = ()

// 46. non-eta unit-argument delegate
let caseUnitNonEta () = Action(handler)

// 47. eta unit-argument delegate
let caseUnitEta () = Action(fun () -> handler ())

// 48. non-eta unit-argument delegate, instance method (receiver kept, unit stripped)
let caseUnitInstanceNonEta (c: C) = Action(c.M)

// 49. eta unit-argument delegate, instance method
let caseUnitInstanceEta (c: C) = Action(fun () -> c.M ())
