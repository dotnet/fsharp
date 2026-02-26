// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ [|1;2;3|] @>
let q' = Expr.NewArray(typeof<float>, [Expr.Value(0.0); Expr.Value(1.0); Expr.Value(2.0)])

let r1 = verify q (|NewArray|_|) "NewArray (Int32, Value (1), Value (2), Value (3))"
let r2 = verify q' (|NewArray|_|) "NewArray (Double, Value (0.0), Value (1.0), Value (2.0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
