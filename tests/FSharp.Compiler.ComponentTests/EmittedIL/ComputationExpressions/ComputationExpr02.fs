// #Regression #NoMono #NoMT #CodeGen #EmittedIL #ComputationExpressions 
// Regression test for FSHARP1.0:4972
// Debug ranges for computation expressions
module Program
open Library

let res2 = 
    eventually { 
        let x = (printfn "hello"; "hello".Length)
        return x + x }
res2 |> Eventually.force
