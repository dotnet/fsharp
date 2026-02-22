// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ for i in 1..10 do () @> 
let q' = Expr.ForIntegerRangeLoop(Var("i", typeof<int>), Expr.Value(-2), Expr.Value(-3), Expr.Value(null))

let r1 = verify q (|ForIntegerRangeLoop|_|)  "ForIntegerRangeLoop (i, Value (1), Value (10), Value (<null>))"
let r2 = verify q' (|ForIntegerRangeLoop|_|) "ForIntegerRangeLoop (i, Value (-2), Value (-3), Value (<null>))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
