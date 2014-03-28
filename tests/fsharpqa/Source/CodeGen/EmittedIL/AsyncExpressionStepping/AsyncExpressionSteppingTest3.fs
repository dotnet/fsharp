// #NoMono #NoMT #CodeGen #EmittedIL #Async #NETFX20Only #NETFX40Only 
module AsyncExpressionSteppingTest3     // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest3 = 
    let f3 () = 
        async { let x = ref 0 
                incr x
                let y = ref 0 
                incr y
                let z = !x + !y
                return z }

    let _ = f3() |> Async.RunSynchronously
