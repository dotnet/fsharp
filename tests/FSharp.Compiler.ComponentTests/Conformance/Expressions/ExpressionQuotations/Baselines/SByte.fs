// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 1y in x @>
let q' = Expr.Value(1y)

let r1 = verify q (|SByte|_|) "Let (x, Value (1y), x)"
let r2 = verify q' (|SByte|_|) "Value (1y)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
