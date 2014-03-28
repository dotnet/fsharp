// #NoMono #NoMT #CodeGen #EmittedIL #Async #NETFX20Only #NETFX40Only 
module AsyncExpressionSteppingTest4             // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest4 = 
    let f4 () = 
        async { let x = ref 0 
                try 
                    let y = ref 0 
                    incr y
                    let z = !x + !y
                    return z 
                finally 
                   incr x
                   printfn "done" }

    let _ = f4() |> Async.RunSynchronously
