// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let q = <@ let x = 'a' in x @>
let q' = Expr.Value('a')

let r1 = verify q (|Char|_|) "Let (x, Value ('a'), x)"
let r2 = verify q' (|Char|_|) "Value ('a')"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
