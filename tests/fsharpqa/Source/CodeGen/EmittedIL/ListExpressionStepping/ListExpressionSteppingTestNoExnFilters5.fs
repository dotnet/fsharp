// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Lists #NETFX20Only #NETFX40Only 
// Regression test for FSHARP1.0:4058
module ListExpressionSteppingTestNoExnFilters5
module ListExpressionSteppingTestNoExnFilters5 = 
    let f4 () = 
        [ let x = ref 0 
          try 
              let y = ref 0 
              incr y
              yield !x
              let z = !x + !y
              yield z 
          finally 
             incr x
             printfn "done" ]

    let _ = f4()
