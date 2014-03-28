// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

type S(z : int) =
    [<DefaultValue>] val mutable x : int

let q = <@ S(1).x <- 1 @>
let q' = Expr.FieldSet(Expr.NewObject(typeof<S>.GetConstructor([|typeof<int>|]), [Expr.Value(1)]), typeof<S>.GetField("x"), Expr.Value(1))    

let r1 = verify q (|FieldSet|_|) "FieldSet (Some (NewObject (S, Value (1))), x, Value (1))"
let r2 = verify q' (|FieldSet|_|) "FieldSet (Some (NewObject (S, Value (1))), x, Value (1))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
