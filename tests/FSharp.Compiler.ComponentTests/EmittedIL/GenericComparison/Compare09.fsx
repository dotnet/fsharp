// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT 
// Regression test for FSHARP1.0:3990
module CompareMicroPerfAndCodeGenerationTests = 
    let f8() = 
       let mutable x = 1
       let t1 = [| 0 .. 100 |]
       let t2 = [| 0 .. 100 |]
       for i = 0 to 100000 do
           x <- compare t1 t2
       x
