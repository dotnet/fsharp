// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 1 in x @>
let q' = Expr.Value(1)

let r1 = verify q (|Int32|_|) "Let (x, Value (1), x)"
let r2 = verify q' (|Int32|_|) "Value (1)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
