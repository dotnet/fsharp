// #Regression #NoMono #NoMT #CodeGen #EmittedIL #ComputationExpressions 
// Regression test for FSHARP1.0:4972
// Debug ranges for computation expressions
open Library

let res7 = 
    eventually { 
        let x = ref 1
        for v in [0 .. 3] do 
            x := !x - v
        return !x
    }
res7 |> Eventually.force
