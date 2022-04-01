// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Lists   
// Regression test for FSHARP1.0:4058
module ListExpressionSteppingTest4
module ListExpressionSteppingTest4 = 
    let f3 () = 
        [ let mutable x = 0 
          x <- x + 1
          let mutable y = 0 
          y <- y + 1
          yield x
          let z = x + y
          yield z ]

    let _ = f3()
