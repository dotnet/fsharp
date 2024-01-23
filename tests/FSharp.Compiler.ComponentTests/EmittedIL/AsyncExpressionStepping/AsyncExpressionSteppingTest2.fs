// #NoMono #NoMT #CodeGen #EmittedIL #Async
module AsyncExpressionSteppingTest2             // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest2 =
    let f2 () =
        let x = ref 0
        async { while x.Value < 4 do
                   x.Value <- x.Value + 1
                   printfn "hello" }
                   

    let _ = f2() |> Async.RunSynchronously
