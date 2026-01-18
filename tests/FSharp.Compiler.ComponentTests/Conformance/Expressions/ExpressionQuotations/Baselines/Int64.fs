// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 1L in x @>
let q' = Expr.Value(1L)

let r1 = verify q (|Int64|_|) "Let (x, Value (1L), x)"
let r2 = verify q' (|Int64|_|) "Value (1L)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
