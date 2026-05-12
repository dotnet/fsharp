// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ while true do () @>
let q' = Expr.WhileLoop(Expr.Value(true), Expr.Value(null, typeof<unit>))

let r1 = verify q (|WhileLoop|_|) "WhileLoop (Value (true), Value (<null>))"
let r2 = verify q' (|WhileLoop|_|) "WhileLoop (Value (true), Value (<null>))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
