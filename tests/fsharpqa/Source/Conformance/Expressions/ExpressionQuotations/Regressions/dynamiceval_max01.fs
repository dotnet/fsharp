// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression test for FSHARP1.0:3239
// Make sure we can dynamically evaluate overloaded math functions
// max
#light

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let Eval (q: Expr<_>) = q.Eval()

if Eval <@ max -1.0 2.0 @> <> 2.0 then exit 1
if Eval <@ max -1y -2y @> <> -1y then exit 1
if Eval <@ max -1s -2s @> <> -1s then exit 1
if Eval <@ max -1L 1L @> <> 1L then exit 1
if Eval <@ max -1I 1I @> <> 1I then exit 1
if Eval <@ max 1M 2M @> <> 2M then exit 1

exit 0
