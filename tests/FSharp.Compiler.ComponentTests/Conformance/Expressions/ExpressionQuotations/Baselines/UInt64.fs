// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 1UL in x @>
let q' = Expr.Value(1UL)

let r1 = verify q (|UInt64|_|) "Let (x, Value (1UL), x)"
let r2 = verify q' (|UInt64|_|) "Value (1UL)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
