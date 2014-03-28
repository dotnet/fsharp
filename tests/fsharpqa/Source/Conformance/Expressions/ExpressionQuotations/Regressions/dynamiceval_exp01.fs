// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression test for FSHARP1.0:3239
// Make sure we can dynamically evaluate overloaded math functions
// exp
//<Expects status=success></Expects>
#light

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let Eval (q: Expr<_>) = q.Eval()

(if (Eval <@ exp 0.0 @>) = 1.0 then 0 else 1) |> exit
