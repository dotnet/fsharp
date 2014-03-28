// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression test for FSHARP1.0:3239
// Make sure we can dynamically evaluate overloaded math functions
// sign
#light

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let Eval (q: Expr<_>) = q.Eval()

(if (Eval <@ sign -2.23 @>) = -1 then 0 else 1) |> exit
if Eval <@ sign 2.1 @> <> 1 then exit 1
if Eval <@ sign -2y @> <> -1 then exit 1
if Eval <@ sign -2s @> <> -1 then exit 1
if Eval <@ sign -2L @> <> -1 then exit 1
if Eval <@ sign 2I @> <> 1 then exit 1
if Eval <@ sign 2M @> <> 1 then exit 1

exit 0
