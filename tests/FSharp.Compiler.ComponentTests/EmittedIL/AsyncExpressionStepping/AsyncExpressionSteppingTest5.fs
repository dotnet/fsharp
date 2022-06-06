// #NoMono #NoMT #CodeGen #EmittedIL #Async   
module AsyncExpressionSteppingTest5         // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest5 = 
    let es = [3;4;5]
    let f7 () = 
        async { for x in es do
                   printfn "hello"
                   printfn "hello 2"
                for x in es do
                   printfn "goodbye"
                   printfn "goodbye 2" }

    let _ = f7() |> Async.RunSynchronously
