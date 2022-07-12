// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module HashMicroPerfAndCodeGenerationTests = 
    let f4_tuple4() = 
       let mutable x = 1
       let t3 = (1,2,3,"5")
       for i = 0 to 10000000 do
           x <- hash t3 


