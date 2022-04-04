// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:5646

let squaresOfOneToTen() = 
    [ for x in 0 .. 2  do
           yield x * x ]

