// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = true in x @>
let q' = Expr.Value(true)

let r1 = verify q (|Bool|_|) "Let (x, Value (true), x)"
let r2 = verify q' (|Bool|_|) "Value (true)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
