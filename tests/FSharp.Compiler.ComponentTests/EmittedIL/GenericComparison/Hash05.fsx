// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module HashMicroPerfAndCodeGenerationTests = 

    type Key = Key of int * int
    let f5() = 
       let mutable x = 1
       for i = 0 to 10000000 do
           hash (Key(1,2)) |> ignore

