// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

type d = delegate of (int * int) -> int
let q = <@ let d1 = new d((fun (x,y) -> x + y)) in () @>
let q' = Expr.NewDelegate(typeof<d>, [Var("x", typeof<int * int>)], Expr.Value(0))

let r1 = verify q (|NewDelegate|_|) "Let (d1,
     NewDelegate (d, tupledArg,
                  Let (x, TupleGet (tupledArg, 0),
                       Let (y, TupleGet (tupledArg, 1),
                            Call (None, op_Addition, [x, y])))), Value (<null>))"
let r2 = verify q' (|NewDelegate|_|) "NewDelegate (d, x, Value (0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
