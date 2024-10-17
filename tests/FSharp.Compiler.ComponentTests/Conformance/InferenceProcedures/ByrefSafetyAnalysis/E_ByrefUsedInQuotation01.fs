// #Regression #Conformance #TypeInference #ByRef 
// Attempt to use a byref in a quotation




open Microsoft.FSharp.Quotations

let test a = ();
let mutable i = 5 in
test <@ &i @>
