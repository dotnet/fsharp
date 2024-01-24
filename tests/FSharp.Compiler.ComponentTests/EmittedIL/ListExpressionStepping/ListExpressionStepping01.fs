// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Lists   
// Regression test for FSHARP1.0:4058
module ListExpressionSteppingTest1
module ListExpressionSteppingTest1 = 
    let f0 () = 
        [ yield 1 ]
    let _ = f0()

