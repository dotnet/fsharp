// #Regression #Conformance #DataExpressions #Tuples 
// Regression test for FSHARP1.0:5514
// This was actually a BCL bug (TFS#660592)
// Comparing tuples of different types should not throw at runtime

let t = (2,3) :> obj
let s = "test" :> obj

(if t = s then 1 else 0) |> exit
