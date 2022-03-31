// #NoMono #NoMT #CodeGen #EmittedIL #Async   
module AsyncExpressionSteppingTest6      // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest6 = 
    let f2 () = 
        async { let x = ref 0 
                x.Value <- x.Value + 1
                let y = ref 0 
                y.Value <- y.Value + 1
                let z = x.Value + y.Value
                return z }

    let f3 () = 
        async { let! x1 = f2()
                let! x2 = f2()
                let! x3 = f2()
                let y = ref 0 
                y.Value <- y.Value + 1
                let! x4 = f2()
                let z = x1 + y.Value + x4
                return z }

    let _ = f3() |> Async.RunSynchronously
