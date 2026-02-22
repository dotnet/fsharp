// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = () in x @>
let q' = Expr.Value(())

let r1 = verify q (|Unit|_|) "Let (x, Value (<null>), x)"
let r2 = verify q' (|Unit|_|) "Value (<null>)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
