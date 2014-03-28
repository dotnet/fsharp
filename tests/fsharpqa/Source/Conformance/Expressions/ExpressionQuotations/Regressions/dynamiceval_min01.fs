// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression test for FSHARP1.0:3239
// Make sure we can dynamically evaluate overloaded math functions
// min
//<Expects status=success></Expects>
#light

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let Eval (q: Expr<_>) = q.Eval()

if Eval <@ min -1.0 2.0 @> <> -1.0 then exit 1
if Eval <@ min -1y -2y @> <> -2y then exit 1
if Eval <@ min -1s -2s @> <> -2s then exit 1
if Eval <@ min -1L 1L @> <> -1L then exit 1
if Eval <@ min -1I 1I @> <> -1I then exit 1
if Eval <@ min 1M 2M @> <> 1M then exit 1

exit 0
