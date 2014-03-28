// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:5646

let squaresOfOneToTenB() = 
    [ for x in 0 .. 2 do
           printfn "hello"
           yield x * x ]
