// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let x = (1, "1") in x @>
let q' = Expr.NewTuple([Expr.Value(1.0); Expr.Value(2.0)])

let r1 = verify q (|NewTuple|_|) "Let (x, NewTuple (Value (1), Value (\"1\")), x)"
let r2 = verify q' (|NewTuple|_|) "NewTuple (Value (1.0), Value (2.0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
