// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT 
//
// Regression test for FSHARP1.0:4461
module ZeroToArrLength02
let f1(arr:int[]) = 
    for i = 0 to Array.length arr - 1 do 
        arr.[i] <- i
