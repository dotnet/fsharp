// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5612
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

[<Struct>]
type S(z : int) =
    [<DefaultValue>] val mutable x : int

let q = <@ S(1).x @>
let q' = Expr.FieldGet(Expr.NewObject(typeof<S>.GetConstructor([|typeof<int>|]), [Expr.Value(1)]), typeof<S>.GetField("x"))    

let r1 = verify q (|FieldGet|_|) "Let (copyOfStruct, NewObject (S, Value (1)), FieldGet (Some (copyOfStruct), x))"
let r2 = verify q' (|FieldGet|_|) "FieldGet (Some (NewObject (S, Value (1))), x)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
