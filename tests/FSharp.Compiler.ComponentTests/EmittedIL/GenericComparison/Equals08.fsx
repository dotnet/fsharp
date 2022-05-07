// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module EqualsMicroPerfAndCodeGenerationTests = 
    let f8() = 
       let mutable x = false
       let t1 = [| 0 .. 100 |]
       let t2 = [| 0 .. 100 |]
       for i = 0 to 10000000 do
           x <- (t1 = t2)
       x
