// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT 
// Regression test for FSHARP1.0:5408
// For-loop emitted with unnecessary tuple
module NoAllocationOfTuple01
let loop n =
    let a = Array.zeroCreate n 
    let mutable i = -1
    for j = 1 to n do
      i <- i + 1 
      a.[ i ] <- j // creates an unnecessary temporary tuple
    a
