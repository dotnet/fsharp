// #Regression #Conformance #UnitsOfMeasure #RequiresPowerPack 
// Regression test for FSHARP1.0:3248
// Verify we can use Units of Measure in a quotation
#light

// Bring .Eval(), .Compile(), etc... in scope!
open Microsoft.FSharp.Linq.QuotationEvaluation

let x = -4.0M<1>
let q = <@ if (x = -4.0M<1>) then 0 else 1 @>

q.Eval() |> exit
