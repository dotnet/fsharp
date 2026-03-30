module UnionTest.Program

open UnionTest.Types
open UnionTest.Operations

[<EntryPoint>]
let main _argv =
    let c = Circle 5.0
    let r = Rectangle(3.0, 4.0)
    printfn "Circle area: %f" (area c)
    printfn "Rect area: %f" (area r)

    let expr = Add(Mul(Const 2, Const 3), Const 4)
    printfn "Expr result: %d" (eval expr)

    printfn "%s" (describe Start)
    0
