module DelegateKnownFunction

open System

// known F# functions compiled as methods
let handlerCurried (x: int) (y: int) : unit = ()
let handlerTupled (x: int, y: int) : unit = ()
let handler3 (x: int) (y: int) (z: int) : unit = ()

// 17. non-eta module function
let case17_nonEta () = Action<int, int>(handlerCurried)

// 1. eta module function (curried application)
let case1_etaCurried () = Action<int, int>(fun a b -> handlerCurried a b)

// 31. eta module function, tupled application (same compiled representation)
let case31_etaTupled () = Action<int, int>(fun a b -> handlerTupled (a, b))

// 37. partial application of module function (constant arg)
let case37_partial () = Action<int, int>(handler3 1)
