// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = (1s) in x @>
let q' = Expr.Value(1s)

let r1 = verify q (|Int16|_|) "Let (x, Value (1s), x)"
let r2 = verify q' (|Int16|_|) "Value (1s)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
