// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module CompareMicroPerfAndCodeGenerationTests = 
    type GenericKey<'a> = GenericKey of 'a * 'a
    let f6() = 
       let mutable x = 1
       let t1 = GenericKey(1,2)
       let t2 = GenericKey(1,3)
       for i = 0 to 10000000 do
           x <- compare t1 t2 
       x
