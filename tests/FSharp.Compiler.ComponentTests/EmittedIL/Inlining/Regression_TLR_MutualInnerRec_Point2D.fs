module Regression_TLR_MutualInnerRec_Point2D

[<Struct>]
type Point2D(x: double, y: double) =
    member _.X = x
    member _.Y = y

let fifth() =
    let rec firstCallee(n, a: Point2D, b: Point2D, c: Point2D, d: Point2D, e: Point2D) =
        if a.X <> 10.0 then -100
        elif n = 0 then 100
        elif n % 2 = 0 then secondCallee(n - 1, a, b, c, d, e)
        else firstCallee(n - 1, a, b, c, d, e)
    and secondCallee(n, a: Point2D, b: Point2D, c: Point2D, d: Point2D, e: Point2D) =
        if n = 0 then 101
        elif n % 2 = 0 then secondCallee(n - 1, a, b, c, d, e)
        else firstCallee(n - 1, a, b, c, d, e)
    let p = Point2D(10.0, 20.0)
    let q = Point2D(30.0, 40.0)
    firstCallee(1000000, p, q, p, q, p)

[<EntryPoint>]
let main _argv = fifth ()
