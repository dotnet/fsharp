// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT 
//
// Regression test for FSHARP1.0:4461
module ZeroToArrLength01
let f1(arr:int[]) = 
    for i = 0 to arr.Length - 1 do 
        arr.[i] <- i
