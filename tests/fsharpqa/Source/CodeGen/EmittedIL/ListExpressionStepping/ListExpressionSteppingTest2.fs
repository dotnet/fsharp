// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Lists #NETFX20Only #NETFX40Only 
// Regression test for FSHARP1.0:4058
module ListExpressionSteppingTest2
module ListExpressionSteppingTest2 = 
    let f1 () = 
        [ printfn "hello"
          yield 1
          printfn "goodbye"
          yield 2]

    let _ = f1()
