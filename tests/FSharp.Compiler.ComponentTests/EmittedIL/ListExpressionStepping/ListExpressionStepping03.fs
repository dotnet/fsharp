// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Lists   
// Regression test for FSHARP1.0:4058
module ListExpressionSteppingTest3
module ListExpressionSteppingTest3 = 
    let f2 () = 
        let mutable x = 0 
        [ while x < 4 do 
             x <- x + 1
             printfn "hello"
             yield x ]

    let _ = f2()
