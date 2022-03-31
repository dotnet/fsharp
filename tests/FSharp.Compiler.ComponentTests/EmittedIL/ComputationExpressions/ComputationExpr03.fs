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

let res3 = 
    eventually { 
        let! x =  res2
        let! x = 
            eventually { 
                let x = (printfn "hello"; "hello".Length)
                return 1 
            } 
        return 1 
    }
res3  |> Eventually.force
