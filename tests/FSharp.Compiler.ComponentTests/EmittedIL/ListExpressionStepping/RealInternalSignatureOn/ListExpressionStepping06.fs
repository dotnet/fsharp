// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Lists   
// Regression test for FSHARP1.0:4058
module ListExpressionSteppingTest6
module ListExpressionSteppingTest6 = 
    let es = [1;2;3]
    let f7 () = 
        [ for x in es do
             printfn "hello"
             yield x 
          for x in es do
             printfn "goodbye"
             yield x ]

    let _ = f7()
