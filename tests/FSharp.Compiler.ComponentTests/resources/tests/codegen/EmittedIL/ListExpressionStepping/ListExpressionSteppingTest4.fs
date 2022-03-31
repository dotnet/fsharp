// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Lists   
// Regression test for FSHARP1.0:4058
module ListExpressionSteppingTest4
module ListExpressionSteppingTest4 = 
    let f3 () = 
        [ let x = ref 0 
          incr x
          let y = ref 0 
          incr y
          yield !x
          let z = !x + !y
          yield z ]

    let _ = f3()
