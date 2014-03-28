// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = true || false in x @>
let q' = Expr.IfThenElse(Expr.Value(true), Expr.Value(true), Expr.Value(false))

let r1 = verify q (|OrElse|_|) "Let (x, IfThenElse (Value (true), Value (true), Value (false)), x)"
let r2 = verify q' (|OrElse|_|) "IfThenElse (Value (true), Value (true), Value (false))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
