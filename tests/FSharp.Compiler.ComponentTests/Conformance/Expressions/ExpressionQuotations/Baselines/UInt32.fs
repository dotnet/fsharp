// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 1u in x @>
let q' = Expr.Value(1u)

let r1 = verify q (|UInt32|_|) "Let (x, Value (1u), x)"
let r2 = verify q' (|UInt32|_|) "Value (1u)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
