// #NoMono #NoMT #CodeGen #EmittedIL #Async   
module AsyncExpressionSteppingTest3     // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest3 = 
    let f3 () = 
        async { let x = ref 0 
                x.Value <- x.Value + 1
                let y = ref 0 
                y.Value <- y.Value + 1
                let z = x.Value + x.Value
                return z }

    let _ = f3() |> Async.RunSynchronously
