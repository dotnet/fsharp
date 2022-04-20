// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module EqualsMicroPerfAndCodeGenerationTests = 
    type Key = Key of int * int
    type KeyWithInnerKeys = KeyWithInnerKeys of Key * (Key * Key)
    let f9() = 
       let mutable x = false
       let key1 = KeyWithInnerKeys(Key(1,2),(Key(1,2),Key(1,2)))
       let key2 = KeyWithInnerKeys(Key(1,2),(Key(1,2),Key(1,3)))
       for i = 0 to 10000000 do
           x <- (key1 = key2)
