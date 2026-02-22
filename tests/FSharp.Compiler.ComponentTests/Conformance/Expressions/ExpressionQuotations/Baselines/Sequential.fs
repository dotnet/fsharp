// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let f x = ()
let g x = ()
let q = <@ f 1; g 2 @>
let q' = Expr.Sequential(
            Expr.Let(Var("x", typeof<string>), Expr.Value("A"), Expr.Value(1)), 
            Expr.Call(Expr.Value("D"), typeof<string>.GetMethod("Contains"), [Expr.Value("C")]))
            
let r1 = verify q (|Sequential|_|) "Sequential (Call (None, f, [Value (1)]), Call (None, g, [Value (2)]))"
let r2 = verify q' (|Sequential|_|) "Sequential (Let (x, Value (\"A\"), Value (1)),
            Call (Some (Value (\"D\")), Contains, [Value (\"C\")]))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
