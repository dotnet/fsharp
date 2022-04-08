// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module HashMicroPerfAndCodeGenerationTests = 
    type Key = Key of int * int
    type KeyWithInnerKeys = KeyWithInnerKeys of Key * (Key * Key)
    let f9() = 
       let mutable x = 1
       let key = KeyWithInnerKeys(Key(1,2),(Key(1,2),Key(1,2)))
       for i = 0 to 10000000 do
           hash key |> ignore

