// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ try let x = 1 in x finally () @>
let q' = Expr.TryFinally(Expr.Value(1), Expr.Value(0))

let r1 = verify q (|TryFinally|_|) "TryFinally (Let (x, Value (1), x), Value (<null>))"
let r2 = verify q' (|TryFinally|_|) "TryFinally (Value (1), Value (0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
