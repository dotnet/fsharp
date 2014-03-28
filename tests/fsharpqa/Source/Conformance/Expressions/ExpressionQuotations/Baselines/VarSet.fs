// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5752
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

open QuoteUtils

let q = <@ let mutable x = 0 in x <- 1 @>
let q' = Expr.VarSet(Var("x", typeof<int>), Expr.Value(2))

let r1 = verify q (|VarSet|_|) "Let (x, Value (0), VarSet (x, Value (1)))"
let r2 = verify q' (|VarSet|_|) "VarSet (x, Value (2))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
