// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 1uy in x @>
let q' = Expr.Value(1uy)

let r1 = verify q (|Byte|_|) "Let (x, Value (1uy), x)"
let r2 = verify q' (|Byte|_|) "Value (1uy)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
