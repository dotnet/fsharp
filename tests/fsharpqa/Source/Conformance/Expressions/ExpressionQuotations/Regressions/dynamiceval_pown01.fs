// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression test for FSHARP1.0:3239
// Make sure we can dynamically evaluate overloaded math functions
// pown
#light

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let Eval (q: Expr<_>) = q.Eval()

if Eval <@ pown 2.0 3 @> <> 8.0 then exit 1
if Eval <@ pown 2y 3 @> <> 8y then exit 1
if Eval <@ pown 2s 3 @> <> 8s then exit 1
if Eval <@ pown 2L 3 @> <> 8L then exit 1
if Eval <@ pown 2I 3 @> <> 8I then exit 1
if Eval <@ pown 2M 3 @> <> 8M then exit 1

exit 0