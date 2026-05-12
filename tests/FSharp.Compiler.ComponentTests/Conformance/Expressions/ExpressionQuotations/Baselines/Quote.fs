// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let f x = <@ x + 1 @> in f 1 @>
let q' = Expr.Quote(Expr.Quote(Expr.Let(Var("x", typeof<int>), Expr.Value(0), Expr.Value(1))))

let r1 = verify q (|Quote|_|) "Let (f, Lambda (x, Quote (Call (None, op_Addition, [x, Value (1)]))),
     Application (f, Value (1)))"
let r2 = verify q' (|Quote|_|) "Quote (Quote (Let (x, Value (0), Value (1))))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
