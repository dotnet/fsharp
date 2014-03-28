// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression test for FSHARP1.0:3239
// Make sure we can dynamically evaluate overloaded math functions
// abs
#light

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let Eval (q: Expr<_>) = q.Eval()

if Eval <@ abs(-1.0) @> <> 1.0 then exit 1
if Eval <@ abs(-1y) @> <> 1y then exit 1
if Eval <@ abs(-1s) @> <> 1s then exit 1
if Eval <@ abs(-1L) @> <> 1L then exit 1
if Eval <@ abs(-1I) @> <> 1I then exit 1
if Eval <@ abs(-1M) @> <> 1M then exit 1

exit 0
