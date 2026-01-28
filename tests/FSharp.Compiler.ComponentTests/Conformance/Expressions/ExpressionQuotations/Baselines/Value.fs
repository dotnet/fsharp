// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ 0 @>
let q' = Expr.Value(null)

let r1 = verify q (|Value|_|) "Value (0)"           
let r2 = verify q' (|Value|_|) "Value (<null>)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
