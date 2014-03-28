// #Regression #NoMono #NoMT #CodeGen #EmittedIL #ComputationExpressions 
// Regression test for FSHARP1.0:4972
// Debug ranges for computation expressions
open Library

let res5 = 
    eventually { 
        let x = (printfn "hello"; "hello".Length)
        use x = { new System.IDisposable with member x.Dispose() = () }
        let x = (printfn "hello"; "hello".Length)
        return 1 
    }
res5 |> Eventually.force
