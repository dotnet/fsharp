// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression test for an issue reported by a customer
// The following code used to give an ICE in CTP bits (1.9.6.2)
//<Expects id="FS0191" status="error">The variable 'x' is bound in a quotation but is used as part of a spliced expression\. This is not permitted since it may escape its scope</Expects>

open Microsoft.FSharp.Quotations

let quote (v:'a) = Expr.Cast<'a>(Expr.Value(v,typeof<'a>))

let g x = <@ fun x -> %(f (quote x)) @>

