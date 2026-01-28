// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 1.0 in x @>
let q' = Expr.Value(-1.0)

let r1 = verify q (|Double|_|) "Let (x, Value (1.0), x)"
let r2 = verify q' (|Double|_|) "Value (-1.0)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
