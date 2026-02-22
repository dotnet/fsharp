// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let x, y = (1,2) in x @>
let q' = Expr.Let(Var("x", typeof<int -> int>), Expr.Lambda(Var("x", typeof<int>), Expr.Value(0)), Expr.Value(1)) 

let r1 = verify q (|Let|_|) "Let (patternInput, NewTuple (Value (1), Value (2)),
     Let (y, TupleGet (patternInput, 1), Let (x, TupleGet (patternInput, 0), x)))"
let r2 = verify q' (|Let|_|) "Let (x, Lambda (x, Value (0)), Value (1))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
