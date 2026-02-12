// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ if true then 0 else 1 @>
let q' = Expr.IfThenElse(Expr.Value(true), Expr.Value(0), Expr.Value(1))

let r1 = verify q (|IfThenElse|_|) "IfThenElse (Value (true), Value (0), Value (1))"
let r2 = verify q' (|IfThenElse|_|) "IfThenElse (Value (true), Value (0), Value (1))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
