module DelegateInstanceMethod

open System

type C(k: int) =
    member _.AddC (x: int) (y: int) : unit = ignore k
    member _.AddT (x: int, y: int) : unit = ignore k
    abstract V : int -> int -> unit
    default _.V (x: int) (y: int) : unit = ignore k

// 10. non-eta-expanded instance method target
let case10_nonEta (o: C) = Action<int, int>(o.AddC)

// 11. eta-expanded, curried application
let case11_etaCurried (o: C) = Action<int, int>(fun a b -> o.AddC a b)

// 12. eta-expanded, tupled application
let case12_etaTupled (o: C) = Action<int, int>(fun a b -> o.AddT(a, b))

// virtual instance method: a direct delegate must use ldvirtftn (with dup) to preserve dispatch
let caseVirtual (o: C) = Action<int, int>(o.V)
