// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module EqualsMicroPerfAndCodeGenerationTests = 
    let f4_triple() = 
       let mutable x = false
       let t1 = (1,2,3)
       let t2 = (1,2,4)
       for i = 0 to 10000000 do
           x <- (t1 = t2)
       x
