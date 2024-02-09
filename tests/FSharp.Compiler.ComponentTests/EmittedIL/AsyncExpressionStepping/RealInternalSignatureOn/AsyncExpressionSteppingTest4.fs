// #NoMono #NoMT #CodeGen #EmittedIL #Async   
module AsyncExpressionSteppingTest4             // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest4 = 
    let f4 () = 
        async { let x = ref 0 
                try 
                    let y = ref 0 
                    y.Value <- y.Value
                    let z = x.Value + y.Value
                    return z
                finally
                   x.Value <- x.Value
                   printfn "done" }

    let _ = f4() |> Async.RunSynchronously
