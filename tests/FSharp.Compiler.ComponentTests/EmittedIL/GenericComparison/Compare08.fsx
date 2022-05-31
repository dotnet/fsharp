// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT 
// Regression test for FSHARP1.0:3990
module CompareMicroPerfAndCodeGenerationTests = 
    let f7() = 
       let mutable x = 1
       let t1 = [| 0uy .. 100uy |]
       let t2 = [| 0uy .. 100uy |]
       for i = 0 to 10000000 do
           x <- compare t1 t2
       x
