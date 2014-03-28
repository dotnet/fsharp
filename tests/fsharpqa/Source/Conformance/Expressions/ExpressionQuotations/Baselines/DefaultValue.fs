// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5751
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

[<Struct>]
type T =
    [<DefaultValue>]
    val data : int
    
let q = <@ T() @>
let q' = Expr.Let(Var("x", typeof<T>), Expr.DefaultValue(typeof<T>), Expr.Value(0))

let r1 = verify q (|DefaultValue|_|) "DefaultValue (T)"
let r2 = verify q' (|DefaultValue|_|) "Let (x, DefaultValue (T), Value (0))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
