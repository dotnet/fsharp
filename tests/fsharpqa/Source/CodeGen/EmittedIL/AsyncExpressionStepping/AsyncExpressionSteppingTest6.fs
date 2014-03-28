// #NoMono #NoMT #CodeGen #EmittedIL #Async #NETFX20Only #NETFX40Only 
module AsyncExpressionSteppingTest6      // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest6 = 
    let f2 () = 
        async { let x = ref 0 
                incr x
                let y = ref 0 
                incr y
                let z = !x + !y
                return z }

    let f3 () = 
        async { let! x1 = f2()
                let! x2 = f2()
                let! x3 = f2()
                let y = ref 0 
                incr y
                let! x4 = f2()
                let z = x1 + !y + x4
                return z }

    let _ = f3() |> Async.RunSynchronously
