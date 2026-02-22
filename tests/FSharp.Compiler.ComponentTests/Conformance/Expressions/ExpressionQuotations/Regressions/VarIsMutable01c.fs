// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:4606
// Make sure we expose a .IsMutable property
// isMutable = <not set>

open Microsoft.FSharp.Quotations;

let v = Microsoft.FSharp.Quotations.Var("aa", typeof<System.Int32>)
(if not v.IsMutable then 0 else 1) |> exit

