// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let (x,y) = (1,2) in x @>
let q' = Expr.Let(Var("x", typeof<int>), Expr.TupleGet(Expr.Var(Var("t", typeof<int * int>)), 0), Expr.Value(0))

let r1 = verify q (|TupleGet|_|) "Let (patternInput, NewTuple (Value (1), Value (2)),
     Let (y, TupleGet (patternInput, 1), Let (x, TupleGet (patternInput, 0), x)))"
let r2 = verify q' (|TupleGet|_|) "Let (x, TupleGet (t, 0), Value (0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
