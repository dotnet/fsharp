// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = (1us) in x @>
let q' = Expr.Value(1us)

let r1 = verify q (|UInt16|_|) "Let (x, Value (1us), x)"
let r2 = verify q' (|UInt16|_|) "Value (1us)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
