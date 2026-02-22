// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = if (true && false) then 0 else 1 in x @>
let q' = Expr.IfThenElse(Expr.IfThenElse(Expr.Value(true), Expr.Value(false), Expr.Value(false)), Expr.Value(0), Expr.Value(1))

let r1 = verify q (|AndAlso|_|) "Let (x,
     IfThenElse (IfThenElse (Value (true), Value (false), Value (false)),
                 Value (0), Value (1)), x)"
let r2 = verify q' (|AndAlso|_|) "IfThenElse (IfThenElse (Value (true), Value (false), Value (false)), Value (0),
            Value (1))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
