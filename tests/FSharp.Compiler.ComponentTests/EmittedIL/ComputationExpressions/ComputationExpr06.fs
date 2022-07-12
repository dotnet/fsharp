// #Regression #NoMono #NoMT #CodeGen #EmittedIL #ComputationExpressions 
// Regression test for FSHARP1.0:4972
// Debug ranges for computation expressions
module Program
open Library

let res6 = 
    eventually { 
        let mutable x = 1
        while x > 0 do 
            printfn "hello"
            printfn "hello"
            printfn "hello"
            printfn "hello"
            printfn "hello"
            x <- x - 1
        return 1 
    }
res6 |> Eventually.force |> ignore
