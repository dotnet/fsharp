// #NoMono #NoMT #CodeGen #EmittedIL #Sequences #NETFX20Only #NETFX40Only 
module SeqExpressionSteppingTestNoExnFilters6 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTestNoExnFilters6 = 
    let es = [1;2;3]
    let f7 () = 
        seq { for x in es do
                 printfn "hello"
                 yield x 
              for x in es do
                 printfn "goodbye"
                 yield x }

    let _ = f7() |> Seq.length


