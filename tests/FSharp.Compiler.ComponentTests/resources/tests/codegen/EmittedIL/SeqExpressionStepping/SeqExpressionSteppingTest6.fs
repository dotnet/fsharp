// #NoMono #NoMT #CodeGen #EmittedIL #Sequences   
module SeqExpressionSteppingTest6 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTest6 = 
    let es = [1;2;3]
    let f7 () = 
        seq { for x in es do
                 printfn "hello"
                 yield x 
              for x in es do
                 printfn "goodbye"
                 yield x }

    let _ = f7() |> Seq.length


