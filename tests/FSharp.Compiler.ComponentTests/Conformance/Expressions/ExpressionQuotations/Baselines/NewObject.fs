// #Conformance #Quotations 
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let x = new List<int>() in x @>
let q' = Expr.NewObject(typeof<List<int>>.GetConstructor([|typeof<int>|]), [Expr.Value(0)])

let r1 = verify q (|NewObject|_|) "Let (x, NewObject (List`1), x)" 
let r2 = verify q' (|NewObject|_|) "NewObject (List`1, Value (0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
