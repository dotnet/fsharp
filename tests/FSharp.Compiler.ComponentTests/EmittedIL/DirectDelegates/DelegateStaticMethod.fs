module DelegateStaticMethod

open System

type C =
    static member AddC (x: int) (y: int) : unit = ()
    static member AddT (x: int, y: int) : unit = ()
    static member Add3 (x: int) (y: int) (z: int) : unit = ()

// 4. non-eta-expanded static method target
//    (a tupled member is seen as a single tuple-arg value and will not coerce non-eta; use the curried member)
let case4_nonEta () = Action<int, int>(C.AddC)

// 5. eta-expanded, curried application
let case5_etaCurried () = Action<int, int>(fun a b -> C.AddC a b)

// 6. eta-expanded, tupled application
let case6_etaTupled () = Action<int, int>(fun a b -> C.AddT(a, b))

// 16. non-eta-expanded static method via partial application
let case16_partial () = Action<int, int>(C.Add3 1)
