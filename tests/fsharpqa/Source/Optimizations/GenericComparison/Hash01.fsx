// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module HashMicroPerfAndCodeGenerationTests = 
    let f4() = 
       let mutable x = 1
       for i = 0 to 10000000 do
           x <- hash (1,2) 
