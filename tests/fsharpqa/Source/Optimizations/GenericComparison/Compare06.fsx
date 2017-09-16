// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module CompareMicroPerfAndCodeGenerationTests = 
    type KeyR = { key1: int; key2 : int }
    let f5c() = 
       let mutable x = 1
       let t1 = { key1 = 1; key2 = 2 }
       let t2 = { key1 = 1; key2 = 3 }
       for i = 0 to 10000000 do
           x <- compare t1 t2
