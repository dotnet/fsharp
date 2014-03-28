// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT 
// Regression test for FSHARP1.0:3982
// Compiler should turn 'foreach' loops over arrays into 'for' loops
module ForEachOnArray01
let test3(arr: int[]) = 
     let mutable z = 0
     for x in arr do
         z <- z + x
