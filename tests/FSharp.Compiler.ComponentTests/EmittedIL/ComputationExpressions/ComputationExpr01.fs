// #Regression #NoMono #NoMT #CodeGen #EmittedIL #ComputationExpressions 
// Regression test for FSHARP1.0:4972
// Debug ranges for computation expressions
module Program

open Library

let res1 = 
    eventually { 
        return 1 
    } 
res1 |> Eventually.force
