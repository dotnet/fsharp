// #Conformance #Quotations 
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ let a = new List<int>(5) in a.[0] <- 4 @>
let q' = Expr.PropertySet(Expr.Value(new List<int>(5)), typeof<List<int>>.GetProperty("Item"), Expr.Value(4), [Expr.Value(0)])

let r1 = verify q (|PropertySet|_|) "Let (a, NewObject (List`1, Value (5)),
     PropertySet (Some (a), Item, [Value (0), Value (4)]))"
let r2 = verify q' (|PropertySet|_|) "PropertySet (Some (Value (seq [])), Item, [Value (0), Value (4)])"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
