module DelegateStaticMethod

open System

type C =
    static member AddC (x: int) (y: int) : unit = ()
    static member AddT (x: int, y: int) : unit = ()
    static member Add3 (x: int) (y: int) (z: int) : unit = ()

// 18. non-eta static method
//     (a tupled member is seen as a single tuple-arg value and will not coerce non-eta; use the curried member)
let case18_nonEta () = Action<int, int>(C.AddC)

// 2. eta static method (curried application)
let case2_etaCurried () = Action<int, int>(fun a b -> C.AddC a b)

// 32. eta static method, tupled application
let case32_etaTupled () = Action<int, int>(fun a b -> C.AddT(a, b))

// 38. partial application of static method (constant arg)
let case38_partial () = Action<int, int>(C.Add3 1)
