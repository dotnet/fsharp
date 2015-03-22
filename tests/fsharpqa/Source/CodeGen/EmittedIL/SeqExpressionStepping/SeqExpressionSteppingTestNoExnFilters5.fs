// #NoMono #NoMT #CodeGen #EmittedIL #Sequences #NETFX20Only #NETFX40Only 
module SeqExpressionSteppingTestNoExnFilters5 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTestNoExnFilters5 = 
    let f4 () = 
        seq { let x = ref 0 
              try 
                  let y = ref 0 
                  incr y
                  yield !x
                  let z = !x + !y
                  yield z 
              finally 
                 incr x
                 printfn "done" }

    let _ = f4()|> Seq.length
