module DelegateKnownFunction

open System

// known F# functions compiled as methods
let handlerCurried (x: int) (y: int) : unit = ()
let handlerTupled (x: int, y: int) : unit = ()
let handler3 (x: int) (y: int) (z: int) : unit = ()

// 1. non-eta-expanded known target
let case1_nonEta () = Action<int, int>(handlerCurried)

// 2. eta-expanded known target (curried application)
let case2_etaCurried () = Action<int, int>(fun a b -> handlerCurried a b)

// 3. eta-expanded known target, tupled application, same compiled representation
let case3_etaTupled () = Action<int, int>(fun a b -> handlerTupled (a, b))

// 15. non-eta-expanded known target via partial application
//     (currently a closure with "delegateArg0"-style names; proposal: use elided arg names y, z)
let case15_partial () = Action<int, int>(handler3 1)
