// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 1.0f in x @>
let q' = Expr.Value(1.0f)

let r1 = verify q (|Single|_|) "Let (x, Value (1.0f), x)"
let r2 = verify q' (|Single|_|) "Value (1.0f)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
