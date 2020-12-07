// #NoMono #NoMT #CodeGen #EmittedIL #Async  
module AsyncExpressionSteppingTest1      // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest1 = 

    let f1 () = 
        async { printfn "hello"
                printfn "stuck in the middle"
                printfn "goodbye"}

    let _ = f1() |> Async.RunSynchronously

