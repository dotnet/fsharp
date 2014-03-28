// #Regression #NoMono #NoMT #CodeGen #EmittedIL #ComputationExpressions 
// Regression test for FSHARP1.0:4972
// Debug ranges for computation expressions
open Library

let res6 = 
    eventually { 
        let x = ref 1
        while !x > 0 do 
            printfn "hello"
            printfn "hello"
            printfn "hello"
            printfn "hello"
            printfn "hello"
            decr x
        return 1 
    }
res6 |> Eventually.force
