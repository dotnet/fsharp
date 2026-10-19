module DelegateInstanceMethod

open System

type C(k: int) =
    member _.AddC (x: int) (y: int) : unit = ignore k
    member _.AddT (x: int, y: int) : unit = ignore k
    abstract V : int -> int -> unit
    default _.V (x: int) (y: int) : unit = ignore k

// 20. non-eta instance method
let case20_nonEta (o: C) = Action<int, int>(o.AddC)

// 4. eta instance method (curried application)
let case4_etaCurried (o: C) = Action<int, int>(fun a b -> o.AddC a b)

// 34. eta instance method, tupled application
let case34_etaTupled (o: C) = Action<int, int>(fun a b -> o.AddT(a, b))

// 21. non-eta virtual instance method: a direct delegate must use ldvirtftn (with dup) to preserve dispatch
let case21_virtual (o: C) = Action<int, int>(o.V)
