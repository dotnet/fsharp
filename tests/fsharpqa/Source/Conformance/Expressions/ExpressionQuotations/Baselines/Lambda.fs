// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ (fun (x: int) -> x) @>
let q' = Expr.Lambda(Var("x", typeof<int>), Expr.Value(-1))

let r1 = verify q (|Lambda|_|) "Lambda (x, x)"
let r2 = verify q' (|Lambda|_|) "Lambda (x, Value (-1))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
